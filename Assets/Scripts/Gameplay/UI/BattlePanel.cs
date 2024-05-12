using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 战斗面板
/// </summary>
public sealed class BattlePanel : UIPanelBase
{
    /// <summary>
    /// 部署/选择动画位置
    /// </summary>
    private static readonly Vector2 DEPLOY_ANIM_P = new(-20, 225),
                                    SELECT_ANIM_P = new(-20, 180);

    /// <summary>
    /// 部署/选择图标颜色
    /// </summary>
    private static readonly Color DEPLOY_COLOR_0 = new(0.9882354f, 0.9882354f, 0.909804f),
                                  DEPLOY_COLOR_1 = new(0.8627452f, 0.7215686f, 0.5490196f),
                                  SELECT_COLOR_0 = new(0.7843138f, 0.345098f, 0.2980392f),
                                  SELECT_COLOR_1 = new(0.9254903f, 0.5803922f, 0.5176471f);

    /// <summary>
    /// 决策选项
    /// </summary>
    private static readonly UnityEngine.Events.UnityAction[] BATTLE_DECIDE_ARRAY = new UnityEngine.Events.UnityAction[]
    {
        NormalAttack,
        Extension,
        Skill,
        JointAttack
    };

    /// <summary>
    /// 决策栏
    /// </summary>
    private static RectTransform _decideT;

    /// <summary>
    /// 玩家栏
    /// </summary>
    private static RectTransform _playerT;

    /// <summary>
    /// 选择器集合
    /// </summary>
    private static Selector[] _selectorArray;

    /// <summary>
    /// 角色集合
    /// </summary>
    private static PlayerProfile[] _playerArray;

    /// <summary>
    /// 角色决策动画
    /// </summary>
    private static BattleSelector _decideAnim;
    
    /// <summary>
    /// 队友选择动画
    /// </summary>
    private static BattleSelector _selectAnim;

    /// <summary>
    /// 全屏遮罩
    /// </summary>
    private static RectTransform _maskT;

    /// <summary>
    /// 当前序号
    /// </summary>
    private static int _currentIndex;

    /// <summary>
    /// 决策角色
    /// </summary>
    private static BattleRole DecidePlayer { get { return BattleField.PlayerList[BattleField.DecideIndex]; } }

    /// <summary>
    /// 目标序号
    /// </summary>
    private static int _targetIndex = -1;

    /// <summary>
    /// 选择来源ID
    /// </summary>
    private static int _selectSourceID;

    /// <summary>
    /// 选择中
    /// </summary>
    private static bool _selecting;

    /// <summary>
    /// 当前选择类型
    /// </summary>
    private static BattleActionType _selectType;

    /// <summary>
    /// 选择队列，敌我
    /// </summary>
    private static List<BattleRole> _selectRoleList;

    /// <summary>
    /// UI伤害备用集合
    /// </summary>
    private static readonly List<UIDamage> _uiDamageReserveList = new(6);

    /// <summary>
    /// UI伤害使用集合
    /// </summary>
    private static readonly List<UIDamage> _uiDamageUsingList = new(6);

    /// <summary>
    /// UI伤害回收集合
    /// </summary>
    private static readonly List<UIDamage> _uiDamageCollectList = new(6);

    private static UIDamage _uiDamage;

