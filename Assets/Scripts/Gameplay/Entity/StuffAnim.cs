using System;
using UnityEngine;

/// <summary>
/// 场景道具动画
/// </summary>
public class StuffAnim : Stuff
{
    /// <summary>
    /// 动画循环
    /// </summary>
    protected bool AnimLoop = true;

    /// <summary>
    /// 动画集合
    /// </summary>
    protected Sprite[] _animArray;

    protected override void Awake()
    {
        base.Awake();

        string path = name;
        for (int i = 0; i != 10; i++) path = path.TrimEnd(i.ToString()[0]);
        _animArray = Resources.LoadAll<Sprite>(nameof(Stuff) + "/" + path);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (0 == _animArray.Length) throw new Exception("AnimArray length is 0 : " + gameObject);
        else StartCoroutine(nameof(AnimC));
    }

    protected System.Collections.IEnumerator AnimC()
    {
        do
        {
            for (int i = 0; i != _animArray.Length; i++)
            {
                SpriteRenderer.sprite = _animArray[i];

                yield return Const.SPECIAL_ANIMATION_PLAY_SPEED;
            }
        } while (AnimLoop);

        gameObject.SetActive(false);
        StopCoroutine(nameof(AnimC));
    }
}