/// <summary>
/// 初始化面板
/// </summary>
public sealed class InitPanel : UIPanelBase
{
    protected override void Awake()
    {
        base.Awake();

        Invoke(nameof(Begin), 3);
    }

    /// <summary>
    /// 阿伟你又在打电动哦
    /// </summary>
    private void Begin() => GameManager_.Trigger(GameEventType.VideoCG, "0");
}