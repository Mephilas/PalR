using UnityEngine;

/// <summary>
/// 纸片(人)基类
/// </summary>
[DisallowMultipleComponent]
public abstract class SpriteBase : MonoBehaviourBase
{
    /// <summary>
    /// 渲染器
    /// </summary>
    protected SpriteRenderer SpriteRenderer { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        SpriteRenderer = GC<SpriteRenderer>();

        SortingOrder();
    }

    /// <summary>
    /// 层级更新
    /// </summary>
    protected virtual void SortingOrder() => SpriteRenderer.sortingOrder = (short)(Transform.position.z * -10);
}