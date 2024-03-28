using UnityEngine;

/// <summary>
/// 单例基类
/// </summary>
public abstract class SingletonBase<T> : MonoBehaviourBase where T : SingletonBase<T>
{
    /// <summary>
    /// 单例
    /// </summary>
    public static T Instance { get; private set; }

    protected new static Transform Transform { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        Instance = this as T;

        Transform = GetComponent<Transform>();
    }
}