using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;

/// <summary>
/// CG面板
/// </summary>
public sealed class CGPanel : UIPanelBase
{
    /// <summary>
    /// 颜色变化时长
    /// </summary>
    private const int COLOR_CHANGE_TIME = 2;

    /// <summary>
    /// CG面板启动事件
    /// </summary>
    private static readonly GameEventData CG_PANEL_EVENT = new(GameEventType.UIPanel, UIPanel.CGPanel.ToString());

    /// <summary>
    /// 渲染图片
    /// </summary>
    private static UnityEngine.UI.RawImage _rawImage;

    /// <summary>
    /// 视频播放器
    /// </summary>
    private static UnityEngine.Video.VideoPlayer _videoPlayer;

    /// <summary>
    /// 序列帧CGID
    /// </summary>
    private static int _sequenceCGID;

    /// <summary>
    /// 序列帧CG集合
    /// </summary>
    private static Sprite[] _sequenceCG;

    /// <summary>
    /// CG种类
    /// </summary>
    private static bool _sequenceOrVideo;

    private static TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions> _doWhite;

    private static TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions> _doBlack;

    protected override void Awake()
    {
        base.Awake();

        CGC(ref _rawImage, "CG");
        CGC(ref _videoPlayer, "CG").loopPointReached += VideoCGOver;
        AC<ButtonE>().Init(Enter);

        GameManager_.Register(GameEventType.VideoCG, VideoCGPlay);
        GameManager_.Register(GameEventType.SequenceCG, SequenceCGPlay);
    }

    protected override void Escape() => Enter();

    protected override void Enter()
    {
        if (_sequenceOrVideo)
        {

        }
        else
        {
            _videoPlayer.Stop();
            VideoCGOver(_videoPlayer);
        }
    }

    protected override void Update()
    {
        if (_videoPlayer.isPlaying && null != _videoPlayer.texture) _rawImage.texture = _videoPlayer.texture;
    }

    /// <summary>
    /// 视频CG播放
    /// </summary>
    /// <param name="videoID">cg id</param>
    private void VideoCGPlay(string[] videoID)
    {
        GameManager_.Trigger(BG_STOP);
        GameManager_.Trigger(CG_PANEL_EVENT);

        _sequenceOrVideo = false;
        //_rawImage.color = Color.white;
        _doBlack.Kill(true);
        _doWhite = _rawImage.DOColor(Color.white, COLOR_CHANGE_TIME);
        _videoPlayer.clip = DataManager_.VideoCGArray[int.Parse(videoID[0])];
        _videoPlayer.Play();
        _videoPlayer.SetDirectAudioVolume(0, 0.2f);
    }

    /// <summary>
    /// 视频CG结束
    /// </summary>
    /// <param name="videoPlayer">播放器</param>
    private void VideoCGOver(UnityEngine.Video.VideoPlayer videoPlayer)
    {
        _doWhite.Kill(true);
        //_rawImage.color = Color.black;
        _doBlack = _rawImage.DOColor(Color.black, COLOR_CHANGE_TIME);
        GameManager_.Trigger(VIDEO_CG_EVENT_ARRAY[int.Parse(videoPlayer.clip.name)]);
    }

    /// <summary>
    /// 序列帧CG播放
    /// </summary>
    /// <param name="sequenceID">cg id</param>
    private void SequenceCGPlay(string[] sequenceID)
    {
        GameManager_.Trigger(BG_STOP);
        GameManager_.Trigger(CG_PANEL_EVENT);

        _sequenceOrVideo = true;
        _sequenceCG = DataManager_.SequenceCGList[_sequenceCGID = int.Parse(sequenceID[0])];
        //_rawImage.color = Color.white;
        _doBlack.Kill(true);
        _doWhite = _rawImage.DOColor(Color.white, COLOR_CHANGE_TIME);

        StartCoroutine(nameof(SequenceCGPlayI));
    }

    /// <summary>
    /// 序列帧CG播放协程
    /// </summary>
    /// <returns></returns>
    private System.Collections.IEnumerator SequenceCGPlayI()
    {
        for (int i = 0; i != _sequenceCG.Length; i++)
        {
            _rawImage.texture = _sequenceCG[i].texture;

            yield return Const.SEQUENCE_CG_PLAY_SPEED;
        }

        GameManager_.Trigger(SEQUENCE_CG_EVENT_ARRAY[_sequenceCGID]);

        StopCoroutine(nameof(SequenceCGPlayI));
    }

    /// <summary>
    /// 视频CG事件集合
    /// </summary>
    private static readonly GameEventData[] VIDEO_CG_EVENT_ARRAY = new GameEventData[]
    {
        new(GameEventType.VideoCG, "1"),
        MAIN_PANEL_EVENT,
        new(GameEventType.Dialogue, "0"),
    };

    /// <summary>
    /// 序列帧CG事件集合
    /// </summary>
    private static readonly GameEventData[] SEQUENCE_CG_EVENT_ARRAY = new GameEventData[]
    {
        new(GameEventType.VideoCG, "1"),
        new(GameEventType.Dialogue, "501"),
    };
}