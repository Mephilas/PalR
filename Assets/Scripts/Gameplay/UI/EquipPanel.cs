using UnityEngine.UI;

/// <summary>
/// 装备面板
/// </summary>
public sealed class EquipPanel : UIPanelBase
{
    /// <summary>
    /// 物品图标
    /// </summary>
    private static Image _itemI;

    /// <summary>
    /// 物品名称
    /// </summary>
    private static Text _itemT;

    /// <summary>
    /// 使用者集合
    /// </summary>
    private static TextSelector[] _selectorArray;

    /// <summary>
    /// 描述
    /// </summary>
    private static Text _description;

    /// <summary>
    /// 文本路径集合
    /// </summary>
    private static readonly string[] _textPathArray = new string[] { "CasqueName", "CapeName", "ArmorName", "WeaponName", "BootsName", "BracersName", "AttackValue", "MagicValue", "DefenseValue", "SpeedValue", "LuckValue" };

    /// <summary>
    /// 文本集合
    /// </summary>
    private static readonly Text[] _textArray = new Text[_textPathArray.Length];

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
        CGC(ref _itemI, "EquipIcon");
        CGC(ref _itemT, "EquipName");
        CGC(ref _selectorArray);
        CGC(ref _description, "Description");

        for (int i = 0; i != _textPathArray.Length; i++)
            _textArray[i] = CGX(_textPathArray[i]);
    }

    protected override void Escape() => GameManager_.Trigger(ITEM_PANEL_EVENT);

    protected override void Enter() => _selectorArray[_currentIndex].Selected();

    protected override void Up()
    {
        if (0 != _currentIndex && -1 == --_currentIndex) _currentIndex = GameManager_.PlayerList.Last();
        Select(_currentIndex);
    }

    protected override void Down()
    {
        if (GameManager_.PlayerList.Last() != _currentIndex && GameManager_.PlayerList.Count == ++_currentIndex) _currentIndex = 0;
        Select(_currentIndex);
    }

    protected override void Left()
    {
        if (0 != _currentIndex) Select(_currentIndex = 0);
    }

    protected override void Right()
    {
        if (GameManager_.PlayerList.Last() != _currentIndex) Select(_currentIndex = GameManager_.PlayerList.Last());
    }

    private static void Select(int index)
    {
        _selectorArray[_lastIndex].Unselect();
        _selectorArray[_lastIndex = index].Select();
    }

    public override void Active(string[] argumentArray = null)
    {
        base.Active();

        _itemI.sprite = ItemPanel.SelectItem.Icon;
        _itemT.text = ItemPanel.SelectItem.Name;
        _description.text = ItemPanel.SelectItem.Description;

        for (int i = 0; i != _selectorArray.Length; i++)
            _selectorArray[i].Clear();

        for (int i = 0; i != GameManager_.PlayerList.Count; i++)
        {
            int index = i;
            RoleData roleData = GameManager_.PlayerList[index].RoleData;
            _selectorArray[index].Init(() => Select(_currentIndex = index), () =>
            {
                GameManager_.Trigger(GameEventType.ItemEquip, GameManager_.PlayerList[roleData.ID].RoleData.ID.ToString(), ItemPanel.SelectItem.ID.ToString());
                Active();
            }, roleData.Name);
        }

        Select(_currentIndex = 0);
        StateDisplay(GameManager_.PlayerList[_lastIndex = _currentIndex = 0] as Player);
    }

    /// <summary>
    /// 状态显示
    /// </summary>
    /// <param name="player">角色</param>
    private static void StateDisplay(Player player)
    {
        int index = 0;

        for (int i = 0; i != (int)OutfitType.Bracers + 1; i++)
            _textArray[index++].text = DataManager_.ItemDataArray[player.OutfitDic[(OutfitType)i]].Name;

        _textArray[index++].text = player.Attack.ToString();
        _textArray[index++].text = player.Magic.ToString();
        _textArray[index++].text = player.Defense.ToString();
        _textArray[index++].text = player.Speed.ToString();
        _textArray[index++].text = player.Luck.ToString();
    }
}