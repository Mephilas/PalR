/// <summary>
/// 主界面
/// </summary>
public sealed class MainPanel : UIPanelBase
{
    /// <summary>
    /// 主界面BG
    /// </summary>
    private static readonly GameEventData MAIN_BG = new(GameEventType.BGPlay, "17");

    /// <summary>
    /// 选择器集合
    /// </summary>
    private static TextSelector[] _selectorArray;

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
    }

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

        _selectorArray[0].Init(() => Select(_currentIndex = 0), GameManager_.NewGame);
        _selectorArray[1].Init(() => Select(_currentIndex = 1), () => GameManager_.Trigger(SL_PANEL_EVENT));
    }

    public override void Active(string[] argumentArray = null)
    {
        base.Active();

        GameManager_.Trigger(MAIN_BG);
        _selectorArray[_currentIndex].Select();
        GameManager_.InGame = false;
    }

    public override void Inactive(bool hide)
    {
        base.Inactive(hide);

        if (hide)
        {
            _selectorArray[_currentIndex].Unselect(!hide);
            GameManager_.InGame = true;
        }
    }
}