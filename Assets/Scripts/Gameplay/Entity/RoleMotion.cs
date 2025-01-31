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
    /// 巡逻点列表
    /// </summary>
    private readonly System.Collections.Generic.List<Vector3> _patrolPointList = new();

    /// <summary>
    /// 默认位置
    /// </summary>
    private Vector3 _defaultP;

    /// <summary>
    /// 移动序号
    /// </summary>
    private int _moveIndex;

    /// <summary>
    /// 巡逻序号
    /// </summary>
    private int _patrolIndex;

    /// <summary>
    /// 追击中
    /// </summary>
    private bool _chasing;

    private void Move2(string[] data)
    {
        _moveTargetList.Clear();
        _moveIndex = 0;

        string[] tempSA = data[1].Split(Const.SPLIT_2);

        for (int i = 0; i < tempSA.Length; i++)
        {
            _moveTargetList.Add(tempSA[i].Split(Const.SPLIT_3).SA2V3());
        }
    }

    /// <summary>
    /// 移动
    /// </summary>
    private void KeepMoving()
    {
        if (0 != _moveTargetList.Count && _moveTargetList.Valid(_moveIndex))
        {
            if (STOP_DISTANCE < Vector3.Distance(Transform.localPosition, _moveTargetList[_moveIndex]))
            {
                Approach(_moveTargetList[_moveIndex]);
            }
            else
            {
                _moveIndex++;
            }
        }
    }

    private void Patrol(string[] data)
    {
        _patrolPointList.Clear();
        _patrolIndex = 0;

        string[] tempSA = data[1].Split(Const.SPLIT_2);

        for (int i = 0; i < tempSA.Length; i++)
        {
            _patrolPointList.Add(tempSA[i].Split(Const.SPLIT_3).SA2V3());
        }
    }

    /// <summary>
    /// 巡逻
    /// </summary>
    private void Patrol()
    {
        if (!_chasing && 0 != _patrolPointList.Count && _patrolPointList.Valid(_patrolIndex))
        {
            if (STOP_DISTANCE < Vector3.Distance(Transform.localPosition, _patrolPointList[_patrolIndex]))
            {
                Approach(_patrolPointList[_patrolIndex]);
            }
            else
            {
                _patrolIndex = 0;
            }
        }
    }

    /// <summary>
    /// 追踪
    /// </summary>
    private void EnemyChaser()
    {
        if (_chasing == CompareTag(nameof(Hostile)) && !GameManager_.Drive)
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
    /// <param name="target">目标</param>
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