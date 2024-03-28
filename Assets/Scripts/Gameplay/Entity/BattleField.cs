using UnityEngine;

/// <summary>
/// 战地
/// </summary>
public sealed class BattleField : MonoBehaviourBase
{
    /// <summary>
    /// 敌方集合
    /// </summary>
    private static readonly Role[] HOSTILE_ARRAY;

    /// <summary>
    /// 战斗行动队列
    /// </summary>
    private static readonly System.Collections.Generic.Queue<BattleAction> _battleActionQueue = new();

    /// <summary>
    /// 战地
    /// </summary>
    private static SpriteRenderer _battleField;

    protected override void Awake()
    {
        base.Awake();

        GameManager_.Register(GameEventType.Battle, BattleBegin);

        GC(ref _battleField);
    }

    private void BattleBegin(string[] battleData)
    {
        Transform.localPosition = Vector2.zero;

        //_battleField.sprite = 
    }

    private void BattleEnd()
    {
        Transform.position = Const.HIDDEN_P;


    }
}

/// <summary>
/// 战斗行动
/// </summary>
public sealed class BattleAction
{
    /// <summary>
    /// 战斗活动类型
    /// </summary>
    public BattleActionType BattleActionType;
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
    /// 围攻
    /// </summary>
    //Siege,

    /// <summary>
    /// 自动战斗
    /// </summary>
    Auto,

    /// <summary>
    /// 道具使用
    /// </summary>
    ItemUse,

    /// <summary>
    /// 道具投掷
    /// </summary>
    ItemThrow,

    /// <summary>
    /// 防御
    /// </summary>
    Defend,

    /// <summary>
    /// 逃跑
    /// </summary>
    Escape,

    /// <summary>
    /// 技能施放
    /// </summary>
    Skill,

    /// <summary>
    /// 合击
    /// </summary>
    JointAttack
}