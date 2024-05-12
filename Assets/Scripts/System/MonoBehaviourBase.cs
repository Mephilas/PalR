using System;
using UnityEngine;

/// <summary>
/// 自定义基类
/// </summary>
public abstract class MonoBehaviourBase : MonoBehaviour
{
    public Transform Transform { get; private set; }

    protected virtual void Awake()
    {
        Transform = GetComponent<Transform>();
    }

    public virtual void QueuedAwake()
    {

    }

    protected virtual void OnEnable()
    {

    }

    public virtual void QueuedOnEnable()
    {

    }

    protected virtual void Reset()
    {

    }

    protected virtual void Start()
    {

    }

    public virtual void QueuedStart()
    {

    }

    protected virtual void FixedUpdate()
    {

    }

    public virtual void QueuedFixedUpdate()
    {

    }

    protected virtual void Update()
    {

    }

    public virtual void QueuedUpdate()
    {

    }

    protected virtual void LateUpdate()
    {

    }

    public virtual void QueuedLateUpdate()
    {

    }

    protected virtual void OnGUI()
    {

    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        
    }

    protected virtual void OnCollisionStay(Collision collision)
    {

    }

    protected virtual void OnCollisionExit(Collision collision)
    {

    }

    protected virtual void OnTriggerEnter(Collider collider)
    {
        
    }

    protected virtual void OnTriggerStay(Collider collider)
    {

    }

    protected virtual void OnTriggerExit(Collider collider)
    {

    }

    protected virtual void OnApplicationPause()
    {

    }

    protected virtual void OnApplicationQuit()
    {

    }

    protected virtual void OnDisable()
    {

    }

    protected virtual void OnDestroy()
    {

    }

    /// <summary>
    /// 隐藏
    /// </summary>
    public virtual void Hide() => Transform.localPosition = Const.HIDDEN_P;

    /// <summary>
    /// 添加组件
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <returns>组件</returns>
    public T AC<T>() where T : Component
    {
        return gameObject.AddComponent<T>();
    }

    /// <summary>
    /// 组件添加
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="component">组件</param>
    public T AC<T>(ref T component) where T : Component
    {
        return component = AC<T>();
    }

    /// <summary>
    /// 组件获取
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <returns>组件</returns>
    /// <exception cref="ArgumentException">类型错误</exception>
    public T GC<T>() where T : Component
    {
        if (TryGetComponent(out T component))
            return component;
        else
            throw new ArgumentException("Type error");
    }

    /// <summary>
    /// 组件获取
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <returns>组件</returns>
    /// <exception cref="ArgumentException">类型错误</exception>
    public T GC<T>(ref T componentT) where T : Component
    {
        return componentT = GC<T>();
    }

    /// <summary>
    /// 子物体获取
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns>子物体</returns>
    /// <exception cref="NullReferenceException">自身为空</exception>
    /// <exception cref="ArgumentException">路径错误</exception>
    public Transform CGT(string path)
    {
        Transform tempT;

        if (null == Transform) throw new NullReferenceException("Transform is null, check awake");
        else if (null == (tempT = Transform.Find(path))) throw new ArgumentException("Path error : " + path);
        else return tempT;
    }

    /// <summary>
    /// 子物体获取
    /// </summary>
    /// <param name="transform">Transform</param>
    /// <param name="path">路径</param>
    public void CGT(ref Transform transform, string path) => transform = CGT(path);

    /// <summary>
    /// 子物体组件获取
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="path">路径</param>
    /// <returns>组件</returns>
    /// <exception cref="ArgumentException"></exception>
    public T CGC<T>(string path) where T : Component
    {
        if (CGT(path).TryGetComponent(out T component))
            return component;
        else
            throw new ArgumentException();
    }

    /// <summary>
    /// 子物体组件获取
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="componentT">组件</param>
    /// <param name="path">路径</param>
    /// <exception cref="ArgumentException"></exception>
    public T CGC<T>(ref T componentT, string path) where T : Component
    {
        return componentT = CGC<T>(path);
    }

    /// <summary>
    /// 子物体组件获取
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <returns>组件集合</returns>
    public T[] CGC<T>() where T : Component
    {
        return GetComponentsInChildren<T>();
    }

    /// <summary>
    /// 子物体组件获取
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="tArray">组件集合</param>
    public void CGC<T>(ref T[] tArray) where T : Component
    {
        tArray = CGC<T>();
    }
}