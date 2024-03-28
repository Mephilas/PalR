using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine;

/// <summary>
/// 仙术界面玩家属性
/// </summary>
public sealed class SkillPlayer : UIBase_, IPointerEnterHandler
{
    /// <summary>
    /// 角色
    /// </summary>
    private Role _player;

    /// <summary>
    /// 迷你头像
    /// </summary>
    private Image _miniHead;

    /// <summary>
    /// 属性
    /// </summary>
    private Text _hp, _hpBase, _mp, _mpBase;

    /// <summary>
    /// 按钮
    /// </summary>
    private ButtonE _buttonE;

    /// <summary>
    /// 选择事件
    /// </summary>
    private UnityAction _select;

    /// <summary>
    /// 选中事件
    /// </summary>
    private UnityAction _selected;

    /// <summary>
    /// 被选中
    /// </summary>
    private bool _isSelected;

    protected override void Awake()
    {
        base.Awake();

        CGI(ref _miniHead, "Head");
        CGX(ref _hp, "HP");
        CGX(ref _hpBase, "HPMax");
        CGX(ref _mp, "MP");
        CGX(ref _mpBase, "MPMax");
        AC(ref _buttonE);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(UnityAction select, UnityAction selected, in Role player)
    {
        Display();
        MaskableGraphic.raycastTarget = true;

        _select = select;
        _buttonE.Init(_selected = selected);
        _player = player;
        _miniHead.sprite = _player.RoleData.ProfilePictureDic[ExpressionType.Normal];
        _hp.text = _player.HP.ToString();
        _hpBase.text = _player.HPMax.ToString();
        _mp.text = _player.MP.ToString();
        _mpBase.text = _player.MPMax.ToString();
    }

    /// <summary>
    /// 刷新
    /// </summary>
    public void Refresh()
    {
        _hp.text = _player.HP.ToString();
        _mp.text = _player.MP.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isSelected)
        {
            _isSelected = true;
            _select();
        }
    }

    /// <summary>
    /// 触发
    /// </summary>
    public void Trigger() => _selected();

    /// <summary>
    /// 选中
    /// </summary>
    /// <returns>选中角色</returns>
    public Role Select()
    {
        _isSelected = true;

        _miniHead.color = Color.white;

        return _player;
    }

    /// <summary>
    /// 取消选择
    /// </summary>
    public void Unselect()
    {
        _isSelected = false;

        _miniHead.color = Color.gray;
    }
}