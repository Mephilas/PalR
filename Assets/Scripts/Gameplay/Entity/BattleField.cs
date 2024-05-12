using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// 战地  后续将两个Team分离，各自管理战斗单位部署行动
/// </summary>
public sealed class BattleField : SingletonBase<BattleField>
{
    /// <summary>
    /// 背景切换时长
    /// </summary>
    private const int BG_FADE_DURATION = 2;

    /// <summary>
    /// 玩家站位
    /// </summary>
    private static readonly Vector2[][] PLAYER_POSITION_ARRAY = new Vector2[4][]
    {
        new Vector2[1]{ new(1, -0.5f) },
        new Vector2[2]{ new(0.85f, -0.6f), new(1.15f, -0.4f) },
        new Vector2[3]{ new(1, -0.5f), new(0.7f, -0.7f), new(1.3f, -0.3f) },
        new Vector2[4]{ new(0.85f, -0.6f), new(1.15f, -0.4f), new(0.55f, -0.8f), new(1.45f, -0.2f) }
    };

    /// <summary>
    /// 敌人站位
    /// </summary>
    private static readonly Vector2[][] HOSTILE_POSITION_ARRAY = new Vector2[6][]
    {
        new Vector2[1]{ new(-1, 0.5f) },
        new Vector2[2]{ new(-1.15f, 0.4f), new(-0.85f, 0.6f) },
        new Vector2[3]{ new(-1, 0.5f), new(-0.7f, 0.7f), new(-1.3f, 0.3f) },
        new Vector2[4]{ new(-1.15f, 0.4f), new(-0.85f, 0.6f), new(-0.75f, 0.2f), new(-0.45f, 0.4f) },
        new Vector2[5]{ new(-1.15f, 0.4f), new(-0.85f, 0.6f), new(-0.75f, 0.2f), new(-0.45f, 0.4f), new(-1.05f, 0) },
        new Vector2[6]{ new(-1, 0.5f), new(-0.7f, 0.7f), new(-1.3f, 0.3f), new(-0.6f, 0.3f), new(-0.3f, 0.5f), new(-0.9f, 0.1f) }
    };

    /// <summary>
    /// 战地
    /// </summary>
    private static SpriteRenderer _battleField;

    /// <summary>
    /// 战斗角色
    /// </summary>
    private static Role _battleRole;

    /// <summary>
    /// 玩家集合
    /// </summary>
    public static readonly List<BattleRole> PlayerList = new();

    /// <summary>
    /// 敌人集合
    /// </summary>
    public static readonly List<BattleRole> HostileList = new();

    /// <summary>
    /// 击败敌人集合
    /// </summary>
    public static readonly List<BattleRole> BeatHostileList = new();

    /// <summary>
    /// 战斗行动角色集合
    /// </summary>
    private static readonly List<BattleAction> _battleActionList = new();

    /// <summary>
    /// 部署序号
    /// </summary>
    public static int DecideIndex { get; private set; }

    /// <summary>
    /// 行动角色
    /// </summary>
    private static BattleAction _action;

    /// <summary>
    /// 决策
    /// </summary>
    public static UnityAction PlayerDecide { get; set; }

    /// <summary>
    /// 行动阶段开始
    /// </summary>
    public static UnityAction ActionStageStart { get; set; }

    /// <summary>
    /// 行动阶段结束
    /// </summary>
    public static UnityAction ActionStageEnd { get; set; }

    /// <summary>
    /// 行动结束
    /// </summary>
    public static UnityAction ActionEnd { get; set; }

    /// <summary>
    /// UI伤害显示
    /// </summary>
    public static System.Func<UIDamage> UIDamageDisplay { get; set; }

    /// <summary>
    /// AI战斗开关，代替神奇的围攻
    /// </summary>
    public static bool AISwitch { get; set; }

    /// <summary>
    /// 特效备用集合
    /// </summary>
    private static readonly List<SpriteRenderer> _effectReserveList = new(6);

    /// <summary>
    /// 特效使用集合
    /// </summary>
    private static readonly List<SpriteRenderer> _effectUsingList = new(6);

    /// <summary>
    /// 特效回收集合
    /// </summary>
    private static readonly List<SpriteRenderer> _effectCollectList = new(6);

    private static SpriteRenderer _effect;

    private static bool _pause;

    private static Vector2[] _tempVA;

