using UnityEngine;
//using DG.Tweening;
//using DG.Tweening.Core;

/// <summary>
/// 战斗选择器
/// </summary>
public sealed class BattleSelector : UIBase_
{
    /// <summary>
    /// 浮动时长
    /// </summary>
    //private const float FLOAT_DURATION = 0.5f;

    /// <summary>
    /// 闪烁时长
    /// </summary>
    //private const float FLASH_DURATION = 0.05f;

    /// <summary>
    /// 浮动高度
    /// </summary>
    //private static readonly Vector2 FLOAT_HEIGHT = new(0, 5);

    /// <summary>
    /// 默认位置
    /// </summary>
    //private Vector2 _defaultPosition;

    /// <summary>
    /// 图标
    /// </summary>
    private UnityEngine.UI.MaskableGraphic _icon;

    /// <summary>
    /// 闪烁颜色
    /// </summary>
    private Color _color0, _color1;

    //private TweenerCore<Vector2, Vector2, DG.Tweening.Plugins.Options.VectorOptions> _floatDGCore, _sinkDGCore;

    //private TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions> _flash0DGCore, _flash1DGCore;

    protected override void Awake()
    {
        base.Awake();

        GC(ref _icon);
    }

    /// <summary>
    /// 启动
    /// </summary>
    public void Display(Vector2 position, Vector2 offset, Color color0, Color color1)
    {
        RectT.position = position;
        /*_defaultPosition = */RectT.anchoredPosition = RectT.anchoredPosition + offset;
        _icon.color = _color0 = color0;
        _color1 = color1;

        //Float2Top();
        //Flash0();
        StartCoroutine(nameof(FlashC));
    }

    public override void Hide()
    {
        base.Hide();

        //_floatDGCore.Kill();
        //_sinkDGCore.Kill();
        //_flash0DGCore.Kill();
        //_flash1DGCore.Kill();

        StopCoroutine(nameof(FlashC));
    }

    /// <summary>
    /// 浮动至顶
    /// </summary>
    //private void Float2Top() => (_floatDGCore = RectT.DOAnchorPos(_defaultPosition + FLOAT_HEIGHT, FLOAT_DURATION)).onComplete = Sink2Bottom;

    /// <summary>
    /// 下沉至底
    /// </summary>
    //private void Sink2Bottom() => (_sinkDGCore = RectT.DOAnchorPos(_defaultPosition - FLOAT_HEIGHT, FLOAT_DURATION)).onComplete = Float2Top;

    //private void Flash0() => (_flash0DGCore = _icon.DOColor(_color1, FLASH_DURATION)).onComplete = Flash1;

    //private void Flash1() => (_flash1DGCore = _icon.DOColor(_color0, FLASH_DURATION)).onComplete = Flash0;

    /// <summary>
    /// 闪烁，有人说上面的不够还原
    /// </summary>
    /// <returns></returns>
    private System.Collections.IEnumerator FlashC()
    {
        while (true)
        {
            _icon.color = _color1;

            yield return Const.WAIT_FOR_POINT_ZERO_5S;

            _icon.color = _color0;

            yield return Const.WAIT_FOR_POINT_ZERO_5S;
        }
    }
}