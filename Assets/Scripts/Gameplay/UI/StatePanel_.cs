/*using UnityEngine;
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
    private static readonly Image[] _imageArray = new Image[_imagePathArray.Length];  // { _profilePictureI, _casqueI, _capeI, _armorI, _weaponI, _bootsI, _bracersI };

    /// <summary>
    /// 图标
    /// </summary>
    //private static Image _profilePictureI, _casqueI, _capeI, _armorI, _weaponI, _bootsI, _bracersI;

    /// <summary>
    /// 文本路径集合
    /// </summary>
    private static readonly string[] _textPathArray = new string[] { "ExperienceValue", "ExperienceRequire", "LevelValue", "HPValue", "HPMax", "MPValue", "MPMax", "AttackValue", "MagicValue", "DefenseValue", "SpeedValue", "LuckValue", "Name", "CasqueName", "CapeName", "ArmorName", "WeaponName", "BootsName", "BracersName" };

    /// <summary>
    /// 文本集合
    /// </summary>
    private static readonly Text[] _textArray = new Text[_textPathArray.Length];  // { _experienceT, _experienceRequireT, _levelT, _hpT, _hpBaseT, _mpT, _mpBaseT, _attackT, _magicT, _defenseT, _speedT, _luckT, _nameT, _casqueT, _capeT, _armorT, _weaponT, _bootsT, _bracersT };

    /// <summary>
    /// 文本
    /// </summary>
    //private static Text _experienceT, _experienceRequireT, _levelT, _hpT, _hpBaseT, _mpT, _mpBaseT, _attackT, _magicT, _defenseT, _speedT, _luckT, _nameT, _casqueT, _capeT, _armorT, _weaponT, _bootsT, _bracersT;

    /// <summary>
    /// 选中序号
    /// </summary>
    private static int _selectIndex;

    protected override void Awake()
    {
        base.Awake();

        DownInputDic.Add(KeyCode.Escape, () => GameManager_.Trigger(ESCAPE_PANEL_EVENT));

        for (int i = 0; i != _imagePathArray.Length; i++)
            _imageArray[i] = CGI(_imagePathArray[i]);

        for (int i = 0; i != _textPathArray.Length; i++)
            _textArray[i] = CGX(_textPathArray[i]);
    }

    public override void Active()
    {
        base.Active();

        StateDisplay(GameManager_.PlayerArray[_selectIndex = 0]);
    }

    /// <summary>
    /// 状态显示
    /// </summary>
    /// <param name="player">角色</param>
    private static void StateDisplay(Role player)
    {
        _imageArray[0].sprite = player.RoleData.ProfilePictureDic[ExpressionType.Normal];
        for (int i = 1; i != _imagePathArray.Length; i++)
            _imageArray[i].sprite = DataManager_.ItemDataArray[player.OutfitDic[_imagePathArray[i].S2E<OutfitType>()]].Icon;

        int index = 0;
        _textArray[index++].text = player.Experience.ToString();
        _textArray[index++].text = RoleData.EXPERIENCE_REQUIRE_ARRAY[player.Level].ToString();
        _textArray[index++].text = player.Level.ToString();
        _textArray[index++].text = player.HP.ToString();
        _textArray[index++].text = player.HPMax.ToString();
        _textArray[index++].text = player.MP.ToString();
        _textArray[index++].text = player.MPMax.ToString();
        _textArray[index++].text = player.Attack.ToString();
        _textArray[index++].text = player.Magic.ToString();
        _textArray[index++].text = player.Defense.ToString();
        _textArray[index++].text = player.Speed.ToString();
        _textArray[index++].text = player.Luck.ToString();
        _textArray[index++].text = player.RoleData.Name;

        for (int i = 0; i != (int)OutfitType.Bracers + 1; i++)
        {
            _textArray[index++].text = DataManager_.ItemDataArray[player.OutfitDic[(OutfitType)i]].Name;
        }


        _experienceT.text = player.Experience.ToString();
        _experienceRequireT.text = player.RoleData.ExperienceRequire.ToString();
        _level.text = player.Level.ToString();
        _hpT.text = player.HP.ToString();
        _hpBaseT.text = player.HPMax.ToString();
        _mpT.text = player.MP.ToString();
        _mpBaseT.text = player.MPMax.ToString();
        _attackT.text = player.Attack.ToString();
        _magicT.text = player.Magic.ToString();
        _defenseT.text = player.Defense.ToString();
        _speedT.text = player.Speed.ToString();
        _luckT.text = player.Luck.ToString();
        _nameT.text = player.RoleData.Name;
        _casqueT.text = player.OutfitDic[OutfitType.Casque].ToString();
        _capeT.text = player.OutfitDic[OutfitType.Cape].ToString();
        _armorT.text = player.OutfitDic[OutfitType.Armor].ToString();
        _weaponT.text = player.OutfitDic[OutfitType.Weapon].ToString();
        _bootsT.text = player.OutfitDic[OutfitType.Boots].ToString();
        _bracersT.text = player.OutfitDic[OutfitType.Bracers].ToString();
    }
}
*/