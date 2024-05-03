using UnityEngine.UI;

/// <summary>
/// 状态面板
/// </summary>
public sealed class StatePanel : UIPanelBase
{
    /// <summary>
    /// 图标路径集合
    /// </summary>
    private static readonly string[] _imagePathArray = new string[] { "ProfilePicture", "Casque", "Cape", "Armor", "Weapon", "Boots", "Bracers" };

    /// <summary>
    /// 图标集合
    /// </summary>
    private static readonly Image[] _imageArray = new Image[_imagePathArray.Length];

    /// <summary>
    /// 文本路径集合
    /// </summary>
    private static readonly string[] _textPathArray = new string[] { "ExperienceValue", "ExperienceRequire", "LevelValue", "HPValue", "HPMax", "MPValue", "MPMax", "AttackValue", "MagicValue", "DefenseValue", "SpeedValue", "LuckValue", "Name", "CasqueName", "CapeName", "ArmorName", "WeaponName", "BootsName", "BracersName" };

    /// <summary>
    /// 文本集合
    /// </summary>
    private static readonly Text[] _textArray = new Text[_textPathArray.Length];

    /// <summary>
    /// 选中序号
    /// </summary>
    private static int _selectIndex;

    /// <summary>
    /// 选中角色
    /// </summary>
    private static Role _selectPlayer;

    protected override void Awake()
    {
        base.Awake();

        AC<ButtonE>().Init(Escape);

        for (int i = 0; i != _imagePathArray.Length; i++)
            _imageArray[i] = CGI(_imagePathArray[i]);

        for (int i = 0; i != _textPathArray.Length; i++)
            _textArray[i] = CGX(_textPathArray[i]);
    }

    protected override void Escape() => Enter();

    protected override void Enter() => GameManager_.Trigger(ESCAPE_PANEL_EVENT);

    protected override void Up()
    {
        if (0 != _selectIndex) Select(_selectIndex = 0);
    }

    protected override void Down()
    {
        if (GameManager_.PlayerList.Last() != _selectIndex) Select(_selectIndex = GameManager_.PlayerList.Last());
    }

    protected override void Left()
    {
        if (-1 == --_selectIndex) _selectIndex = GameManager_.PlayerList.Last();
        Select(_selectIndex);
    }

    protected override void Right()
    {
        if (GameManager_.PlayerList.Count == ++_selectIndex) _selectIndex = 0;
        Select(_selectIndex);
    }

    public override void Active(string[] argumentArray = null)
    {
        base.Active();

        Select(_selectIndex = 0);
    }

    /// <summary>
    /// 选择
    /// </summary>
    /// <param name="playerIndex">玩家序号</param>
    private static void Select(int playerIndex)
    {
        _selectPlayer = GameManager_.PlayerList[playerIndex];

        _imageArray[0].sprite = _selectPlayer.RoleData.ProfilePictureDic[ExpressionType.Normal];
        for (int i = 1; i != _imagePathArray.Length; i++)
            _imageArray[i].sprite = DataManager_.ItemDataArray[_selectPlayer.OutfitDic[_imagePathArray[i].S2E<OutfitType>()]].Icon;

        int index = 0;
        _textArray[index++].text = _selectPlayer.Experience.ToString();
        _textArray[index++].text = RoleData.EXPERIENCE_REQUIRE_ARRAY[_selectPlayer.Level].ToString();
        _textArray[index++].text = _selectPlayer.Level.ToString();
        _textArray[index++].text = _selectPlayer.HP.ToString();
        _textArray[index++].text = _selectPlayer.HPMax.ToString();
        _textArray[index++].text = _selectPlayer.MP.ToString();
        _textArray[index++].text = _selectPlayer.MPMax.ToString();
        _textArray[index++].text = _selectPlayer.Attack.ToString();
        _textArray[index++].text = _selectPlayer.Magic.ToString();
        _textArray[index++].text = _selectPlayer.Defense.ToString();
        _textArray[index++].text = _selectPlayer.Speed.ToString();
        _textArray[index++].text = _selectPlayer.Luck.ToString();
        _textArray[index++].text = _selectPlayer.RoleData.Name;

        for (int i = 0; i != (int)OutfitType.Bracers + 1; i++)
            _textArray[index++].text = DataManager_.ItemDataArray[_selectPlayer.OutfitDic[(OutfitType)i]].Name;
    }
}