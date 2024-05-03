/// <summary>
/// 仙术界面玩家简介
/// </summary>
public sealed class SkillPlayerProfile : PlayerProfile, UnityEngine.EventSystems.IPointerEnterHandler
{
    public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (!_isSelected)
        {
            _isSelected = true;
            _select();
        }
    }

    /// <summary>
    /// 选中
    /// </summary>
    /// <returns>选中角色</returns>
    public override Role Select()
    {
        _isSelected = true;

        _miniHead.color = UnityEngine.Color.white;

        return _player;
    }

    /// <summary>
    /// 取消选择
    /// </summary>
    public override void Unselect()
    {
        _isSelected = false;

        _miniHead.color = Const.GREYISH;
    }
}