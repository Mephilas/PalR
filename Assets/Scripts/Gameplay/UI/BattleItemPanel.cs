using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 战斗物品面板
/// </summary>
public sealed class BattleItemPanel : UIPanelBase
{
    /// <summary>
    /// 最大行数
    /// </summary>
    private const int HORIZONTAL_COUINT = 7;

    /// <summary>
    /// 翻页行号
    /// </summary>
    private const int TURN_LINE = (int)(HORIZONTAL_COUINT * 0.5f);

    /// <summary>
    /// 自适应最大列数
    /// </summary>
    private static int _verticalCount;

    /// <summary>
    /// 单行垂直高度
    /// </summary>
    private static int _verticalHeight;

    /// <summary>
    /// 物品Rect
    /// </summary>
    private static RectTransform _itemGridT; 

    /// <summary>
    /// 选择器集合
    /// </summary>
    private static TextSelector[] _selectorArray;

    /// <summary>
    /// 物品矩阵
    /// </summary>
    private static GridLayoutGroup _itemGrid;

    /// <summary>
    /// 物品图标
    /// </summary>
    private static Image _itemI;

    /// <summary>
    /// 描述
    /// </summary>
    private static Text _description;

    /// <summary>
    /// 选中物品
    /// </summary>
    public static ItemData SelectItem { get; set; }

    /// <summary>
    /// 当前行号
    /// </summary>
    private static int CurrentLineIndex { get { return _currentIndex / _verticalCount; } }

    /// <summary>
    /// 当前首行行号
    /// </summary>
    private static int _topLineIndex;

    /// <summary>
    /// 当前尾行行号
    /// </summary>
    private static int BottomLineIndex { get { return _topLineIndex + HORIZONTAL_COUINT - 1; } }

    /// <summary>
    /// 翻页行数
    /// </summary>
    private static int _turnCount;

    /// <summary>
    /// 当前序号
    /// </summary>
    private static int _currentIndex = -1;

    protected override void Awake()
    {
        base.Awake();

        AC<ButtonE>().Init(Escape);
        _itemGrid = CGC<GridLayoutGroup>("ItemMask/ItemGrid");
        _verticalHeight = (int)(_itemGrid.cellSize.y + _itemGrid.spacing.y);
        _itemGridT = _itemGrid.GetComponent<RectTransform>();
        CGC(ref _selectorArray);
        CGC(ref _itemI, "IconBG/Icon");
        CGC(ref _description, "Description");
    }

    protected override void Escape() => GameManager_.Trigger(new(GameEventType.UIPanel, UIPanel.BattleExtensionPanel.ToString()));

    protected override void Enter() => _selectorArray[_currentIndex].Selected();

    protected override void Up() => Select(_currentIndex < _verticalCount ? 0 : _currentIndex - _verticalCount);

    protected override void Down() => Select(GameManager_.Bag.Last() < _currentIndex + _verticalCount ? GameManager_.Bag.Last() : _currentIndex + _verticalCount);

    protected override void Left() => Select(_currentIndex - 1);

    protected override void Right() => Select(_currentIndex + 1);

    private static void Select(in int index)
    {
        if (index == _currentIndex) return;

        if (_selectorArray.Valid(_currentIndex)) _selectorArray[_currentIndex].Unselect();
        SelectItem = DataManager_.ItemDataArray[_selectorArray[_currentIndex = index].Select()];
        _itemI.sprite = SelectItem.Icon;
        _description.text = SelectItem.Description;

        if (TURN_LINE <= ((float)GameManager_.Bag.Count / _verticalCount).Ceil() - 1 - CurrentLineIndex && TURN_LINE < CurrentLineIndex - _topLineIndex)
        {
            if (GameManager_.Bag.Count / _verticalCount - CurrentLineIndex < TURN_LINE)
            {
                _turnCount = GameManager_.Bag.Count / _verticalCount - CurrentLineIndex;
            }
            else _turnCount = CurrentLineIndex - _topLineIndex - TURN_LINE;

            _itemGridT.localPosition = _itemGridT.localPosition.V3ModifyYAdd(_verticalHeight * _turnCount);
            _topLineIndex += _turnCount;
        }
        else if (TURN_LINE <= CurrentLineIndex && TURN_LINE < BottomLineIndex - CurrentLineIndex)
        {
            if (CurrentLineIndex < TURN_LINE)
            {
                _turnCount = CurrentLineIndex;
            }
            else _turnCount = -(BottomLineIndex - CurrentLineIndex - TURN_LINE);

            _itemGridT.localPosition = _itemGridT.localPosition.V3ModifyYAdd(_verticalHeight * _turnCount);
            _topLineIndex += _turnCount;
        }
    }

    public override void Active(string[] argumentArray = null)
    {
        base.Active();

        _verticalCount = (int)((_itemGridT.rect.width - _itemGrid.cellSize.x) / (_itemGrid.cellSize.x + _itemGrid.spacing.x) + 1);

        for (int i = 0; i != _selectorArray.Length; i++)
            _selectorArray[i].Clear();

        if (0 == GameManager_.Bag.Count)
        {
            _itemI.sprite = null;
            _description.text = null;
        }
        else
        {
            System.Collections.Generic.Dictionary<int, int>.Enumerator enumerator = GameManager_.Bag.GetEnumerator();
            for (int i = 0; i != GameManager_.Bag.Count; i++)
            {
                enumerator.MoveNext();

                int itemID = enumerator.Current.Key, index = i;
                ItemData item = DataManager_.ItemDataArray[itemID];
                _selectorArray[index].Init(() => Select(index), () =>
                {
                    if (SelectItem.Throw || -1 != SelectItem.SellPrice && !SelectItem.Outfit)
                    {
                        GameManager_.Trigger(new(GameEventType.UIPanel, new string[] { UIPanel.BattlePanel.ToString(), "True", SelectItem.ID.ToString(), "False" }));
                    }
                    else
                    {
                        //GameManager_.Trigger(new(GameEventType.UIPanel, UIPanel.BattleExtensionPanel.ToString()));
                        GameManager_.Trigger(new(GameEventType.Tip, "46"));
                    }
                }, item.Name, item.ID, index, GameManager_.Bag[itemID]);
            }

            _itemGridT.localPosition = _itemGridT.localPosition.V3ModifyY(0);
            Select(_topLineIndex = 0);
        }
    }

    public override void Inactive(bool hide)
    {
        base.Inactive(hide);

        _currentIndex = -1;
    }
}