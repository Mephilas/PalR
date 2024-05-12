/// <summary>
/// 战斗扩展面板
/// </summary>
public sealed class BattleExtensionPanel : UIPanelBase
{
    /// <summary>
    /// 选择循环开关
    /// </summary>
    private static bool SELECT_LOOP_SWITCH = true;

    /// <summary>
    /// 扩展选项
    /// </summary>
    private static readonly UnityEngine.Events.UnityAction[] BattleExtensionArray = new UnityEngine.Events.UnityAction[4]
    {
        BattleField.AIBattle,
        () => GameManager_.Trigger(GameEventType.UIPanel, UIPanel.BattleItemPanel.ToString()),
        BattleField.Defense,
        BattleField.RunForYourLife
    };

    /// <summary>
    /// 选择器集合
    /// </summary>
    private static TextSelector[] _selectorArray;

    /// <summary>
    /// 当前序号
    /// </summary>
    private static int _currentIndex;

    protected override void Awake()
    {
        base.Awake();

        CGC(ref _selectorArray);
    }

    protected override void Escape() => GameManager_.Trigger(BATTLE_PANEL_EVENT);

    protected override void Enter() => _selectorArray[_currentIndex].Selected();

    protected override void Up() => Select(_currentIndex - 1);

    protected override void Down() => Select(_currentIndex + 1);

    protected override void Left() => Select(0);

    protected override void Right() => Select(_selectorArray.Last());

    protected override void Start()
    {
        base.Start();

        for (int i = 0; i != _selectorArray.Length; i++)
        {
            int index = i;

            _selectorArray[i].Init(() => Select(index), BattleExtensionArray[index]);
        }
    }

    private static void Select(in int index)
    {
        if (index != _currentIndex)
        {
            if (SELECT_LOOP_SWITCH)
            {
                _selectorArray[_currentIndex].Unselect();

                if (index < 0)
                {
                    _currentIndex = _selectorArray.Last();
                }
                else if (_selectorArray.Last() < index)
                {
                    _currentIndex = 0;
                }
                else
                {
                    _currentIndex = index;
                }

                _selectorArray[_currentIndex].Select();
            }
            else if (_selectorArray.Valid(index))
            {
                _selectorArray[_currentIndex].Unselect();
                _selectorArray[_currentIndex = index].Select();
            }
        }
    }

    public override void Active(string[] argumentArray = null)
    {
        base.Active();

        _selectorArray[_currentIndex].Select();
    }

    public override void Inactive(bool hide)
    {
        base.Inactive(hide);

        _selectorArray[_currentIndex].Unselect(!hide);
    }
}