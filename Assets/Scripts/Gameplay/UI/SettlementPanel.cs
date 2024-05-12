using UnityEngine;
using DG.Tweening;

/// <summary>
/// 结算面板
/// </summary>
public sealed class SettlementPanel : UIPanelBase
{
    /// <summary>
    /// 增加文本
    /// </summary>
    private const string EXPERIENCE_ADD = "经验增加：",
                         ITEM_ADD = "道具获得：";

    /// <summary>
    /// 经验、物品获得BG
    /// </summary>
    private static RectTransform _experienceBG, _itemBG;

    /// <summary>
    /// 经验、物品获得文字
    /// </summary>
    private static UnityEngine.UI.Text _experienceT, _itemT;

    /// <summary>
    /// 经验值
    /// </summary>
    private static int _experience;

    /// <summary>
    /// 战利品集合
    /// </summary>
    private static readonly System.Collections.Generic.Dictionary<int, int> _rewardDic = new();

    private static readonly System.Text.StringBuilder _stringB = new();

    protected override void Awake()
    {
        base.Awake();

        CGC(ref _experienceBG, "ExperienceBG");
        CGC(ref _itemBG, "ItemBG");
        CGC(ref _experienceT, "ExperienceBG/Text");
        CGC(ref _itemT, "ItemBG/Text");
    }

    /// <summary>
    /// 返回
    /// </summary>
    protected override void Escape() => BattleField.Instance.BattleEnd();

    /// <summary>
    /// 进入
    /// </summary>
    protected override void Enter() => BattleField.Instance.BattleEnd();

    private static int[] _beatItem;

    public override void Active(string[] argumentArray = null)
    {
        base.Active(argumentArray);

        for (int i = 0; i != BattleField.BeatHostileList.Count; i++)
        {
            _experience += BattleField.BeatHostileList[i].RoleData.BeatExperience;

            if (null != (_beatItem = BattleField.BeatHostileList[i].RoleData.BeatItemArray))
            {
                if (_rewardDic.ContainsKey(_beatItem[0]))
                {
                    _rewardDic[_beatItem[0]] += _beatItem[1];
                }
                else
                {
                    _rewardDic.Add(_beatItem[0], _beatItem[1]);
                }
            }
        }

        for (int i = 0; i != GameManager_.PlayerList.Count; i++)
        {
            GameManager_.PlayerList[i].BattleDataWriteBack(BattleField.PlayerList[i]);
            GameManager_.PlayerList[i].Trigger(RoleEffectType.ExperienceAdd, _experience);
        }

        foreach (int itemID in _rewardDic.Keys)
        {
            _stringB.Append(-1 == itemID ? "金钱" : DataManager_.ItemDataArray[itemID].Name);
            _stringB.Append("x");
            _stringB.Append(_rewardDic[itemID]);
            _stringB.Append(" ");

            if (-1 == _beatItem[0])
                GameManager_.Trigger(GameEventType.CopperAdd, _beatItem[1].ToString());
            else GameManager_.Trigger(GameEventType.ItemAdd, itemID.ToString(), _rewardDic[itemID].ToString());
        }

        _experienceT.text = EXPERIENCE_ADD + _experience;
        _experienceBG.DOScale(Vector2.one, 0.5f);

        if (0 != _stringB.Length)
        {
            _itemT.text = ITEM_ADD + _stringB.Remove(_stringB.Length - 1, 1).ToString();
            _itemBG.DOScale(Vector2.one, 0.5f);
        }
    }

    public override void Inactive(bool hide)
    {
        base.Inactive(hide);

        _experienceBG.localScale = _itemBG.localScale = Vector2.zero;

        _experience = 0;
        _rewardDic.Clear();
        _stringB.Clear();
    }
}