/// <summary>
/// 一次性场景道具动画
/// </summary>
public sealed class StuffAnimOneShot : StuffAnim
{
    protected override void Awake()
    {
        AnimLoop = false;

        base.Awake();
    }
}