/// <summary>
/// һ���Գ������߶���
/// </summary>
public sealed class StuffAnimOneShot : StuffAnim
{
    protected override void Awake()
    {
        IsOneShot = false;

        base.Awake();
    }
}