using UnityEngine.Events;

/// <summary>
/// 玩家简介
/// </summary>
public class PlayerProfile : UIBase_
{
    /// <summary>
    /// 角色
    /// </summary>
    protected Role _player;

    /// <summary>
    /// 迷你头像
    /// </summary>
    protected UnityEngine.UI.Image _miniHead;

    /// <summary>
    /// 属性
    /// </summary>
    private UnityEngine.UI.Text _hp, _hpBase, _mp, _mpBase;

    /// <summary>
    /// 按钮
    /// </summary>
    private ButtonE _buttonE;

    /// <summary>
    /// 选择事件
    /// </summary>
    protected UnityAction _select;

    /// <summary>
    /// 被选中
    /// </summary>
    protected bool _isSelected;

    protected override void Awake()
    {
        base.Awake();

        CGC(ref _miniHead, "Head");
        CGC(ref _hp, "HP");
        CGC(ref _hpBase, "HPMax");
        CGC(ref _mp, "MP");
        CGC(ref _mpBase, "MPMax");
        AC(ref _buttonE);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="role">角色</param>
    public void Init(in Role role)
    {
        Display();

        _player = role;
        _miniHead.sprite = _player.RoleData.ProfilePictureDic[ExpressionType.Normal];
        _hp.text = _player.HP.ToString();
        _hpBase.text = _player.HPMax.ToString();
        _mp.text = _player.MP.ToString();
        _mpBase.text = _player.MPMax.ToString();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(UnityAction select, UnityAction selected, in Role player)
    {
        _select = select;
        _buttonE.Init(selected);
        MaskableGraphic.raycastTarget = true;
        Init(player);
    }

    /// <summary>
    /// 刷新
    /// </summary>
    public void Refresh()
    {
        _hp.text = _player.HP.ToString();
        _mp.text = _player.MP.ToString();
    }

    /// <summary>
    /// 选中
    /// </summary>
    /// <returns>选中角色</returns>
    public virtual Role Select() => _player;

    /// <summary>
    /// 取消选择
    /// </summary>
    public virtual void Unselect() { }
}