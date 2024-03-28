using UnityEngine;
using DG.Tweening;

/// <summary>
/// 旋转动画
/// </summary>
public sealed class RotateAnim : SpriteBase
{
    /// <summary>
    /// 旋转时间
    /// </summary>
    private const float ROTATE_TIME = 2f;

    /// <summary>
    /// 旋转角度
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