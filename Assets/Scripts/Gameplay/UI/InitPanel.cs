/// <summary>
/// 初始化面板
/// </summary>
public sealed class InitPanel : UIPanelBase
{
    protected override void Awake()
    {
        base.Awake();

        Invoke(nameof(Begin), 6);
    }

    /// <summary>
    /// 阿伟你又在打电动哦
    /// </summary>
    private void Begin() => GameManager_.Trigger(new(GameEventType.VideoCG, "0"));
}