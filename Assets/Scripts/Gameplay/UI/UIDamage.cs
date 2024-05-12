using UnityEngine;
using DG.Tweening;

/// <summary>
/// UI伤害显示
/// </summary>
public sealed class UIDamage : UIBase_
{
    /// <summary>
    /// 浮动时长
    /// </summary>
    private const int FLOAT_DURATION = 1;

    /// <summary>
    /// 浮动目标
    /// </summary>
    private static readonly Vector2 FLOAT_TARGET = new(0, 50);

    /// <summary>
    /// 图标
    /// </summary>
    private UnityEngine.UI.Text _hpT, _mpT;

    /// <summary>
    /// 浮动中
    /// </summary>
    public bool Floating { get; private set; }

    private static Color _mpColor;

    protected override void Awake()
    {
        base.Awake();

        GC(ref _hpT);
        _mpColor = CGC(ref _mpT, "MP").color;
    }

    /// <summary>
    /// 启动
    /// </summary>
    public void Display(Color color, Vector2 position, string hp, string mp = null)
    {
        Floating = true;

        _hpT.color = color;
        _hpT.text = hp;
        _hpT.DOFade(0, FLOAT_DURATION);
        _mpT.color = _mpColor;
        _mpT.text = mp;
        _mpT.DOFade(0, FLOAT_DURATION);

        RectT.position = position;
        RectT.DOAnchorPos(RectT.anchoredPosition + FLOAT_TARGET, FLOAT_DURATION).onComplete = () => { Floating = false; Hide(); };
    }
}