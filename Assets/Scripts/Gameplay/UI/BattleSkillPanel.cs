using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 仙术面板
/// </summary>
public sealed class BattleSkillPanel : UIPanelBase
{
    #region
    /// <summary>
    /// 消耗灵力
    /// </summary>
    private static Text _costMP;

    /// <summary>
    /// 当前灵力
    /// </summary>
    private static Text _currentMP;

    /// <summary>
    /// 描述
    /// </summary>
    private static Text _description;

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
    /// 技能集合
    /// </summary>
    private static TextSelector[] _skillArray;

    /// <summary>
    /// 物品矩阵
    /// </summary>
    private static GridLayoutGroup _itemGrid;

    /// <summary>
    /// 施放角色
    /// </summary>
    private static Role _castPlayer;

    /// <summary>
    /// 选中仙术
    /// </summary>
    private static SkillData _selectSkill;

    /// <summary>
    /// 当前序号
    /// </summary>
    private static int _currentIndex = -1;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        AC<ButtonE>().Init(Escape);
        _itemGrid = CGC<GridLayoutGroup>("SkillGrid");
        _verticalHeight = (int)(_itemGrid.cellSize.y + _itemGrid.spacing.y);
        _itemGridT = _itemGrid.GetComponent<RectTransform>();
        CGC(ref _costMP, "MPBG/CostMP");
        CGC(ref _currentMP, "MPBG/CurrentMP");
        CGC(ref _description, "Description");
        CGC(ref _skillArray);
    }

    protected override void Escape() => GameManager_.Trigger(BATTLE_PANEL_EVENT);

    protected override void Enter() => SkillSelected();

    protected override void Up() => SkillSelect(0);

    protected override void Down() => SkillSelect(_castPlayer.SkillList.Last());

    protected override void Left() => SkillSelect(_currentIndex - 1);

    protected override void Right() => SkillSelect(_currentIndex + 1);

    /// <summary>
    /// 仙术选择
    /// </summary>
    private static void SkillSelect(in int index)
    {
        if (index != _currentIndex && _castPlayer.SkillList.Valid(index))
        {
            if (_skillArray.Valid(_currentIndex)) _skillArray[_currentIndex].Unselect();
            _selectSkill = DataManager_.SkillDataArray[_skillArray[_currentIndex = index].Select()];
            _costMP.text = _selectSkill.Cost.ToString();
            _currentMP.text = _castPlayer.MP.ToString();
            _description.text = _selectSkill.Description;
        }
    }

    /// <summary>
    /// 仙术选中
    /// </summary>
    private static void SkillSelected()
    {
        if (_selectSkill.Cost <= _castPlayer.MP)
            GameManager_.Trigger(new(GameEventType.UIPanel, new string[] { UIPanel.BattlePanel.ToString(), "True", _selectSkill.ID.ToString(), "True" }));
    }

    public override void Active(string[] argumentArray = null)
    {
        base.Active();

        _verticalCount = (int)((_itemGridT.rect.width - _itemGrid.cellSize.x) / (_itemGrid.cellSize.x + _itemGrid.spacing.x) + 1);

        _costMP.text = _currentMP.text = _description.text = null;

        for (int i = 0; i != _skillArray.Length; i++)
            _skillArray[i].Clear();

        _castPlayer = BattleField.PlayerList[int.Parse(argumentArray[2])];
        for (int i = 0; i != _castPlayer.SkillList.Count; i++)
        {
            int index = i;
            SkillData skillData = DataManager_.SkillDataArray[_castPlayer.SkillList[index]];
            _skillArray[i].Init(() => SkillSelect(index), SkillSelected, skillData.Name, skillData.ID, index);
        }

        SkillSelect(0);
    }

    public override void Inactive(bool hide)
    {
        base.Inactive(hide);

        _currentIndex = -1;
    }
}