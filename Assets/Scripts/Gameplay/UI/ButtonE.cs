using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// 增强按钮
/// </summary>
[DisallowMultipleComponent]
public sealed class ButtonE : UIBase_, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    /// <summary>
    /// 音效事件
    /// </summary>
    private static readonly GameEventData BUTTON_AUDIO_EVENT = new(GameEventType.ButtonAudio);

    /// <summary>
    /// 缩放尺寸
    /// </summary>
    private static readonly Vector2 SCALE = new(0.9f, 0.9f);

    /// <summary>
    /// 缩放时间
    /// </summary>
    private const float SCALE_DURATION = 0.5f;

    /// <summary>
    /// 缩放控制器
    /// </summary>
    private static DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> _do;

    /// <summary>
    /// 点击/长按
    /// </summary>
    private bool _clickOrHold = true;

    /// <summary>
    /// 方法
    /// </summary>
    private UnityAction _method;

    public UnityEngine.UI.MaskableGraphic UI { get; set; }

    protected override void Awake()
    {
        base.Awake();

        UI = GC<UnityEngine.UI.MaskableGraphic>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_clickOrHold)
        {
            _do = RectT.DOScale(SCALE, SCALE_DURATION);
            GameManager_.Trigger(BUTTON_AUDIO_EVENT);
            StartCoroutine(nameof(HoldC));
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_clickOrHold)
        {
            _do.Kill();
            RectT.DOScale(Vector2.one, SCALE_DURATION);
            StopCoroutine(nameof(HoldC));
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_clickOrHold) Invoke();
    }

    /// <summary>
    /// 长按
    /// </summary>
    /// <returns>间隔时间</returns>
    private System.Collections.IEnumerator HoldC()
    {
        while (true)
        {
            Invoke();

            yield return Time.deltaTime;
        }
    }

    /// <summary>
    /// 调用
    /// </summary>
    private void Invoke() => _method?.Invoke();

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="method">方法</param>
    /// <param name="clickOrHold">点击/长按</param>
    public void Init(UnityAction method, bool clickOrHold = true)
    {
        _method = method;

        _clickOrHold = clickOrHold;
    }
}