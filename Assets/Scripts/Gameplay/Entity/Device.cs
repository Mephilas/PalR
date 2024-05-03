using UnityEngine;

/// <summary>
/// 装置
/// </summary>
public class Device : MonoBehaviourBase
{
    /// <summary>
    /// 提示音效
    /// </summary>
    protected static readonly GameEventData TIP_SOUND_EFFECTS = new(GameEventType.VaultSoundEffects, "Tip");

    /// <summary>
    /// 交互
    /// </summary>
    public virtual void Interact() => GameManager_.TriggerAll(DataManager_.MapEventDataDic[Transform.parent.name + Const.SPLIT_3 + name]);
}