    /// <summary>
    /// 合击检查
    /// </summary>
    private static bool JointAttackCheck
    {
        get
        {
            if (1 == BattleField.PlayerList.Count) return false;

            for (int i = 0; i != BattleField.PlayerList.Count; i++)
            {
                if (BattleField.PlayerList[i].HP * 5 < BattleField.PlayerList[i].HPMax)
                    return false;
            }

            return true;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        CGC(ref _decideT, "Decide");
        CGC(ref _playerT, "Player");
        CGC(ref _selectorArray);
        CGC(ref _playerArray);
        CGC(ref _decideAnim, "Arrow");
        CGC(ref _selectAnim, "Select");
        CGC(ref _maskT, "Mask");
    }

    /// <summary>
    /// 返回
    /// </summary>
    protected override void Escape()
    {
        if (_selecting) ExitSelect();
        else BattleField.Rollback();

        _selectAnim.Hide();
    }

    /// <summary>
    /// 进入
    /// </summary>
    protected override void Enter()
    {
        if (_selecting)
        {
            ExitSelect();
            RoleSelected();
        }
        else _selectorArray[_currentIndex].Selected();
    }

    /// <summary>
    /// 上
    /// </summary>
    protected override void Up()
    {
        if (_selecting) RoleSelect(0 == _targetIndex ? _selectRoleList.Last() : 0, false);
        else Select(0);
    }

    /// <summary>
    /// 下
    /// </summary>
    protected override void Down()
    {
        if (_selecting) RoleSelect(_selectRoleList.Last() == _targetIndex ? 0 : _selectRoleList.Last(), true);
        else Select(1);
    }

    /// <summary>
    /// 左
    /// </summary>
    protected override void Left()
    {
        if (_selecting) RoleSelect(0 == _targetIndex ? _selectRoleList.Last() : _targetIndex - 1, false);
        else Select(2);
    }

    /// <summary>
    /// 右
    /// </summary>
    protected override void Right()
    {
        if (_selecting) RoleSelect(_selectRoleList.Last() == _targetIndex ? 0 : _targetIndex + 1, true);
        else if (JointAttackCheck) Select(3);
    }

    protected override void Start()
    {
        base.Start();

        BattleField.PlayerDecide = PlayerDecide;
        BattleField.ActionStageStart = ActionStageStart;
        BattleField.ActionStageEnd = ActionStageEnd;
        BattleField.ActionEnd = ActionEnd;
        BattleField.UIDamageDisplay = UIDamageDisplay;

        for (int i = 0; i != _selectorArray.Length; i++)
        {
            int index = i;

            _selectorArray[i].Init(() => Select(index), BATTLE_DECIDE_ARRAY[index]);
        }
    }

    private static void Select(int index)
    {
        _selectorArray[_currentIndex].Unselect();
        _selectorArray[_currentIndex = index].Select();
    }

    /// <summary>
    /// 普通攻击
    /// </summary>
    private static void NormalAttack()
    {
        if (DataManager_.ItemDataArray[DecidePlayer.RoleEntity.OutfitDic[OutfitType.Weapon]].Ranged)
            BattleField.Confirm(new(DecidePlayer, BattleActionType.NormalAttack));
        else
            TargetSelect(BattleActionType.NormalAttack);
    }

    /// <summary>
    /// 扩展
    /// </summary>
    private static void Extension() => GameManager_.Trigger(GameEventType.UIPanel, UIPanel.BattleExtensionPanel.ToString(), "False");

    /// <summary>
    /// 仙术
    /// </summary>
    private static void Skill() => GameManager_.Trigger(GameEventType.UIPanel, UIPanel.BattleSkillPanel.ToString(), "False", BattleField.DecideIndex.ToString());

    /// <summary>
    /// 合击
    /// </summary>
    private static void JointAttack()
    {
        _selectSourceID = DecidePlayer.JointSkill.ID;

        if (DecidePlayer.JointSkill.Effect2All)
        {
            BattleField.Confirm(new(DecidePlayer, BattleActionType.JointAttack, _selectSourceID));

            for (int i = 0; i != BattleField.PlayerList.Count; i++)
                BattleField.PlayerList[i].Deployed = true;
        }
        else
        {
            TargetSelect(BattleActionType.JointAttack);
        }
    }

    /// <summary>
    /// 玩家决策
    /// </summary>
    private static void PlayerDecide()
    {
        _decideT.anchoredPosition = Vector2.zero;
        _decideAnim.Display(ToolsE.W2S(DecidePlayer.Transform.position), DEPLOY_ANIM_P, DEPLOY_COLOR_0, DEPLOY_COLOR_1);
        Select(0);
    }

    /// <summary>
    /// 目标选择
    /// </summary>
    /// <param name="battleActionType">类型</param>
    /// <param name="selectHostile">此面向敌</param>
    private static void TargetSelect(BattleActionType battleActionType, bool selectHostile = true)
    {
        _selecting = true;

        _decideT.anchoredPosition = Const.HIDDEN_P;

        _selectType = battleActionType;

        _selectRoleList = selectHostile ? BattleField.HostileList : BattleField.PlayerList;

        RoleSelect(0, true);
    }

    /// <summary>
    /// 角色选择
    /// </summary>
    /// <param name="index">序号</param>
    /// <returns>存活</returns>
    private static void RoleSelect(in int index, bool direction)
    {
        if (BattleField.HostileList == _selectRoleList)
        {
            if (_selectRoleList.Valid(_targetIndex)) _selectRoleList[_targetIndex].HostileSelect(false);
            _selectRoleList[_targetIndex = index].HostileSelect(true);
        }
        else
        {
            _targetIndex = index;

            while (RoleBattleState.Decease == _selectRoleList[_targetIndex].BattleState)
            {
                _targetIndex += direction ? 1 : -1;
                if (_selectRoleList.Count == _targetIndex) _targetIndex = 0;
                else if (-1 == _targetIndex) _targetIndex = _selectRoleList.Last();
            }

            _selectAnim.Display(ToolsE.W2S(_selectRoleList[_targetIndex].Transform.position), SELECT_ANIM_P, SELECT_COLOR_0, SELECT_COLOR_1);
        }
    }

    /// <summary>
    /// 退出选择
    /// </summary>
    private static void ExitSelect()
    {
        _selecting = false;
        _selectRoleList[_targetIndex].HostileSelect(false);
        _decideT.anchoredPosition = Vector2.zero;
    }

    /// <summary>
    /// 角色选择完毕
    /// </summary>
    private static void RoleSelected()
    {
        _selectRoleList[_targetIndex].HostileSelect(false);

        BattleField.Confirm(new(DecidePlayer, _selectType, _selectSourceID, _selectRoleList[_targetIndex]));

        _targetIndex = -1;

        _selectAnim.Hide();
    }

    /// <summary>
    /// 行动阶段开始
    /// </summary>
    private void ActionStageStart()
    {
        InputSwitch = false;

        UIDisplay(_maskT);
        UIHide(_decideT);
        UIHide(_playerT);

        _decideAnim.Hide();
    }

    /// <summary>
    /// 行动阶段结束
    /// </summary>
    private void ActionStageEnd()
    {
        InputSwitch = true;

        UIHide(_maskT);
        UIDisplay(_decideT);
        UIDisplay(_playerT);

        if (JointAttackCheck) _selectorArray[^1].Effect(false);
        else _selectorArray[^1].Effect();
    }

    /// <summary>
    /// 行动结束
    /// </summary>
    private void ActionEnd()
    {
        for (int i = 0; i != GameManager_.PlayerList.Count; i++)
            _playerArray[i].Refresh();

        for (int i = 0; i != _uiDamageUsingList.Count; i++)
        {
            if (!_uiDamageUsingList[i].Floating)
                _uiDamageCollectList.Add(_uiDamageUsingList[i]);

            //i--;
        }

        for (int i = 0; i != _uiDamageCollectList.Count; i++)
        {
            _uiDamageUsingList.Remove(_uiDamageCollectList[i]);
            _uiDamageReserveList.Add(_uiDamageCollectList[i]);
        }

        _uiDamageCollectList.Clear();
    }

    /// <summary>
    /// UI伤害显示
    /// </summary>
    /// <returns></returns>
    private UIDamage UIDamageDisplay()
    {
        if (0 == _uiDamageReserveList.Count)
            _uiDamage = Instantiate(Resources.Load<GameObject>("Prefabs/" + nameof(UIDamage)), Transform).GetComponent<UIDamage>();
        else
        {
            _uiDamage = _uiDamageReserveList[^1];
            _uiDamageReserveList.RemoveAt(_uiDamageReserveList.Last());
        }

        _uiDamageUsingList.Add(_uiDamage);

        return _uiDamage;
    }

    public override void Active(string[] argumentArray = null)
    {
        base.Active();

        for (int i = 0; i != _selectorArray.Length; i++)
            _selectorArray[i].Effect(false);

        if (!JointAttackCheck)
            _selectorArray[^1].Effect();

        Select(0);

        for (int i = 0; i != _playerArray.Length; i++)
            _playerArray[i].Hide();

        for (int i = 0; i != BattleField.PlayerList.Count; i++)
        {
            int index = i;

            _playerArray[index].Init(BattleField.PlayerList[index]);
        }

        if (2 < argumentArray.Length)
        {
            _selectSourceID = int.Parse(argumentArray[2]);
            bool _skillOrItem = bool.Parse(argumentArray[3]);
            bool _effect2All = _skillOrItem ? DataManager_.SkillDataArray[_selectSourceID].Effect2All : DataManager_.ItemDataArray[_selectSourceID].Effect2All;
            BattleActionType _battleActionType = _skillOrItem ? BattleActionType.Skill : BattleActionType.Item;
            bool _selectHostile = _skillOrItem ? DataManager_.SkillDataArray[_selectSourceID].Attack : DataManager_.ItemDataArray[_selectSourceID].Throw;

            if (_effect2All)
            {
                BattleField.Confirm(new(DecidePlayer, _battleActionType, _selectSourceID));
            }
            else
            {
                TargetSelect(_battleActionType, _selectHostile);
            }
        }
    }
}