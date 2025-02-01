using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 台词面板
/// </summary>
public sealed class DialoguePanel : UIPanelBase
{
    /// <summary>
    /// 台词面板事件
    /// </summary>
    private static readonly GameEventData DIALOGUE_PANEL_EVENT = new(GameEventType.UIPanel, UIPanel.DialoguePanel.ToString(), "False");

    /// <summary>
    /// 角色头像Image
    /// </summary>
    private static Image _roleProfilePictureI;

    /// <summary>
    /// 角色名Text
    /// </summary>
    private static Text _roleNameT;

    /// <summary>
    /// 台词Text
    /// </summary>
    private static Text _dialogueT;

    /// <summary>
    /// 台词数据
    /// </summary>
    private static DialogueData _dialogueData;

    /// <summary>
    /// 上轮台词数据，防止事件中被覆盖
    /// </summary>
    private static DialogueData _lastDialogueData;

    /// <summary>
    /// 逐字显示
    /// </summary>
    private static readonly System.Text.StringBuilder _stringBuilder = new();

    /// <summary>
    /// 台词结束
    /// </summary>
    private bool _isDialogueEnd;

    /// <summary>
    /// 台词播放速度
    /// </summary>
    private WaitForSeconds _dialoguePlaySpeed = Const.DIALOGUE_PLAY_SPEED;

    protected override void Awake()
    {
        base.Awake();

        CGC(ref _roleProfilePictureI, "ProfilePicture");
        CGC(ref _roleNameT, "RoleName");
        CGC(ref _dialogueT, "Dialogue");
        AC<ButtonE>().Init(Enter);

        GameManager_.Register(GameEventType.Dialogue, DialogueStart);
    }

    protected override void Escape() => Enter();

    protected override void Enter()
    {
        if (!_dialogueData.AutoPlay)
        {
            if (_isDialogueEnd)
                DialogueEnd();
            else
                _dialoguePlaySpeed = Const.DIALOGUE_FAST_PLAY_SPEED;
        }
    }

    /// <summary>
    /// 台词开始
    /// </summary>
    /// <param name="dialogueDataID">台词数据ID</param>
    private void DialogueStart(string[] dialogueDataID)
    {
        GameManager_.Trigger(DIALOGUE_PANEL_EVENT);

        _dialogueData = DataManager_.DialogueDataArray[int.Parse(dialogueDataID[0])];

        if (null != _dialogueData.StartEvent) GameManager_.Trigger(_dialogueData.StartEvent);
        if (null != _dialogueData.StartEvent_) GameManager_.Trigger(_dialogueData.StartEvent_);
        _roleNameT.text = null == _dialogueData.Speaker ? null : _dialogueData.Speaker.Name + "：";

        if (ExpressionType.Nil == _dialogueData.ExpressionType)
        {
            _roleProfilePictureI.sprite = null;
            _roleProfilePictureI.color = Color.clear;
        }
        else
        {
            _roleProfilePictureI.color = Color.white;
            _roleProfilePictureI.sprite = _dialogueData.Speaker.ProfilePictureDic[_dialogueData.ExpressionType];
            _roleProfilePictureI.SetNativeSize();
        }

        _dialoguePlaySpeed = _dialogueData.AutoPlay ? Const.DIALOGUE_SLOW_PLAY_SPEED : Const.DIALOGUE_PLAY_SPEED;

        StartCoroutine(nameof(DialoguePlayC));
    }

    /// <summary>
    /// 台词播放
    /// </summary>
    /// <returns></returns>
    private IEnumerator DialoguePlayC()
    {
        for (int i = 0; i != _dialogueData.DialogueText.Length; i++)
        {
            _dialogueT.text = _stringBuilder.Append(_dialogueData.DialogueText[i]).ToString();

            yield return _dialoguePlaySpeed;
        }

        yield return Const.WAIT_FOR_POINT_1S;

        _isDialogueEnd = true;

        yield return Const.WAIT_FOR_2S;

        if (_dialogueData.AutoPlay) DialogueEnd();
    }

    /// <summary>
    /// 台词结束
    /// </summary>
    private void DialogueEnd()
    {
        StopCoroutine(nameof(DialoguePlayC));

        _stringBuilder.Clear();
        _isDialogueEnd = false;

        GameManager_.Trigger(GameEventType.MissionUpdate, MissionType.Dialogue.ToString(), _dialogueData.ID.ToString());

        if (null == _dialogueData.EventArray)
            GameManager_.Trigger(BASIC_PANEL_EVENT);
        else
            GameManager_.TriggerAll((_lastDialogueData = _dialogueData).EventArray);

        GameManager_.Trigger(GameEventType.DialogueDone, null);
    }
}