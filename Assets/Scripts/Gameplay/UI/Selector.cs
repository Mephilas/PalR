using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// 选择器
/// </summary>
public sealed class Selector : UIBase_, IPointerEnterHandler
{
    /// <summary>
    /// 选中特效时间
    /// </summary>
    private const float SELECT_EFFECT_TIME = 0.25f;

    /// <summary>
    /// 字体颜色，吸管吸来的
    /// </summary>
    private static readonly Color DEFAULT_COLOR = new(0.6901961f, 0.6470588f, 0.6078432f),
                                  LIGHT_COLOR = new(0.8470589f, 0.6745098f, 0.3137255f),
                                  DEEP_COLOR = new(0.9568628f, 0.8941177f, 0.4235294f);

    /// <summary>
    /// 按钮文本
    /// </summary>
    private Text _text;

    /// <summary>
    /// 计数文本
    /// </summary>
    private Text _count;

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
    /// 序号
    /// </summary>
    public int Index { get; private set; }

    /// <summary>
    /// 内容ID
    /// </summary>
    private int _contentID;

    /// <summary>
    /// 特效控制器
    /// </summary>
    private DG.Tweening.Core.TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions> _do;

    /// <summary>
    /// 被选中
    /// </summary>
    private bool _isSelected;

    protected override void Awake()
    {
        base.Awake();

        CGX(ref _text, "Text");
        if (null != Transform.Find("Count")) CGX(ref _count, "Count");
        AC(ref _buttonE);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isSelected)
            _select();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="select">选择</param>
    /// <param name="selected">触发事件</param>
    public void Init(UnityAction select, UnityAction selected)
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
    public void Init(UnityAction select, UnityAction selected, string text, int contentID = 0, int index = 0, int count = 0)
    {
        Init(select, selected);

        Index = index;
        _contentID = contentID;
        _text.text = text;
        if (0 != count) _count.text = count.ToString();
    }

    /// <summary>
    /// 选择
    /// </summary>
    /// <returns>内容ID</returns>
    public int Select()
    {
        if (!_isSelected) Effect(_isSelected = true);

        return _contentID;
    }

    /// <summary>
    /// 取消选择
    /// </summary>
    public void Unselect(bool isPause = false)
    {
        if (_isSelected)
        {
            _isSelected = false;
            Effect(isPause ? null : false);
        }
    }

    /// <summary>
    /// 选中
    /// </summary>
    public void Selected() => _selected();

    /// <summary>
    /// 清空
    /// </summary>
    public void Clear()
    {
        Unselect();
        _text.text = null;
        if (null != _count) _count.text = null;
        MaskableGraphic.raycastTarget = false;
    }

    /// <summary>
    /// 特效
    /// </summary>
    /// <param name="sw1tch">开关停</param>
    private void Effect(bool? sw1tch = null)
    {
        _do.Kill();

        if (null == sw1tch) return;

        if ((bool)sw1tch)
            Light2Deep();
        else
            _text.color = DEFAULT_COLOR;
    }

    /// <summary>
    /// 浅到深
    /// </summary>
    private void Light2Deep()
    {
        _text.color = LIGHT_COLOR;
        (_do = _text.DOColor(DEEP_COLOR, SELECT_EFFECT_TIME)).onComplete = Deep2Light;
    }

    /// <summary>
    /// 深到浅
    /// </summary>
    private void Deep2Light()
    {
        _text.color = DEEP_COLOR;
        (_do = _text.DOColor(LIGHT_COLOR, SELECT_EFFECT_TIME)).onComplete = Light2Deep;
    }
}