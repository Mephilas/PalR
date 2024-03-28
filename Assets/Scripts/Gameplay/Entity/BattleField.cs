using UnityEngine;

/// <summary>
/// ս��
/// </summary>
public sealed class BattleField : MonoBehaviourBase
{
    /// <summary>
    /// �з�����
    /// </summary>
    private static readonly Role[] HOSTILE_ARRAY;

    /// <summary>
    /// ս���ж�����
    /// </summary>
    private static readonly System.Collections.Generic.Queue<BattleAction> _battleActionQueue = new();

    /// <summary>
    /// ս��
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
/// ս���ж�
/// </summary>
public sealed class BattleAction
{
    /// <summary>
    /// ս�������
    /// </summary>
    public BattleActionType BattleActionType;
}

/// <summary>
/// ս�������
/// </summary>
public enum BattleActionType
{
    /// <summary>
    /// ��ͨ����
    /// </summary>
    NormalAttack,

    /// <summary>
    /// Χ��
    /// </summary>
    //Siege,

    /// <summary>
    /// �Զ�ս��
    /// </summary>
    Auto,

    /// <summary>
    /// ����ʹ��
    /// </summary>
    ItemUse,

    /// <summary>
    /// ����Ͷ��
    /// </summary>
    ItemThrow,

    /// <summary>
    /// ����
    /// </summary>
    Defend,

    /// <summary>
    /// ����
    /// </summary>
    Escape,

    /// <summary>
    /// ����ʩ��
    /// </summary>
    Skill,

    /// <summary>
    /// �ϻ�
    /// </summary>
    JointAttack
}