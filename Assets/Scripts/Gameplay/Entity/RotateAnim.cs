using UnityEngine;
using DG.Tweening;

/// <summary>
/// ��ת����
/// </summary>
public sealed class RotateAnim : SpriteBase
{
    /// <summary>
    /// ��תʱ��
    /// </summary>
    private const float ROTATE_TIME = 2f;

    /// <summary>
    /// ��ת�Ƕ�
    /// </summary>
    private static readonly Vector3 ROTATE_ANGLE = new(0, 0, 5);

    protected override void Awake()
    {
        base.Awake();

        Rotate2L();
    }

    private void Rotate2L() => Transform.DOLocalRotate(-ROTATE_ANGLE, ROTATE_TIME).onComplete = Rotate2R;

    private void Rotate2R() => Transform.DOLocalRotate(ROTATE_ANGLE, ROTATE_TIME).onComplete = Rotate2L;
}