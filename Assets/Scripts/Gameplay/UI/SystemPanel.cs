using UnityEngine;

/// <summary>
/// 系统面板
/// </summary>
public sealed class SystemPanel : UIPanelBase
{
    /// <summary>
    /// 选择事件集合
    /// </summary>
    private static readonly UnityEngine.Events.UnityAction[] SELECTOR_EVENT_ARRAY = new UnityEngine.Events.UnityAction[]
    {
        () => { SLPanel.SaveOrLoad = true; GameManager_.Trigger(SL_PANEL_EVENT); },
        () => { SLPanel.SaveOrLoad = false; GameManager_.Trigger(SL_PANEL_EVENT); },
        () => { },
        () => { },
        Application.Quit
    };

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

    private static int _width, _height;

    protected override void Awake()
    {
        base.Awake();

        AC<ButtonE>().Init(Escape);
        _width = int.Parse(Screen.resolutions[^1].ToString()[..4]);
        _height = int.Parse(Screen.resolutions[^1].ToString().Substring(7, 4));
        CGC<ButtonE>("Classic").Init(() => Screen.SetResolution(_height / 3 * 4, _height, true));
        CGC<ButtonE>("Modern").Init(() => Screen.SetResolution(_width, _height, true));
        CGC(ref _selectorArray);
    }

    protected override void Escape() => GameManager_.Trigger(ESCAPE_PANEL_EVENT);

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

            _selectorArray[i].Init(() => Select(_currentIndex = index), SELECTOR_EVENT_ARRAY[index]);
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

        if (hide) _selectorArray[_currentIndex].Unselect(!hide);
    }
}