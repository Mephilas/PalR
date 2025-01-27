using UnityEngine;

/// <summary>
/// 角色移动
/// </summary>
public partial class Role : SpriteBase
{
    /// <summary>
    /// 停止距离
    /// </summary>
    private const int STOP_DISTANCE = 1;

    /// <summary>
    /// 追击距离
    /// </summary>
    private const int CHASER_DISTANCE = 10;

    /// <summary>
    /// 移动目标列表
    /// </summary>
    private readonly System.Collections.Generic.List<Vector3> _moveTargetList = new();

    /// <summary>
    /// 默认位置
    /// </summary>
    private Vector3 _defaultP;

    private void Move2(string[] data)
    {
        string[] tempSA = data[1].Split(Const.SPLIT_2);
        for (int i = 0; i < tempSA.Length; i++)
        {
            _moveTargetList.Add(tempSA[i].Split(Const.SPLIT_3).SA2V3());
        }
    }

    /// <summary>
    /// 持续移动
    /// </summary>
    private void KeepMoving()
    {
        if (0 != _moveTargetList.Count && STOP_DISTANCE < Vector3.Distance(Transform.localPosition, GameManager_.Leader.Transform.localPosition))
        {
            Approach(_moveTargetList[0]);
        }
    }

    /// <summary>
    /// 追踪
    /// </summary>
    private void EnemyChaser()
    {
        if (CompareTag(nameof(Hostile)) && !GameManager_.Drive)
        {
            if (Vector3.Distance(_defaultP, GameManager_.Leader.Transform.localPosition) < (GameManager_.Bait ? CHASER_DISTANCE * 2 : CHASER_DISTANCE))
            {
                Approach(GameManager_.Leader.Transform.localPosition);
            }
            else if (CHASER_DISTANCE < Vector3.Distance(Transform.localPosition, _defaultP))
            {
                Approach(_defaultP);
            }
        }
    }

    /// <summary>
    /// 抵近
    /// </summary>
    /// <param name="target"></param>
    private void Approach(Vector3 target)
    {
        InputType keyCode;

        if (target.x <= Transform.localPosition.x && target.y <= Transform.localPosition.y)
        {
            keyCode = InputType.Up;
        }
        else if (Transform.localPosition.x < target.x && Transform.localPosition.y < target.y)
        {
            keyCode = InputType.Down;
        }
        else if (Transform.localPosition.x < target.x && target.y <= Transform.localPosition.y)
        {
            keyCode = InputType.Left;
        }
        else
        {
            keyCode = InputType.Right;
        }

        InputHandle(keyCode);
    }
}