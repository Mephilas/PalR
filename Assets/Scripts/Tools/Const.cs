using UnityEngine;

/// <summary>
/// 常量
/// </summary>
public static class Const
{
    /// <summary>
    /// 日夜切换时间
    /// </summary>
    public const float TIME_CHANGE_DURATION = 2;

    /// <summary>
    /// 隐藏位置
    /// </summary>
    public static readonly Vector2 HIDDEN_P = new(90000, 0);

    /// <summary>
    /// 透明
    /// </summary>
    public static readonly Color CLEAR = new(1, 1, 1, 0);

    /// <summary>
    /// 夜晚颜色
    /// </summary>
    public static readonly Color NIGHT_COLOR = new(0.5f, 1, 1);

    /// <summary>
    /// 深灰
    /// </summary>
    public static readonly Color CHARCOAL = new(0.25f, 0.25f, 0.25f);

    /// <summary>
    /// 浅灰
    /// </summary>
    public static readonly Color GREYISH = new(0.75f, 0.75f, 0.75f);

    /// <summary>
    /// 火攻
    /// </summary>
    public static readonly Color FIRE = new(1, 0.25f, 0);

    /// <summary>
    /// 土攻
    /// </summary>
    public static readonly Color SOIL = new(0.75f, 0.5f, 0.25f);

    /// <summary>
    /// 等待物理帧
    /// </summary>
    public static readonly WaitForFixedUpdate WAIT_FOR_FIXED_UPDATE = new();

    /// <summary>
    /// 等待0.05秒
    /// </summary>
    public static readonly WaitForSeconds WAIT_FOR_POINT_ZERO_5S = new(0.05f);

    /// <summary>
    /// 等待0.1秒
    /// </summary>
    public static readonly WaitForSeconds WAIT_FOR_POINT_1S = new(0.1f);

    /// <summary>
    /// 等待半秒
    /// </summary>
    public static readonly WaitForSeconds WAIT_FOR_HS = new(0.5f);

    /// <summary>
    /// 等待1秒
    /// </summary>
    public static readonly WaitForSeconds WAIT_FOR_1S = new(1);

    /// <summary>
    /// 等待2秒
    /// </summary>
    public static readonly WaitForSeconds WAIT_FOR_2S = new(2);

    /// <summary>
    /// 等待3秒
    /// </summary>
    public static readonly WaitForSeconds WAIT_FOR_3S = new(3);

    /// <summary>
    /// 等待5秒
    /// </summary>
    public static readonly WaitForSeconds WAIT_FOR_5S = new(5);

    /// <summary>
    /// 序列帧CG播放速度
    /// </summary>
    public static readonly WaitForSeconds SEQUENCE_CG_PLAY_SPEED = new(0.05f);

    /// <summary>
    /// 动画播放速度
    /// </summary>
    public static readonly WaitForSeconds ANIMATION_PLAY_SPEED = new(0.1f);

    /// <summary>
    /// 动画播放速度
    /// </summary>
    public static readonly WaitForSeconds SPECIAL_ANIMATION_PLAY_SPEED = new(0.2f);

    /// <summary>
    /// 仙术序列帧播放速度
    /// </summary>
    public static readonly WaitForSeconds SKILL_PLAY_SPEED = new(0.05f);

    /// <summary>
    /// 台词播放速度
    /// </summary>
    public static readonly WaitForSeconds DIALOGUE_PLAY_SPEED = new(0.05f);

    /// <summary>
    /// 台词加速播放速度
    /// </summary>
    public static readonly WaitForSeconds DIALOGUE_FAST_PLAY_SPEED = new(0.005f);

    /// <summary>
    /// 台词慢速播放速度
    /// </summary>
    public static readonly WaitForSeconds DIALOGUE_SLOW_PLAY_SPEED = new(0.15f);

    public const string SPLIT_RN = "\r\n";

    public const char SPLIT_P = '%';

    public const char SPLIT_R = '\r';

    public const char SPLIT_N = '\n';

    public const char SPLIT_0 = ',';

    public const char SPLIT_1 = '`';

    public const char SPLIT_2 = '_';

    public const char SPLIT_3 = '/';
}