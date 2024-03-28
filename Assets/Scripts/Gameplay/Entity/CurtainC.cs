using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// 幕布控制器
/// </summary>
public sealed class CurtainC : MonoBehaviourBase
{
    /// <summary>
    /// 幕布切换处理器集合
    /// </summary>
    private static Dictionary<CurtainEffectType, UnityAction> _curtainSwitchDic;

    /// <summary>
    /// 幕布参数集合
    /// </summary>
    private static string[] _curtainArguments;

    private static Color _tempColor;

    /// <summary>
    /// 渲染器
    /// </summary>
    private SpriteRenderer _spriteRenderer;

    protected override void Awake()
    {
        base.Awake();

        GC(ref _spriteRenderer);

        GameManager_.Register(GameEventType.Curtain, CurtainSwitch);

        _curtainSwitchDic = new()
        {
            { CurtainEffectType.ColorChange, ColorChange },
            { CurtainEffectType.AlphaGradual, AlphaGradual },
            { CurtainEffectType.Movement, Movement },
            { CurtainEffectType.ShapeChange, ShapeChange },
        };
    }

    /// <summary>
    /// 幕布切换
    /// </summary>
    /// <param name="curtainArguments">幕布效果</param>
    private void CurtainSwitch(string[] curtainArguments)
    {
        _curtainArguments = curtainArguments;

        _curtainSwitchDic[curtainArguments[0].S2E<CurtainEffectType>()]();
    }

    /// <summary>
    /// 变色
    /// </summary>
    private void ColorChange()
    {
        _tempColor.r = float.Parse(_curtainArguments[1]);
        _tempColor.g = float.Parse(_curtainArguments[2]);
        _tempColor.b = float.Parse(_curtainArguments[3]);
        _tempColor.a = float.Parse(_curtainArguments[4]);

        _spriteRenderer.color = _tempColor;
    }

    /// <summary>
    /// 透明渐变
    /// </summary>
    private void AlphaGradual()
    {
        if ("0" == _curtainArguments[2])
        {
            _spriteRenderer.color.ColorModifyA(float.Parse(_curtainArguments[1]));
        }
        else _spriteRenderer.DOFade(float.Parse(_curtainArguments[1]), float.Parse(_curtainArguments[2]));
    }

    /// <summary>
    /// 位移
    /// </summary>
    private void Movement()
    {
        
    }

    /// <summary>
    /// 形变
    /// </summary>
    private void ShapeChange()
    {
        
    }
}


/// <summary>
/// 幕布效果
/// </summary>
public enum CurtainEffectType
{
    /// <summary>
    /// 变色
    /// </summary>
    ColorChange,

    /// <summary>
    /// 透明渐变
    /// </summary>
    AlphaGradual,

    /// <summary>
    /// 位移
    /// </summary>
    Movement,

    /// <summary>
    /// 形变
    /// </summary>
    ShapeChange,
}