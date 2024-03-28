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
    public virtual void Hide() => RectT.anchoredPosition = Const.HIDDEN_P;

    /// <summary>
    /// 显示
    /// </summary>
    public virtual void Display() => RectT.anchoredPosition = _defaultAnchoredP;

    /// <summary>
    /// 子物体获取
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns>子物体</returns>
    /// <exception cref="NullReferenceException">自身为空</exception>
    /// <exception cref="ArgumentException">路径错误</exception>
    protected RectTransform CGRT(string path) => CGC<RectTransform>(path);

    /// <summary>
    /// 子物体获取
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns>子物体</returns>
    protected void CGRT(ref RectTransform rectTransform, string path) => rectTransform = CGRT(path);

    /// <summary>
    /// Image获取
    /// </summary>
    /// <returns>Image</returns>
    public Image GCI() => GC<Image>();

    /// <summary>
    /// 子物体Image获取
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns>Image</returns>
    public Image CGI(string path) => CGC<Image>(path);

    /// <summary>
    /// 子物体Image获取
    /// </summary>
    /// <param name="image">Image</param>
    /// <param name="path">路径</param>
    public void CGI(ref Image image, string path) => image = CGI(path);

    /// <summary>
    /// Text获取
    /// </summary>
    /// <returns>Text</returns>
    public Text GCX() => GC<Text>();

    /// <summary>
    /// 子物体Text获取
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns>Text</returns>
    public Text CGX(string path) => CGC<Text>(path);

    /// <summary>
    /// 子物体Text获取
    /// </summary>
    /// <param name="text">Text</param>
    /// <param name="path">路径</param>
    public void CGX(ref Text text, string path) => text = CGX(path);

    /// <summary>
    /// Button获取
    /// </summary>
    /// <returns>Button</returns>
    public ButtonE GCB() => GC<ButtonE>();

    /// <summary>
    /// 子物体Button获取
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns>Button</returns>
    public ButtonE CGB(string path) => CGC<ButtonE>(path);

    /// <summary>
    /// 子物体Button获取
    /// </summary>
    /// <param name="button">Button</param>
    /// <param name="path">路径</param>
    public void CGB(ref ButtonE button, string path) => button = CGB(path);
}