    protected override void Awake()
    {
        base.Awake();

        GameManager_.Register(GameEventType.Battle, BattleBegin);

        GC(ref _battleField);

        Hide();
    }

    /// <summary>
    /// 战斗开始
    /// </summary>
    /// <param name="battleData">战斗数据</param>
    private void BattleBegin(string[] battleData)
    {
        Transform.localPosition = Vector3.forward;

        for (int i = 0; i != GameManager_.PlayerList.Count; i++)
        {
            PlayerList.Add(DataManager_.Instance.RoleCreate<BattleRole>(GameManager_.PlayerList[i].RoleData, Transform));
        }

        _tempVA = PLAYER_POSITION_ARRAY[PlayerList.Last()];
        for (int i = 0; i != _tempVA.Length; i++)
        {
            PlayerList[i].BattleInit(_tempVA[i], GameManager_.PlayerList[i]);
        }

        _battleRole = GameManager_.RoleList[int.Parse(battleData[0])];
        for (int i = 0; i != _battleRole.RoleData.BattleIDGroup.Length; i++)
        {
            HostileList.Add(DataManager_.Instance.RoleCreate<BattleRole>(GameManager_.RoleList[_battleRole.RoleData.BattleIDGroup[i]].RoleData, Transform));
        }

        _tempVA = HOSTILE_POSITION_ARRAY[HostileList.Last()];
        for (int i = 0; i != _tempVA.Length; i++)
        {
            HostileList[i].BattleInit(_tempVA[i]);
        }

        _battleField.sprite = DataManager_.BattleGroundArray[GameManager_.BattleGroundID];
        _battleField.color = Const.CLEAR;
        _battleField.DOFade(1, BG_FADE_DURATION);
        GameManager_.Trigger(GameEventType.BGPlay, _battleRole.RoleData.BattleBG);
        GameManager_.Trigger(GameEventType.UIPanel, UIPanel.BattlePanel.ToString());
        if (-1 != _battleRole.RoleData.BattleDialogue)
            GameManager_.Trigger(GameEventType.Dialogue, _battleRole.RoleData.BattleDialogue.ToString());

        StartCoroutine(nameof(BattleC));
    }

    /// <summary>
    /// 战斗
    /// </summary>
    /// <returns></returns>
    private System.Collections.IEnumerator BattleC()
    {
        while (true)
        {
            ToolsE.LogWarning("  Deploy");

            for (int i = 0; i != PlayerList.Count; i++)
                PlayerList[i].Deployed = false;

            if (AISwitch) AIDeploy();
            else
            {
                while (PlayerDeploy())
                {
                    if (AISwitch)
                    {
                        AIDeploy();

                        break;
                    }

                    PlayerDecide();

                    while (!PlayerList[DecideIndex].Deployed)
                        yield return null;
                }
            }

            for (int i = 0; i != HostileList.Count; i++)
                if (HostileList[i].IsAlive) HostileList[i].HostileAction();

            _battleActionList.Sort();
            ActionStageStart();
            ToolsE.LogWarning("  Action");
            while (0 != _battleActionList.Count)
            {
                RoleAction();

                while (_action.ActionRole.Actioning)
                    yield return Const.WAIT_FOR_HS;

                ActionEnd();

                if (AliveCheck(false))
                {
                    GameManager_.Trigger(GameEventType.BGPlay, "-1");
                    //GameManager_.Trigger(new(GameEventType.SoundEffects, ""));
                    GameManager_.Trigger(GameEventType.UIPanel, UIPanel.SettlementPanel.ToString());

                    StopCoroutine(nameof(BattleC));
                }
                else if (AliveCheck(true))
                {
                    _pause = true;

                    _battleActionList.Clear();

                    GameManager_.Trigger(GameEventType.BGPlay, "-1");
                    //GameManager_.Trigger(new(GameEventType.SoundEffects, ""));
                    GameManager_.Trigger(GameEventType.UIPanel, UIPanel.GGPanel.ToString());

                    while (_pause) yield return Const.WAIT_FOR_2S;
                }
            }

            _battleActionList.Clear();

            ActionStageEnd();
        }
    }

    /// <summary>
    /// 存活检查
    /// </summary>
    private static bool AliveCheck(bool player)
    {
        List<BattleRole> roleList = player ? PlayerList : HostileList;

        for (int i = 0; i != roleList.Count; i++)
        {
            if (roleList[i].IsAlive)
                return false;
        }

        return true;
    }

