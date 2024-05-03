using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 选择器
/// </summary>
public class Selector : UIBase_, IPointerEnterHandler
{
    /// <summary>
    /// 变色
    /// </summary>
    private static readonly Color LIGHT_COLOR = new(1, 0.75f, 0);

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
    /// 内容ID
    /// </summary>
    protected int ContentID { get; set; }

    /// <summary>
    /// 特效控制器
    /// </summary>
    private DG.Tweening.Core.TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions> _do;

    /// <summary>
    /// 被选中
    /// </summary>
    protected bool IsSelected { get; set; }

    protected override void Awake()
    {
        base.Awake();

        AC(ref _buttonE).Init(Selected);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsSelected) _select?.Invoke();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="select">选择</param>
    /// <param name="selected">触发事件</param>
    public virtual void Init(UnityAction select, UnityAction selected)
    {
        MaskableGraphic.raycastTarget = true;

        _select = select;
        _buttonE.Init(_selected = selected);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="select">选择</param>
    /// <param name="selected">选中</param>
    /// <param name="index">序号</param>
    /// <param name="contentID">内容ID</param>
    /// <param name="text">文本</param>
    public virtual void Init(UnityAction select, UnityAction selected, string text, int contentID = 0, int index = 0, int count = 0) { }

    /// <summary>
    /// 选择
    /// </summary>
    /// <returns>内容ID</returns>
    public virtual int Select()
    {
        if (!IsSelected) Effect(IsSelected = true);

        return ContentID;
    }

    /// <summary>
    /// 取消选择
    /// </summary>
    public virtual void Unselect(bool isPause = false)
    {
        if (IsSelected)
        {
            IsSelected = false;
            Effect(isPause ? null : false);
        }
    }

    /// <summary>
    /// 选中
    /// </summary>
    public void Selected() => _selected?.Invoke();

    /// <summary>
    /// 清空
    /// </summary>
    public virtual void Clear()
    {
        Unselect();

        MaskableGraphic.raycastTarget = false;
    }

    /// <summary>
    /// 选中效果
    /// </summary>
    /// <param name="sw1tch">开关停</param>
    public virtual void Effect(bool? sw1tch = null)
    {
        _buttonE.UI.raycastTarget = !(null == sw1tch);

        _buttonE.UI.color = null == sw1tch ? Color.red : (bool)sw1tch ? LIGHT_COLOR : Const.GREYISH;
    }
}