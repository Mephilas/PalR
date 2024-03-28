/*using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 物品面板
/// </summary>
public sealed class ItemPanel_ : UIPanelBase
{
    /// <summary>
    /// 单列最大数量
    /// </summary>
    private const int MAX_VERTICAL_COUNT = 7;

    /// <summary>
    /// 物品集合
    /// </summary>
    private static UIItem[] _itemArray;

    /// <summary>
    /// 物品二维集合
    /// </summary>
    private static UIItem[][] _itemArray2D;

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
    private static ItemData _tempI;

    /// <summary>
    /// 选中坐标
    /// </summary>
    private static Vector2Int _selectV;

    private static Vector2Int _tempV;

    /// <summary>
    /// 单行最大数量
    /// </summary>
    private static int _maxHorizontalCount;

    protected override void Awake()
    {
        base.Awake();

        CGC(ref _itemArray);
        CGC(ref _itemI, "IconBG/Icon");
        CGC(ref _description, "Description");

        DownInputDic.Add(KeyCode.Escape, () => GameManager_.Trigger(ESCAPE_PANEL_EVENT));
        DownInputDic.Add(KeyCode.UpArrow, () => { _tempV.y = 0 == _tempV.y ? MAX_VERTICAL_COUNT - 1 : _tempV.y + 1; Select(); });
        DownInputDic.Add(KeyCode.DownArrow, () => { _tempV.y = MAX_VERTICAL_COUNT - 1 == _tempV.y ? 0 : _tempV.y - 1; Select(); });
        DownInputDic.Add(KeyCode.LeftArrow, () => { _tempV.x = 0 == _tempV.x ? _maxHorizontalCount - 1 : _tempV.x + 1; Select(); });
        DownInputDic.Add(KeyCode.RightArrow, () => { _tempV.x = _maxHorizontalCount - 1 == _tempV.x ? 0 : _tempV.x + 1; Select(); });

        _maxHorizontalCount = (int)(Screen.width / _itemArray[0].RectT.sizeDelta.x);
        ToolsE.Log("Max horizontal count : " + _maxHorizontalCount);
        _itemArray2D = new UIItem[_maxHorizontalCount][];
        for (int i = 0; i != _maxHorizontalCount; i++)
        {
            _itemArray2D[i] = new UIItem[MAX_VERTICAL_COUNT];

            for (int j = 0; j != MAX_VERTICAL_COUNT; j++)
            {
                _itemArray2D[i][j] = _itemArray[i * MAX_VERTICAL_COUNT + j];
            }
        }


        static void Select()
        {
            _itemArray2D[_selectV.x][_selectV.y].Unselect();
            _selectV = _tempV;
            _itemArray2D[_selectV.x][_selectV.y].Selected();
        }
    }

    public override void Active()
    {
        base.Active();

        for (int i = 0; i != _itemArray.Length; i++)
            _itemArray[i].Clean();

        if (0 == GameManager_.Bag.Count)
        {
            _itemI.sprite = null;
            _description.text = null;
        }
        else
        {
            for (int i = 0; i != GameManager_.Bag.Count; i++)
                _itemArray[i].Init(DataManager_.ItemDataArray[GameManager_.Bag[i]]);

            _tempV = _selectV = Vector2Int.zero;
            _tempI = _itemArray2D[_selectV.x][_selectV.y].Selected();
            _itemI.sprite = _tempI.Icon;
            _description.text = _tempI.Description;
        }
    }

    public override void Inactive(bool hide)
    {
        base.Inactive(hide);

        _itemArray2D[_selectV.x][_selectV.y].Unselect();
    }
}*/