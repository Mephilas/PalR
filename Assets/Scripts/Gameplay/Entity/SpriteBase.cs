using UnityEngine;

/// <summary>
/// 纸片(人)基类
/// </summary>
[DisallowMultipleComponent]
public abstract class SpriteBase : MonoBehaviourBase
{
    /// <summary>
    /// 跳帧
    /// </summary>
    private const int SKIP_FRAME = 6;

    /// <summary>
    /// 渲染层级调整距离
    /// </summary>
    private const int SORT_DISTANCE = 3;

    /// <summary>
    /// 渲染器
    /// </summary>
    protected SpriteRenderer SpriteRenderer { get; private set; }

    /// <summary>
    /// 跳帧计数
    /// </summary>
    private int _skipCount;

    /// <summary>
    /// 排序开关
    /// </summary>
    protected bool SortSwitch = true;

    /// <summary>
    /// 排序额外加权
    /// </summary>
    protected int AdditionalWeighting;

    protected override void Awake()
    {
        base.Awake();

        SpriteRenderer = GC<SpriteRenderer>();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (SortSwitch && SKIP_FRAME == ++_skipCount)
        {
            _skipCount = 0;

            SortingOrder();
        }
    }

    /// <summary>
    /// 层级更新
    /// </summary>
    protected virtual void SortingOrder()
    {
        if (Vector3.Distance(GameManager_.Leader.Transform.position, Transform.position) < SORT_DISTANCE)
        {
            SpriteRenderer.sortingOrder = (short)((GameManager_.Leader.Transform.position.z - Transform.position.z) * 100 + AdditionalWeighting);
        }
    }
}