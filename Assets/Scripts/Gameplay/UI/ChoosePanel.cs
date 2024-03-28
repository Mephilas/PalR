/// <summary>
/// 选择面板
/// </summary>
public sealed class ChoosePanel : UIPanelBase
{
    /// <summary>
    /// 选择面板事件
    /// </summary>
    private static readonly GameEventData CHOOSE_PANEL_EVENT = new(GameEventType.UIPanel, UIPanel.ChoosePanel.ToString());

    /// <summary>
    /// 选择数据
    /// </summary>
    private static ChooseData _chooseData;

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

    protected override void Awake()
    {
        base.Awake();

        CGC(ref _selectorArray);

        GameManager_.Register(GameEventType.Choose, Choose);
    }

    protected override void Enter() => _selectorArray[_currentIndex].Selected();

    protected override void Up()
    {
        if (0 != _currentIndex) Select(_currentIndex = 0);
    }

    protected override void Down()
    {
        if (_selectorArray.Last() != _currentIndex) Select(_currentIndex = _selectorArray.Last());
    }

    protected override void Left()
    {
        if (-1 == --_currentIndex) _currentIndex = _selectorArray.Last();
        Select(_currentIndex);
    }

    protected override void Right()
    {
        if (_selectorArray.Length == ++_currentIndex) _currentIndex = 0;
        Select(_currentIndex);
    }

    private static void Select(int index)
    {
        _selectorArray[_lastIndex].Unselect();
        _selectorArray[_lastIndex = index].Select();
    }

    /// <summary>
    /// 选择
    /// </summary>
    /// <param name="data">选择数据</param>
    private void Choose(string[] data)
    {
        GameManager_.Trigger(CHOOSE_PANEL_EVENT);

        _chooseData = DataManager_.ChooseDataArray[int.Parse(data[0])];
        _selectorArray[0].Init(() => Select(_currentIndex = 0), () => { GameManager_.Trigger(BASIC_PANEL_EVENT); GameManager_.TriggerAll(_chooseData.AcceptEventArray); });
        _selectorArray[1].Init(() => Select(_currentIndex = 1), () => { GameManager_.Trigger(BASIC_PANEL_EVENT); GameManager_.TriggerAll(_chooseData.RefuseEventArray); });
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