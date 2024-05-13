using DG.Tweening;

/// <summary>
/// 初始化面板
/// </summary>
public sealed class InitPanel : UIPanelBase
{
    private const int LOGO_FADE_DURATION = 3;

    protected override void Awake()
    {
        base.Awake();

        StartCoroutine(nameof(BeginC));
    }

    /// <summary>
    /// 阿伟你又在打电动哦
    /// </summary>
    private System.Collections.IEnumerator BeginC()
    {
        yield return Const.WAIT_FOR_2S;

        CGC<UnityEngine.UI.MaskableGraphic>("LogoSoftStar").DOFade(1, LOGO_FADE_DURATION);
        CGC<UnityEngine.UI.MaskableGraphic>("Copyright").DOFade(1, LOGO_FADE_DURATION);

        yield return Const.WAIT_FOR_3S;

        CGC<UnityEngine.UI.MaskableGraphic>("LogoSoftStar").DOFade(0, LOGO_FADE_DURATION);
        CGC<UnityEngine.UI.MaskableGraphic>("Copyright").DOFade(0, LOGO_FADE_DURATION);

        yield return Const.WAIT_FOR_3S;

        CGC<UnityEngine.UI.MaskableGraphic>("LogoCube").DOFade(1, LOGO_FADE_DURATION);
        CGC<UnityEngine.UI.MaskableGraphic>("CopyrightCube").DOFade(1, LOGO_FADE_DURATION);

        yield return Const.WAIT_FOR_3S;

        CGC<UnityEngine.UI.MaskableGraphic>("LogoCube").DOFade(0, LOGO_FADE_DURATION);
        CGC<UnityEngine.UI.MaskableGraphic>("CopyrightCube").DOFade(0, LOGO_FADE_DURATION);

        yield return Const.WAIT_FOR_3S;

        GameManager_.Trigger(GameEventType.VideoCG, "0");
    }
}