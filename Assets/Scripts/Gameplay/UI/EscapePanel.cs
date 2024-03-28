using UnityEngine;

/// <summary>
/// Escape面板
/// </summary>
public sealed class EscapePanel : UIPanelBase
{
    /// <summary>
    /// 金钱Text
    /// </summary>
    private static UnityEngine.UI.Text _copperT;

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

        AC<ButtonE>().Init(Escape);

        CGX(ref _copperT, "CopperBG/Value");
        CGC(ref _selectorArray);

        GameManager_.CopperTUpdate += CopperTUpdate;
    }

    protected override void Escape() => GameManager_.Trigger(BASIC_PANEL_EVENT);

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

    protected override void Start()
    {
        base.Start();

        for (int i = 0; i != _selectorArray.Length; i++)
        {
            int index = i;

            _selectorArray[i].Init(() => Select(_currentIndex = index), () => GameManager_.Trigger(new(GameEventType.UIPanel, new string[] { _selectorArray[index].name, "False" })));
        }
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.Alpha4))
                GameManager_.Trigger(new(GameEventType.CopperAdd, "500"));
        }
    }

    private static void Select(int index)
    {
        _selectorArray[_lastIndex].Unselect();
        _selectorArray[_lastIndex = index].Select();
    }

    public override void Active()
    {
        base.Active();

        CopperTUpdate(GameManager_.Copper);

        _selectorArray[_currentIndex].Select();
    }

    public override void Inactive(bool hide)
    {
        base.Inactive(hide);

        _selectorArray[_currentIndex].Unselect(!hide);
    }

    private static void CopperTUpdate(int value) => _copperT.text = value.ToString();
}