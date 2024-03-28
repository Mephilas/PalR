/// <summary>
/// 顺序执行管理器
/// </summary>
public sealed class QueuedExecuteManager : SingletonBase<QueuedExecuteManager>
{
    /// <summary>
    /// 顺序执行队列
    /// </summary>
    public static readonly MonoBehaviourBase[] ExecuteQueue = new MonoBehaviourBase[666];

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i != ExecuteQueue.Length; i++)
            if (null != ExecuteQueue[i]) QueuedAwake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        for (int i = 0; i != ExecuteQueue.Length; i++)
            if (null != ExecuteQueue[i]) QueuedOnEnable();
    }

    protected override void Start()
    {
        base.Start();

        for (int i = 0; i != ExecuteQueue.Length; i++)
            if (null != ExecuteQueue[i]) QueuedStart();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        for (int i = 0; i != ExecuteQueue.Length; i++)
            if (null != ExecuteQueue[i]) QueuedFixedUpdate();
    }

    protected override void Update()
    {
        base.Update();

        for (int i = 0; i != ExecuteQueue.Length; i++)
            if (null != ExecuteQueue[i]) QueuedUpdate();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        for (int i = 0; i != ExecuteQueue.Length; i++)
            if (null != ExecuteQueue[i]) QueuedLateUpdate();
    }
}