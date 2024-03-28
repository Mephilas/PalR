using UnityEngine.UI;

/// <summary>
/// 交易面板
/// </summary>
public sealed class TradePanel : UIPanelBase
{
    /// <summary>
    /// 选择器集合
    /// </summary>
    private static Selector[] _selectorArray;

    /// <summary>
    /// 物品图标
    /// </summary>
    private static Image _itemI;

    /// <summary>
    /// 描述
    /// </summary>
    private static Text _description;

    /// <summary>
    /// 物品计数
    /// </summary>
    private static Text _countT;

    /// <summary>
    /// 金钱计数
    /// </summary>
    private static Text _copperT;

    /// <summary>
    /// 选中物品
    /// </summary>
    public static ItemData SelectItem { get; private set; }

    /// <summary>
    /// 当前序号
    /// </summary>
    private static int _currentIndex;

    /// <summary>
    /// 上轮序号
    /// </summary>
    private static int _lastIndex;

    protected override void Awake()
    {
        base.Awake();

        AC<ButtonE>().Init(Escape);
        CGC(ref _selectorArray);
        CGC(ref _itemI, "IconBG/Icon");
        CGC(ref _description, "Description");
        CGC(ref _countT, "CountBG/Count");
        CGC(ref _copperT, "CopperBG/Count");

        GameManager_.CopperTUpdate += CopperTUpdate;
    }

    protected override void Escape() => GameManager_.Trigger(BASIC_PANEL_EVENT);

    protected override void Enter() => _selectorArray[_currentIndex].Selected();

    protected override void Up()
    {
        if (-1 == --_currentIndex) _currentIndex = Player.SellItem.Last();
        Select(_currentIndex);
    }

    protected override void Down()
    {
        if (Player.SellItem.Length == ++_currentIndex) _currentIndex = 0;
        Select(_currentIndex);
    }

    protected override void Left()
    {
        if (0 != _currentIndex) Select(_currentIndex = 0);
    }

    protected override void Right()
    {
        if (Player.SellItem.Last() != _currentIndex) Select(_currentIndex = Player.SellItem.Last());
    }

    private static void Select(int index)
    {
        _selectorArray[_lastIndex].Unselect();
        SelectItem = DataManager_.ItemDataArray[_selectorArray[_lastIndex = index].Select()];
        _itemI.sprite = SelectItem.Icon;
        _description.text = SelectItem.Description;
        _countT.text = GameManager_.Bag.ContainsKey(SelectItem.ID) ? GameManager_.Bag[SelectItem.ID].ToString() : "0";
    }

    public override void Active()
    {
        base.Active();

        for (int i = 0; i != _selectorArray.Length; i++)
            _selectorArray[i].Clear();

        for (int i = 0; i != Player.SellItem.Length; i++)
        {
            int index = i;
            ItemData item = DataManager_.ItemDataArray[Player.SellItem[index]];
            _selectorArray[index].Init(() => Select(_currentIndex = index), () =>
            {
                if (GameManager_.CopperAdd(-item.BoughtPrice))
                    GameManager_.Trigger(new(GameEventType.ItemAdd, item.ID.ToString()));
            }, item.Name, item.ID, index, item.BoughtPrice);
        }

        Select(_currentIndex = 0);

        _copperT.text = GameManager_.Copper.ToString();
    }

    public override void Inactive(bool hide)
    {
        base.Inactive(hide);

        _selectorArray[_currentIndex].Clear();
    }

    private static void CopperTUpdate(int value) => _copperT.text = value.ToString();
}