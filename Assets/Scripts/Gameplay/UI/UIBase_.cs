using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI基类
/// </summary>
public abstract class UIBase_ : MonoBehaviourBase
{
    /// <summary>
    /// UI transform
    /// </summary>
    public RectTransform RectT { get; private set; }

    /// <summary>
    /// 射线接收
    /// </summary>
    protected MaskableGraphic MaskableGraphic { get; private set; }

    /// <summary>
    /// 默认锚点位置
    /// </summary>
    private Vector2 _defaultAnchoredP;

    protected override void Awake()
    {
        base.Awake();

        RectT = GC<RectTransform>();
        MaskableGraphic = GC<MaskableGraphic>();

        _defaultAnchoredP = RectT.anchoredPosition;
    }

    /// <summary>
    /// 隐藏
    /// </summary>
    public override void Hide() => RectT.anchoredPosition = Const.HIDDEN_P;

    /// <summary>
    /// 显示
    /// </summary>
    public virtual void Display() => RectT.anchoredPosition = _defaultAnchoredP;

    /// <summary>
    /// UI隐藏
    /// </summary>
    /// <param name="rectT">目标</param>
    public void UIHide(RectTransform rectT) => rectT.anchoredPosition = Const.HIDDEN_P;

    /// <summary>
    /// UI显示
    /// </summary>
    /// <param name="rectT"></param>
    public void UIDisplay(RectTransform rectT) => rectT.anchoredPosition = Vector2.zero;

    /// <summary>
    /// 子物体获取
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns>子物体</returns>
    /// <exception cref="NullReferenceException">自身为空</exception>
    /// <exception cref="ArgumentException">路径错误</exception>
    protected RectTransform CGRT(string path) => CGC<RectTransform>(path);

    /// <summary>
    /// 子物体Image获取
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns>Image</returns>
    public Image CGI(string path) => CGC<Image>(path);

    /// <summary>
    /// 子物体Text获取
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns>Text</returns>
    public Text CGX(string path) => CGC<Text>(path);

    /// <summary>
    /// 子物体Button获取
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns>Button</returns>
    public ButtonE CGB(string path) => CGC<ButtonE>(path);
}