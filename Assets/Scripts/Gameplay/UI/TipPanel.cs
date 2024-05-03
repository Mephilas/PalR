using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 提示面板
/// </summary>
public sealed class TipPanel : UIPanelBase
{
    /// <summary>
    /// 提示面板事件
    /// </summary>
    private static readonly GameEventData TIP_PANEL_EVENT = new(GameEventType.UIPanel, UIPanel.TipPanel.ToString());

    /// <summary>
    /// 提示时长
    /// </summary>
    private const int TIP_DURATION = 1;

    /// <summary>
    /// 提示Text
    /// </summary>
    private static Text _tipT;

    /// <summary>
    /// 提示BG
    /// </summary>
    private static Image _tipBG;

    /// <summary>
    /// 提示BG位置
    /// </summary>
    private static RectTransform _tipBGT;

    /// <summary>
    /// 提示数据
    /// </summary>
    private static TipData _tipData;

    protected override void Awake()
    {
        base.Awake();

        AC<ButtonE>().Init(Escape);
        CGC(ref _tipBG, "BG");
        CGC(ref _tipBGT, "BG");
        CGC(ref _tipT, "Tip");

        GameManager_.Register(GameEventType.Tip, TipStart);
    }

    protected override void Escape() => Enter();

    protected override void Enter()
    {
        //CancelInvoke(nameof(TipEnd));

        //TipEnd();
    }

    /// <summary>
    /// 提示开始
    /// </summary>
    /// <param name="tipData">提示数据</param>
    private void TipStart(string[] tipData)
    {
        GameManager_.Trigger(TIP_PANEL_EVENT);

        _tipData = DataManager_.TipDataArray[int.Parse(tipData[0])];
        _tipT.text = _tipData.TipText;
        _tipBGT.sizeDelta = new(_tipBGT.sizeDelta.y + _tipT.fontSize * _tipT.text.Length, _tipBGT.sizeDelta.y);

        Invoke(nameof(TipEnd), TIP_DURATION);
    }

    /// <summary>
    /// 提示结束
    /// </summary>
    private void TipEnd()
    {
        GameManager_.Trigger(PANEL_RETURN_EVENT);

        if (null != _tipData.EventArray) GameManager_.TriggerAll(_tipData.EventArray);
    }
}