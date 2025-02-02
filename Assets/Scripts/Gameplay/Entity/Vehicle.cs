using UnityEngine;
using DG.Tweening;

/// <summary>
/// 载具
/// </summary>
public sealed class Vehicle : StuffAnim
{
    /// <summary>
    /// 移动速度
    /// </summary>
    private const int MOVE_SPEED = 1;

    protected override void OnEnable()
    {
        base.OnEnable();

        GameManager_.Trigger(GameEventType.MoveSwitch, null, "false");
        GameManager_.Leader.Transform.SetParent(Transform);

        VehicleData data = DataManager_.VehicleDataDic[Transform.parent.name + Const.SPLIT_3 + name];
        float duration = Vector3.Distance(data.Target, Transform.position) / MOVE_SPEED;
        Transform.DOMove(data.Target, duration).SetEase(Ease.Linear).onComplete = () =>
        {
            GameManager_.TriggerAll(data.ReachEventArray);
            GameManager_.Leader.Transform.SetParent(DataManager_.RoleParent);
            GameManager_.Trigger(GameEventType.MoveSwitch, null, "true");
        };
    }

    protected override void OnTriggerExit(Collider collider)
    {
        base.OnTriggerExit(collider);

        if (GameManager_.Leader.gameObject == collider.gameObject)
        {
            GameManager_.Trigger(DataManager_.VehicleDataDic[Transform.parent.name + Const.SPLIT_3 + name].LeaveEvent);
        }
    }
}