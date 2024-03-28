using UnityEngine;

/// <summary>
/// װ��
/// </summary>
public class Device : MonoBehaviourBase
{
    /// <summary>
    /// ��ʾ��Ч
    /// </summary>
    protected static readonly GameEventData TIP_SOUND_EFFECTS = new(GameEventType.VaultSoundEffects, "Tip");

    /// <summary>
    /// ����
    /// </summary>
    public virtual void Interact() => GameManager_.TriggerAll(DataManager_.MapEventDataDic[Transform.parent.name + Const.SPLIT_3 + name]);
}