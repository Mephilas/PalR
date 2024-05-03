using UnityEngine;

/// <summary>
/// 玩家
/// </summary>
public sealed class Player : Role
{
    /// <summary>
    /// 射线长度
    /// </summary>
    private const float RAY_LENGTH = 3;

    /// <summary>
    /// 射线方向偏移集合
    /// </summary>
    private static readonly float[] RAY_DIRECTION_OFFSET_ARRAY = new float[] { 0.2f, 0.15f, 0.1f, 0 };

    /// <summary>
    /// 交互距离
    /// </summary>
    public const float INTERACT_DISTANCE = 0.275f;

    /// <summary>
    /// 射线层级
    /// </summary>
    private static int _rayLayer;

    /// <summary>
    /// 射线方向
    /// </summary>
    private Vector3 _rayDirection;

    /// <summary>
    /// 射线碰撞
    /// </summary>
    private static RaycastHit _raycastHit;

    /// <summary>
    /// 侦测点
    /// </summary>
    private Transform _detectT;

    /// <summary>
    /// 移动向量
    /// </summary>
    private Vector3 _moveV;

    /// <summary>
    /// 是否移动
    /// </summary>
    protected override bool IsMoving { get { return Vector3.zero != _moveV; } }

    /// <summary>
    /// 空闲判断
    /// </summary>
    private bool _isIdle;

    /// <summary>
    /// 交互目标
    /// </summary>
    private static Role _interactRole;

    /// <summary>
    /// 出售物品
    /// </summary>
    public static int[] SellItem { get { return _interactRole.RoleData.SellItem; } }

    protected override void Awake()
    {
        base.Awake();

        _rayLayer = 1 << LayerMask.NameToLayer(nameof(Device)) | 1 << LayerMask.NameToLayer(nameof(Role));

        CGC(ref _detectT, "Head");
    }

    protected override void Update()
    {
        base.Update();

        IdleCheck();

        _isIdle = true;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (GameManager_.InGame && this == GameManager_.Leader && other.CompareTag(nameof(Hostile)))
        {
            GameManager_.Trigger(new(GameEventType.Battle, other.GetComponent<Hostile>().RoleData.ID.ToString()));

            other.GetComponent<Hostile>().Hide();
        }
    }

    /// <summary>
    /// 侦察
    /// </summary>
    public override void Detect()
    {
        for (int i = 0; i != RAY_DIRECTION_OFFSET_ARRAY.Length; i++)
        {
            if (Physics.Raycast(_detectT.position + _rayDirection.normalized * RAY_DIRECTION_OFFSET_ARRAY[i], Vector3.down, out _raycastHit, RAY_LENGTH, _rayLayer))
            {
                RayHandle(_raycastHit, false);

                return;
            }
        }
    }

    public override void ScreenRaycast()
    {
        if (Physics.Raycast(CameraController.Camera.ScreenPointToRay(Input.mousePosition), out _raycastHit, 100, _rayLayer))
            RayHandle(_raycastHit, true);
    }

    /// <summary>
    /// 射线处理
    /// </summary>
    /// <param name="raycastHit">射线碰撞</param>
    private void RayHandle(RaycastHit raycastHit, bool keyboard)
    {
        if (keyboard || Vector3.Distance(raycastHit.collider.transform.position, Transform.position) < INTERACT_DISTANCE)
        {
            if (raycastHit.collider.gameObject.CompareTag(nameof(Role)))
            {
                (_interactRole = raycastHit.collider.GetComponent<Role>()).Interact();
            }
            else if (raycastHit.collider.gameObject.CompareTag(nameof(Device)))
            {
                raycastHit.collider.gameObject.GetComponent<Device>().Interact();
                //GameManager_.TriggerAll(DataManager_.MapEventDataDic[raycastHit.collider.transform.parent.name + Const.SPLIT_3 + raycastHit.collider.name]);
            }
        }
        else ToolsE.LogWarning("Too far away:  " + Vector3.Distance(raycastHit.collider.transform.position, Transform.position));
    }

    /// <summary>
    /// 闲置检查
    /// </summary>
    private void IdleCheck()
    {
        if (CanMove && _isIdle && IsMoving) Idle();
    }

    protected override void Idle()
    {
        StopCoroutine(nameof(AnimationC));
        if (Vector3.zero != _moveV) _rayDirection = _moveV;
        _moveV = Vector3.zero;
        SpriteRenderer.sprite = RoleData.CurrentAnimDic[LastKeyCode][0];
    }

    /// <summary>
    /// 输入处理
    /// </summary>
    /// <param name="keyCode">输入</param>
    public override void InputHandle(InputType keyCode)
    {
        if (!CanMove) return;

        _isIdle = false;

        CurrentKeyCode = keyCode;

        if (IsMoving)
        {
            if (LastKeyCode != CurrentKeyCode)
            {
                StopCoroutine(nameof(AnimationC));
                CurrentAnimArray = RoleData.CurrentAnimDic[CurrentKeyCode];
                StartCoroutine(nameof(AnimationC));
            }
        }
        else
        {
            CurrentAnimArray = RoleData.CurrentAnimDic[CurrentKeyCode];
            StartCoroutine(nameof(AnimationC));
        }

        LastKeyCode = CurrentKeyCode;

        CharacterC.Move((_moveV = MOVE_INPUT_DIC[CurrentKeyCode]).normalized * (MOVE_SPEED * Time.deltaTime));

        //SortingOrder();
    }
}