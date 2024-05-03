using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

/// <summary>
/// 文字选择器
/// </summary>
public sealed class TextSelector : Selector
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
    private UnityEngine.UI.Text _text;

    /// <summary>
    /// 计数文本
    /// </summary>
    private UnityEngine.UI.Text _count;

    /// <summary>
    /// 序号
    /// </summary>
    public int Index { get; private set; }

    /// <summary>
    /// 特效控制器
    /// </summary>
    private DG.Tweening.Core.TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions> _do;

    protected override void Awake()
    {
        base.Awake();

        CGC(ref _text, "Text");
        if (null != Transform.Find("Count")) CGC(ref _count, "Count");
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="select">选择</param>
    /// <param name="selected">选中</param>
    /// <param name="index">序号</param>
    /// <param name="contentID">内容ID</param>
    /// <param name="text">文本</param>
    public override void Init(UnityAction select, UnityAction selected, string text, int contentID = 0, int index = 0, int count = 0)
    {
        Init(select, selected);

        Index = index;
        ContentID = contentID;
        _text.text = text;
        if (0 != count) _count.text = count.ToString();
    }

    /// <summary>
    /// 选择
    /// </summary>
    /// <returns>内容ID</returns>
    public override int Select()
    {
        if (!IsSelected) Effect(IsSelected = true);

        return ContentID;
    }

    /// <summary>
    /// 取消选择
    /// </summary>
    public override void Unselect(bool isPause = false)
    {
        if (IsSelected)
        {
            IsSelected = false;
            Effect(isPause ? null : false);
        }
    }

    /// <summary>
    /// 清空
    /// </summary>
    public override void Clear()
    {
        Unselect();
        _text.text = null;
        if (null != _count) _count.text = null;
        MaskableGraphic.raycastTarget = false;
    }

    /// <summary>
    /// 选中效果
    /// </summary>
    /// <param name="sw1tch">开关停</param>
    public override void Effect(bool? sw1tch = null)
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