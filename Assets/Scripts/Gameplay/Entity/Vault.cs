/// <summary>
/// БІВи
/// </summary>
public sealed class Vault : Device
{
    public override void Interact()
    {
        base.Interact();

        GameManager_.Trigger(TIP_SOUND_EFFECTS);
    }
}