    /// <summary>
    /// 复活
    /// </summary>
    public static void Resurrect()
    {
        for (int i = 0; i != PlayerList.Count; i++)
            PlayerList[i].Resurrect();

        GameManager_.Trigger(GameEventType.BGPlay, "-2");
        GameManager_.Trigger(GameEventType.UIPanel, UIPanel.BattlePanel.ToString());

        _pause = false;
    }

    /// <summary>
    /// AI部署
    /// </summary>
    private static void AIDeploy()
    {
        for (int i = 0; i != PlayerList.Count; i++)
            if (PlayerList[i].IsAlive) PlayerList[i].AIAction();
    }

    /// <summary>
    /// 玩家未部署
    /// </summary>
    /// <returns>是/否</returns>
    private static bool PlayerDeploy()
    {
        for (int i = 0; i != PlayerList.Count; i++)
        {
            if (RoleBattleState.Decease != PlayerList[i].BattleState && !PlayerList[i].Deployed)
            {
                DecideIndex = i;

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 确认
    /// </summary>
    public static void Confirm(BattleAction battleAction)
    {
        if (BattleActionType.JointAttack == battleAction.ActionType)
        {
            for (int i = 0; i != _battleActionList.Count; i++)
            {
                if (null != _battleActionList[i].ActionRole.RoleEntity)
                    _battleActionList.RemoveAt(i--);
            }

            for (int i = 0; i != PlayerList.Count; i++) PlayerList[i].Deployed = true;
        }

        _battleActionList.Add(battleAction);

        battleAction.ActionRole.Deployed = true;
    }

    /// <summary>
    /// 角色死亡
    /// </summary>
    /// <param name="role">角色</param>
    /// <param name="isHostile">敌/我</param>
    public static void RoleDecease(BattleRole role, bool isHostile)
    {
        for (int i = 0; i != _battleActionList.Count; i++)
        {
            if (_battleActionList[i].ActionRole == role)
                _battleActionList.RemoveAt(i--);
        }

        if (isHostile)
        {
            HostileList.Remove(role);
            BeatHostileList.Add(role);
        }
    }

    /// <summary>
    /// 回滚
    /// </summary>
    public static void Rollback()
    {
        if (0 != DecideIndex)
        {
            --DecideIndex;

            for (int i = 0; i != _battleActionList.Count; i++)
            {
                if (_battleActionList[i].ActionRole == PlayerList[DecideIndex])
                    _battleActionList.RemoveAt(i--);
            }

            PlayerList[DecideIndex].Deployed = false;
            PlayerDecide();
        }
    }

    /// <summary>
    /// 角色行动
    /// </summary>
    private static void RoleAction()
    {
        (_action = _battleActionList[^1]).ActionRole.Action(_action);

        _battleActionList.RemoveAt(_battleActionList.Last());
    }

    /// <summary>
    /// 围攻
    /// </summary>
    public static void AIBattle()
    {
        GameManager_.Trigger(GameEventType.UIPanel, UIPanel.BattlePanel.ToString());

        AISwitch = true;
    }

    /// <summary>
    /// 防御
    /// </summary>
    public static void Defense()
    {
        PlayerList[DecideIndex].Deployed = PlayerList[DecideIndex].Defensing = true;

        GameManager_.Trigger(GameEventType.UIPanel, UIPanel.BattlePanel.ToString());
    }

    /// <summary>
    /// 逃跑
    /// </summary>
    public static void RunForYourLife()
    {
        for (int i = 0; i != PlayerList.Count; i++)
        {
            PlayerList[DecideIndex].Defensing = true;

            Confirm(new(PlayerList[i], BattleActionType.Retreat));
        }

        GameManager_.Trigger(GameEventType.UIPanel, UIPanel.BattlePanel.ToString());
    }

    /// <summary>
    /// 胜负已分
    /// </summary>
    public void BattleEnd(bool retreat = false)
    {
        _battleField.DOFade(0, BG_FADE_DURATION).onComplete = () =>
        {
            for (int i = 0; i != PlayerList.Count; i++) Destroy(PlayerList[i].gameObject);
            for (int i = 0; i != HostileList.Count; i++) Destroy(HostileList[i].gameObject);
            PlayerList.Clear();
            HostileList.Clear();
            BeatHostileList.Clear();
            GameManager_.Trigger(GameEventType.BGRecover);

            if (-1 == _battleRole.RoleData.BeatDialogue)
                GameManager_.Trigger(GameEventType.UIPanel, UIPanel.BasicPanel.ToString());
            else if (!retreat)
                GameManager_.Trigger(GameEventType.Dialogue, _battleRole.RoleData.BeatDialogue.ToString());

            for (int i = 0; i != _effectUsingList.Count; i++)
                _effectCollectList.Add(_effectUsingList[i]);

            for (int i = 0; i != _effectCollectList.Count; i++)
            {
                _effectUsingList.Remove(_effectCollectList[i]);
                _effectReserveList.Add(_effectCollectList[i]);
            }

            _effectCollectList.Clear();

            Hide();
        };
    }

    /// <summary>
    /// 特效获取
    /// </summary>
    /// <returns>特效</returns>
    public static SpriteRenderer EffectGet()
    {
        if (0 == _effectReserveList.Count)
            _effect = Instantiate(Resources.Load<GameObject>("Prefabs/SkillEffect"), Transform).GetComponent<SpriteRenderer>();
        else
        {
            _effect = _effectReserveList[^1];
            _effectReserveList.RemoveAt(_effectReserveList.Last());
        }

        _effectUsingList.Add(_effect);

        return _effect;
    }
}


/// <summary>
/// 战斗行动
/// </summary>
public sealed class BattleAction : System.IComparable<BattleAction>
{
    /// <summary>
    /// 行动角色
    /// </summary>
    public readonly BattleRole ActionRole;

    /// <summary>
    /// 战斗行动类型
    /// </summary>
    public readonly BattleActionType ActionType;

    /// <summary>
    /// 源ID
    /// </summary>
    public readonly int SourceID = -1;

    /// <summary>
    /// 目标
    /// </summary>
    public readonly BattleRole Target;

    public int CompareTo(BattleAction battleAction) => Speed(battleAction) < Speed(this) ? 1 : -1;

    private float Speed(BattleAction battleAction)
    {
        float speed;

        if (BattleActionType.JointAttack == battleAction.ActionType)
        {
            speed = 10;
        }
        else if (BattleActionType.Defense == battleAction.ActionType)
        {
            speed = 5;
        }
        else if (BattleActionType.Item == battleAction.ActionType
            || null == battleAction.ActionRole.RoleEntity && BattleActionType.Skill == battleAction.ActionType
            || null != battleAction.ActionRole.RoleEntity && BattleActionType.Skill == battleAction.ActionType && !DataManager_.SkillDataArray[battleAction.SourceID].Attack)
        {
            speed = 3;
        }
        else if (BattleActionType.Retreat == battleAction.ActionType)
        {
            speed = 2;
        }
        else speed = 1;
        
        speed *= battleAction.ActionRole.Speed * Random.Range(0.9f, 1.1f);
        //还缺仙风云体术速度*3
        if (battleAction.ActionRole.HP * 5 < battleAction.ActionRole.HPMax)
            speed *= 0.5f;
        //ToolsE.LogWarning(battleAction.ActionRole.name + "  " + battleAction.ActionRole.Speed + "  " + speed);
        return speed;
    }

    public BattleAction(BattleRole actionRole, BattleActionType actionType, int sourceID = -1)
    {
        ActionRole = actionRole;

        ActionType = actionType;

        SourceID = sourceID;
    }

    public BattleAction(BattleRole actionRole, BattleActionType actionType, int sourceID, BattleRole target = null)
    {
        ActionRole = actionRole;

        ActionType = actionType;

        SourceID = sourceID;

        Target = target;
    }
}


/// <summary>
/// 战斗活动类型
/// </summary>
public enum BattleActionType
{
    /// <summary>
    /// 普通攻击
    /// </summary>
    NormalAttack,

    /// <summary>
    /// 围攻，make a really big fucking hole !
    /// </summary>
    //Siege,

    /// <summary>
    /// 道具使用/投掷
    /// </summary>
    Item,

    /// <summary>
    /// 防御
    /// </summary>
    Defense,

    /// <summary>
    /// 逃跑
    /// </summary>
    Retreat,

    /// <summary>
    /// 仙术施放
    /// </summary>
    Skill,

    /// <summary>
    /// 合击
    /// </summary>
    JointAttack
}