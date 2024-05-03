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
    /// 玩家存活数
    /// </summary>
    private static int _playerAliveCount;

    /// <summary>
    /// 敌人集合
    /// </summary>
    public static readonly List<BattleRole> HostileList = new();

    /// <summary>
    /// 敌人存活数
    /// </summary>
    private static int _hostileAliveCount;

    /// <summary>
    /// 战斗行动集合
    /// </summary>
    private static readonly Dictionary<BattleRole, BattleAction> _battleActionDic = new();

    /// <summary>
    /// 战斗行动角色集合
    /// </summary>
    private static readonly List<BattleRole> _battleActionList = new();

    /// <summary>
    /// 部署序号
    /// </summary>
    public static int DecideIndex { get; private set; }

    /// <summary>
    /// 行动角色
    /// </summary>
    private static BattleRole _actionRole;

    /// <summary>
    /// 决策
    /// </summary>
    public static UnityAction PlayerDecide { get; set; }

    /// <summary>
    /// 行动开始
    /// </summary>
    public static UnityAction ActionStart { get; set; }

    /// <summary>
    /// 行动结束
    /// </summary>
    public static UnityAction ActionEnd { get; set; }

    /// <summary>
    /// AI战斗开关，代替神奇的围攻
    /// </summary>
    public static bool AISwitch { get; set; }

    /// <summary>
    /// 特效
    /// </summary>
    private static SpriteRenderer _effect;

    private static Vector2[] _tempVA;

    protected override void Awake()
    {
        base.Awake();

        GameManager_.Register(GameEventType.Battle, BattleBegin);

        GC(ref _battleField);

        CGC(ref _effect, "Effect");

        Hide();
    }

    /// <summary>
    /// 战斗开始
    /// </summary>
    /// <param name="battleData">战斗数据</param>
    private void BattleBegin(string[] battleData)
    {
        PlayerList.Clear();
        HostileList.Clear();

        Transform.localPosition = Vector3.forward;

        for (int i = 0; i != GameManager_.PlayerList.Count; i++)
        {
            PlayerList.Add(DataManager_.Instance.RoleCreate<BattleRole>(GameManager_.PlayerList[i].RoleData, Transform));
        }

        _tempVA = PLAYER_POSITION_ARRAY[PlayerList.Count - 1];
        for (int i = 0; i != _tempVA.Length; i++)
        {
            PlayerList[i].BattleInit(_tempVA[i], GameManager_.PlayerList[i]);
        }

        _battleRole = GameManager_.RoleList[int.Parse(battleData[0])];
        for (int i = 0; i != _battleRole.RoleData.BattleIDGroup.Length; i++)
        {
            HostileList.Add(DataManager_.Instance.RoleCreate<BattleRole>(GameManager_.RoleList[_battleRole.RoleData.BattleIDGroup[i]].RoleData, Transform));
        }

        _tempVA = HOSTILE_POSITION_ARRAY[HostileList.Count - 1];
        for (int i = 0; i != _tempVA.Length; i++)
        {
            HostileList[i].BattleInit(_tempVA[i]);
        }

        _battleField.sprite = DataManager_.BattleGroundArray[GameManager_.BattleGroundID];
        _battleField.color = Const.CLEAR;
        _battleField.DOFade(1, BG_FADE_DURATION);
        GameManager_.Trigger(new GameEventData(GameEventType.BGPlay, _battleRole.RoleData.BattleBG));
        GameManager_.Trigger(new GameEventData(GameEventType.UIPanel, UIPanel.BattlePanel.ToString()));

        StartCoroutine(nameof(BattleI));
    }

    /// <summary>
    /// 战斗协程
    /// </summary>
    /// <returns></returns>
    private System.Collections.IEnumerator BattleI()
    {
        while (PlayerAliveCheck() && HostileAliveCheck())
        {
            yield return null;

            ToolsE.LogWarning("  Deploy");

            for (int i = 0; i != PlayerList.Count; i++)
                PlayerList[i].Deployed = false;

            if (AISwitch) AIDeploy();
            else
            {
                while (-1 != PlayerDeploy())
                {
                    yield return null;

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
                if (HostileList[i].IsAlive) _battleActionDic.Add(HostileList[i], HostileList[i].HostileAction());

            ToolsE.LogWarning("  Action");

            BattleSort();
            ActionStart();

            while (0 != _battleActionList.Count)
            {
                yield return null;

                RoleAction();

                while (_actionRole.Actioning)
                    yield return Const.WAIT_FOR_HS;
            }

            _battleActionDic.Clear();
            _battleActionList.Clear();

            ActionEnd();
        }

        ToolsE.LogWarning("  BattleSettlement");

        BattleSettlement();
    }

    /// <summary>
    /// 玩家存活检查
    /// </summary>
    private static bool PlayerAliveCheck()
    {
        _playerAliveCount = 0;

        for (int i = 0; i != PlayerList.Count; i++)
        {
            if (PlayerList[i].IsAlive)
                _playerAliveCount++;
        }

        return 0 != _playerAliveCount;
    }

    /// <summary>
    /// 敌人存活检查
    /// </summary>
    private static bool HostileAliveCheck()
    {
        _hostileAliveCount = 0;

        for (int i = 0; i != HostileList.Count; i++)
        {
            if (HostileList[i].IsAlive)
                _hostileAliveCount++;
        }

        return 0 != _hostileAliveCount;
    }

    /// <summary>
    /// AI部署
    /// </summary>
    private static void AIDeploy()
    {
        for (int i = 0; i != PlayerList.Count; i++)
            if (PlayerList[i].IsAlive) _battleActionDic.Add(PlayerList[i], PlayerList[i].HostileAction());
    }

    /// <summary>
    /// 玩家部署
    /// </summary>
    /// <returns>是否</returns>
    private static int PlayerDeploy()
    {
        for (int i = 0; i != PlayerList.Count; i++)
        {
            if (!PlayerList[i].Deployed)
                return DecideIndex = i;
        }

        return DecideIndex = -1;
    }

    /// <summary>
    /// 确认
    /// </summary>
    public static void Confirm(BattleAction battleAction)
    {
        _battleActionDic.Add(PlayerList[DecideIndex], battleAction);

        PlayerList[DecideIndex].Deployed = true;
    }

    /// <summary>
    /// 回滚
    /// </summary>
    public static void Rollback()
    {
        if (0 != DecideIndex)
        {
            _battleActionDic.Remove(PlayerList[DecideIndex--]);

            PlayerDecide();
        }
    }

    /// <summary>
    /// 战斗排序
    /// </summary>
    private static void BattleSort()
    {
        foreach (BattleRole battleRole in _battleActionDic.Keys)
            _battleActionList.Add(battleRole);

        _battleActionList.Sort();
    }

    /// <summary>
    /// 角色行动
    /// </summary>
    private static void RoleAction()
    {
        (_actionRole = _battleActionList[0]).Action(_battleActionDic[_actionRole]);

        _battleActionList.RemoveAt(0);
    }

    /// <summary>
    /// 围攻
    /// </summary>
    public static void AIBattle()
    {
        GameManager_.Trigger(new GameEventData(GameEventType.UIPanel, UIPanel.BattlePanel.ToString()));

        AISwitch = true;
    }

    /// <summary>
    /// 防御
    /// </summary>
    public static void Defense()
    {
        Confirm(new(BattleActionType.Defense));

        GameManager_.Trigger(new GameEventData(GameEventType.UIPanel, UIPanel.BattlePanel.ToString()));
    }

    /// <summary>
    /// 逃跑
    /// </summary>
    public static void RunForYourLife()
    {
        for (int i = 0; i != PlayerList.Count; i++)
            Confirm(new(BattleActionType.Retreat));
    }

    /// <summary>
    /// 战斗结算
    /// </summary>
    private void BattleSettlement()
    {
        GameManager_.Trigger(new GameEventData(GameEventType.BGPlay, "-1"));

        _battleField.DOFade(0, BG_FADE_DURATION).onComplete = BattleEnd;
    }

    /// <summary>
    /// 胜负已分
    /// </summary>
    private void BattleEnd()
    {
        Hide();
        GameManager_.Trigger(new GameEventData(GameEventType.BGRecover));
        GameManager_.Trigger(new GameEventData(GameEventType.UIPanel, UIPanel.BasicPanel.ToString()));
    }
}


/// <summary>
/// 战斗行动
/// </summary>
public sealed class BattleAction
{
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

    public BattleAction(BattleActionType actionType, int sourceID = -1)
    {
        ActionType = actionType;

        SourceID = sourceID;
    }

    public BattleAction(BattleActionType actionType, int sourceID, BattleRole target = null)
    {
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
    Siege,

    /// <summary>
    /// 自动战斗
    /// </summary>
    Auto,

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
    /// 技能施放
    /// </summary>
    Skill,

    /// <summary>
    /// 合击
    /// </summary>
    JointAttack
}