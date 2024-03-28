using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

/// <summary>
/// �����������
/// </summary>
public sealed class CameraController : MonoBehaviourBase
{
    /// <summary>
    /// ����ƫ��
    /// </summary>
    private static readonly Vector3 FOLLOW_OFFSET = new(0, 5, -0.09f);

    /// <summary>
    /// �����
    /// </summary>
    public static Camera Camera { get; private set; }

    /// <summary>
    /// ���濪��
    /// </summary>
    private static bool _follow = true;

    private static TweenerCore<Vector3, Vector3, VectorOptions> _moveDT;

    private static TweenerCore<float, float, FloatOptions> _scaleDT;

    protected override void Awake()
    {
        base.Awake();

        GameManager_.Register(GameEventType.CameraFollow, CameraFollow);
        GameManager_.Register(GameEventType.CameraMove2Leader, CameraMove2Leader);
        GameManager_.Register(GameEventType.CameraMove, CameraMove);
        GameManager_.Register(GameEventType.CameraScale, CameraScale);
        GameManager_.Register(GameEventType.CameraRotate, CameraRotate);

        Camera = GC<Camera>();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (GameManager_.InGame && _follow) Transform.position = GameManager_.Leader.Transform.position + FOLLOW_OFFSET;
        //if (_follow) Transform.localPosition = Vector3.SmoothDamp(Transform.position, GameManager_.Leader.Transform.localPosition + OFFSET, ref tempV, 1);
    }

    private void CameraFollow(string[] data) => _follow = bool.Parse(data[0]);
    private void CameraMove2Leader(string[] data)
    {
        string[] moveData = new string[data.Length + 3];
        moveData[0] = data[0];
        if (1 < data.Length) moveData[4] = data[1];
        moveData[1] = GameManager_.Leader.Transform.position.x.ToString();
        moveData[2] = FOLLOW_OFFSET.y.ToString();
        moveData[3] = GameManager_.Leader.Transform.position.z.ToString();

        GameManager_.Trigger(new(GameEventType.CameraMove, moveData));
    }
    private void CameraMove(string[] data)
    {
        _follow = false;
        _moveDT.Kill();
        GameManager_.Trigger(UIPanelBase.NULL_PANEL_EVENT);

        (_moveDT = Transform.DOMove(ToolsE.SA2V(data), float.Parse(data[0]))).onComplete = () =>
        {
            if (4 < data.Length) GameManager_.Trigger(new(GameEventType.Dialogue, data[4]));
        };
    }
    private void CameraScale(string[] data)
    {
        _scaleDT.Kill();
        _scaleDT = Camera.DOOrthoSize(float.Parse(data[0]), float.Parse(data[1]));
    }
    private void CameraRotate(string[] data) { }
}