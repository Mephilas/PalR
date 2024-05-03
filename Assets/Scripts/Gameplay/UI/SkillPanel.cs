using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 仙术面板
/// </summary>
public sealed class SkillPanel : UIPanelBase
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
    /// 角色集合
    /// </summary>
    private static PlayerProfile[] _playerArray;

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
    /// 应用角色
    /// </summary>
    private static Role _applyPlayer;

    /// <summary>
    /// 选中仙术
    /// </summary>
    private static SkillData _selectSkill;

    /// <summary>
    /// 上轮序号
    /// </summary>
    private static int _lastPlayerIndex;

    /// <summary>
    /// 当前序号
    /// </summary>
    private static int _currentPlayerIndex;

    /// <summary>
    /// 上轮序号
    /// </summary>
    private static int _lastSkillIndex;

    /// <summary>
    /// 当前序号
    /// </summary>
    private static int _currentSkillIndex;

    /// <summary>
    /// 玩家选中
    /// </summary>
    private static bool _isPlayerSelected;

    /// <summary>
    /// 仙术选中
    /// </summary>
    public static bool IsSkillSelected { get; private set; }

    /// <summary>
    /// 施放角色选择状态
    /// </summary>
    private static bool CastSelectState { get { return !_isPlayerSelected && !IsSkillSelected; } }

    /// <summary>
    /// 仙术选择状态
    /// </summary>
    private static bool SkillSelectState { get { return _isPlayerSelected && !IsSkillSelected; } }

    /// <summary>
    /// 使用角色选择状态
    /// </summary>
    private static bool ApplySelectState { get { return _isPlayerSelected && IsSkillSelected; } }

    /// <summary>
    /// 角色选择状态
    /// </summary>
    private static bool PlayerSelectState { get { return CastSelectState || ApplySelectState; } }
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
        CGC(ref _playerArray);
    }

    protected override void Escape()
    {
        if (CastSelectState)
        {
            GameManager_.Trigger(ESCAPE_PANEL_EVENT);
        }
        else if (SkillSelectState)
        {
            _isPlayerSelected = false;
            PlayerSelect(_currentPlayerIndex);
            _skillArray[_currentSkillIndex].Unselect();

            _costMP.text = _currentMP.text = _description.text = null;
        }
        else if (ApplySelectState)
        {
            IsSkillSelected = false;
            _playerArray[_currentPlayerIndex].Unselect();
            SkillSelect(_currentSkillIndex);
        }
    }

    protected override void Enter()
    {
        if (CastSelectState)
        {
            PlayerSelected();
        }
        else if (SkillSelectState)
        {
            SkillSelected();
        }
        else if (ApplySelectState)
        {
            IsSkillSelected = false;
            PlayerSelected();
            for (int i = 0; i != GameManager_.PlayerList.Count; i++)
                _playerArray[i].Refresh();
        }
    }

    protected override void Up()
    {
        if (PlayerSelectState)
        {
            if (0 != _currentPlayerIndex)
            {
                _currentPlayerIndex = 0;
                PlayerSelect(_currentPlayerIndex);
            }
        }
        else if (0 != _currentSkillIndex)
        {
            if ((_currentSkillIndex -= _verticalCount) < 0)
                _currentSkillIndex = 0;

            SkillSelect(_currentSkillIndex);
        }
    }

    protected override void Down()
    {
        if (PlayerSelectState)
        {
            if (GameManager_.PlayerList.Last() != _currentPlayerIndex)
            {
                _currentPlayerIndex = GameManager_.PlayerList.Last();
                PlayerSelect(_currentPlayerIndex);
            }
        }
        else if (_castPlayer.SkillList.Last() != _currentSkillIndex)
        {
            if (_castPlayer.SkillList.Last() < (_currentSkillIndex += _verticalCount))
                _currentSkillIndex = _castPlayer.SkillList.Last();

            SkillSelect(_currentSkillIndex);
        }
    }

    protected override void Left()
    {
        if (PlayerSelectState)
        {
            if (-1 == --_currentPlayerIndex)
                _currentPlayerIndex = GameManager_.PlayerList.Last();

            PlayerSelect(_currentPlayerIndex);
        }
        else
        {
            if (-1 == --_currentSkillIndex)
                _currentSkillIndex = _castPlayer.SkillList.Last();

            SkillSelect(_currentSkillIndex);
        }
    }

    protected override void Right()
    {
        if (PlayerSelectState)
        {
            if (GameManager_.PlayerList.Count == ++_currentPlayerIndex)
                _currentPlayerIndex = 0;

            PlayerSelect(_currentPlayerIndex);
        }
        else
        {
            if (_castPlayer.SkillList.Count == ++_currentSkillIndex)
                _currentSkillIndex = 0;

            SkillSelect(_currentSkillIndex);
        }
    }

    /// <summary>
    /// 玩家选择
    /// </summary>
    private static void PlayerSelect(int playerIndex)
    {
        if (_isPlayerSelected)
        {
            _applyPlayer = _playerArray[playerIndex].Select();
        }
        else
        {
            if (_lastPlayerIndex != _currentPlayerIndex) _playerArray[_lastPlayerIndex].Unselect();
            _castPlayer = _playerArray[_lastPlayerIndex = playerIndex].Select();

            for (int i = 0; i != _skillArray.Length; i++)
                _skillArray[i].Clear();

            for (int i = 0; i != _castPlayer.SkillList.Count; i++)
            {
                int index = i;
                SkillData skillData = DataManager_.SkillDataArray[_castPlayer.SkillList[index]];
                _skillArray[i].Init(() => SkillSelect(index), SkillSelected, skillData.Name, skillData.ID, index);
            }
        }
    }

    /// <summary>
    /// 玩家选中
    /// </summary>
    private static void PlayerSelected()
    {
        _playerArray[_currentPlayerIndex].Unselect();

        if (_isPlayerSelected)
        {
            GameManager_.Trigger(new(GameEventType.SkillCast,
                new string[] { _castPlayer.RoleData.ID.ToString(), _selectSkill.ID.ToString(), _applyPlayer.RoleData.ID.ToString() }));
        }
        else
        {
            _isPlayerSelected = true;
        }

        SkillSelect(_currentSkillIndex);
    }

    /// <summary>
    /// 仙术选择
    /// </summary>
    private static void SkillSelect(int index)
    {
        if (_lastSkillIndex != index) _skillArray[_lastSkillIndex].Unselect();
        _selectSkill = DataManager_.SkillDataArray[_skillArray[_lastSkillIndex = index].Select()];
        _costMP.text = _selectSkill.Cost.ToString();
        _currentMP.text = _castPlayer.MP.ToString();
        _description.text = _selectSkill.Description;
    }

    /// <summary>
    /// 仙术选中
    /// </summary>
    private static void SkillSelected()
    {
        if (_selectSkill.Cost <= _castPlayer.MP)
        {
            IsSkillSelected = true;

            _skillArray[_currentSkillIndex].Unselect(true);

            PlayerSelect(_currentPlayerIndex);
        }
    }

    public override void Active(string[] argumentArray = null)
    {
        base.Active();

        _verticalCount = (int)((_itemGridT.rect.width - _itemGrid.cellSize.x) / (_itemGrid.cellSize.x + _itemGrid.spacing.x) + 1);

        _costMP.text = _currentMP.text = _description.text = null;

        for (int i = 0; i != _skillArray.Length; i++)
            _skillArray[i].Clear();

        for (int i = 0; i != _playerArray.Length; i++)
            _playerArray[i].Hide();

        for (int i = 0; i != GameManager_.PlayerList.Count; i++)
        {
            int index = i;

            _playerArray[index].Init(() => PlayerSelect(index), PlayerSelected, GameManager_.PlayerList[index]);
        }

        PlayerSelect(_lastPlayerIndex = _currentPlayerIndex = 0);
    }

    public override void Inactive(bool hide)
    {
        base.Inactive(hide);

        _playerArray[_currentPlayerIndex].Hide();

        _isPlayerSelected = IsSkillSelected = false;
    }
}