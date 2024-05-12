using DG.Tweening;

/// <summary>
/// 团灭面板
/// </summary>
public sealed class GGPanel : UIPanelBase
{
    private const string GG = "胜败乃兵家常事\r\n大侠请重新来过";

    private static UnityEngine.UI.Image _bg;

    private static UnityEngine.UI.Text _gg, _ganso;

    protected override void Awake()
    {
        base.Awake();

        GC(ref _bg);
        CGC(ref _gg, "GG");
        CGC(ref _ganso, "Ganso");
    }

    public override void Active(string[] argumentArray = null)
    {
        base.Active(argumentArray);

        StartCoroutine(nameof(GG_));
    }

    private System.Collections.IEnumerator GG_()
    {
        _bg.DOFade(0.2f, 1);

        yield return Const.WAIT_FOR_2S;

        _gg.DOFade(1, 2);

        yield return Const.WAIT_FOR_5S;

        _ganso.DOFade(1, 2);

        yield return Const.WAIT_FOR_5S;

        BattleField.Resurrect();
    }

    public override void Inactive(bool hide)
    {
        base.Inactive(hide);

        _bg.color = _bg.color.ColorModifyA(0);
        _gg.color = _gg.color.ColorModifyA(0);
        _ganso.color = _ganso.color.ColorModifyA(0);
    }
}