/// <summary>
/// 存档面板
/// </summary>
public sealed class SLPanel : UIPanelBase
{
    /// <summary>
    /// 系统面板事件
    /// </summary>
    private static readonly GameEventData SYSTEM_PANEL_EVENT = new(GameEventType.UIPanel, UIPanel.SystemPanel.ToString());

    /// <summary>
    /// 选择器集合
    /// </summary>
    private static Selector[] _selectorArray;

    /// <summary>
    /// 上轮序号
    /// </summary>
    private static int _lastIndex;

    /// <summary>
    /// 当前序号
    /// </summary>
    private static int _currentIndex;

    /// <summary>
    /// 存档/读档
    /// </summary>
    public static bool SaveOrLoad { get; set; }

    protected override void Awake()
    {
        base.Awake();

        AC<ButtonE>().Init(Escape);
        CGC(ref _selectorArray);
    }

    protected override void Escape() => GameManager_.Trigger(GameManager_.InGame ? SYSTEM_PANEL_EVENT : MAIN_PANEL_EVENT);

    protected override void Enter() => _selectorArray[_currentIndex].Selected();

    protected override void Up()
    {
        if (-1 == --_currentIndex) _currentIndex = _selectorArray.Last();
        Select(_currentIndex);
    }

    protected override void Down()
    {
        if (_selectorArray.Length == ++_currentIndex) _currentIndex = 0;
        Select(_currentIndex);
    }

    protected override void Left()
    {
        if (0 != _currentIndex) Select(_currentIndex = 0);
    }

    protected override void Right()
    {
        if (_selectorArray.Last() != _currentIndex) Select(_currentIndex = _selectorArray.Last());
    }

    private static void Select(int index)
    {
        _selectorArray[_lastIndex].Unselect();
        _selectorArray[_lastIndex = index].Select();
    }

    protected override void Start()
    {
        base.Start();

        for (int i = 0; i != _selectorArray.Length; i++)
        {
            int index = i;

            _selectorArray[i].Init(() => Select(_currentIndex = index), () =>
            {
                UIManager_.PanelClear();
                GameManager_.InGame = true;
                GameManager_.Trigger(BASIC_PANEL_EVENT);
                GameManager_.Trigger(new(SaveOrLoad ? GameEventType.Save : GameEventType.Load, index.ToString()));
            });
        }
    }

    public override void Active()
    {
        base.Active();

        _selectorArray[_currentIndex].Select();
    }

    public override void Inactive(bool hide)
    {
        base.Inactive(hide);

        _selectorArray[_currentIndex].Unselect();
    }
}