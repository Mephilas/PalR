using UnityEngine.UI;

/// <summary>
/// 使用面板
/// </summary>
public sealed class UsePanel : UIPanelBase
{
    /// <summary>
    /// 文本路径集合
    /// </summary>
    private static readonly string[] _textPathArray = new string[] { "LevelValue", "HPValue", "HPMax", "MPValue", "MPMax", "AttackValue", "MagicValue", "DefenseValue" };

    /// <summary>
    /// 文本集合
    /// </summary>
    private static readonly Text[] _textArray = new Text[_textPathArray.Length];

    /// <summary>
    /// 选择器集合
    /// </summary>
    private static TextSelector[] _userArray;

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
        for (int i = 0; i != _textPathArray.Length; i++)
            _textArray[i] = CGX(_textPathArray[i]);

        CGC(ref _userArray);
    }

    protected override void Escape() => GameManager_.Trigger(ITEM_PANEL_EVENT);

    protected override void Enter() => _userArray[_currentIndex].Selected();

    protected override void Up()
    {
        if (-1 == --_currentIndex) _currentIndex = GameManager_.PlayerList.Last(); Select(_currentIndex);
    }

    protected override void Down()
    {
        if (GameManager_.PlayerList.Count == ++_currentIndex) _currentIndex = 0; Select(_currentIndex);
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
        _userArray[_lastIndex].Unselect();
        _userArray[_lastIndex = index].Select();
        StateDisplay(GameManager_.PlayerList[_lastIndex]);
    }

    public override void Active(string[] argumentArray = null)
    {
        base.Active();

        for (int i = 0; i != _userArray.Length; i++)
            _userArray[i].Clear();

        for (int i = 0; i != GameManager_.PlayerList.Count; i++)
        {
            int index = i;
            RoleData roleData = GameManager_.PlayerList[index].RoleData;
            _userArray[index].Init(() => Select(_currentIndex = index), () =>
            {
                GameManager_.Trigger(GameEventType.ItemUse, GameManager_.PlayerList[roleData.ID].RoleData.ID.ToString(), ItemPanel.SelectItem.ID.ToString());
                GameManager_.Trigger(ITEM_PANEL_EVENT);
            }, roleData.Name);
        }

        Select(_currentIndex = 0);
        StateDisplay(GameManager_.PlayerList[_lastIndex = _currentIndex = 0]);
    }

    /// <summary>
    /// 状态显示
    /// </summary>
    /// <param name="player">角色</param>
    private static void StateDisplay(Role player)
    {
        int index = 0;

        _textArray[index++].text = player.Level.ToString();
        _textArray[index++].text = player.HP.ToString();
        _textArray[index++].text = player.HPMax.ToString();
        _textArray[index++].text = player.MP.ToString();
        _textArray[index++].text = player.MPMax.ToString();
        _textArray[index++].text = player.Attack.ToString();
        _textArray[index++].text = player.Magic.ToString();
        _textArray[index++].text = player.Defense.ToString();
    }
}