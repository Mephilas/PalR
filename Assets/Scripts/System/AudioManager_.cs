using UnityEngine;

/// <summary>
/// 音频管理器
/// </summary>
public sealed class AudioManager_ : SingletonBase<AudioManager_>
{
    /// <summary>
    /// 背景音源
    /// </summary>
    private static AudioSource _bgAS;

    /// <summary>
    /// BG序号保存用
    /// </summary>
    public static int BGIndex { get; private set; }

    /// <summary>
    /// 战斗结束恢复用
    /// </summary>
    private static int _lastBGIndex;

    /// <summary>
    /// 按钮音源
    /// </summary>
    private static AudioSource _buttonAS;

    /// <summary>
    /// 音效
    /// </summary>
    private static AudioSource _soundEffects;

    /// <summary>
    /// 宝藏音效
    /// </summary>
    private static AudioSource _vaultSoundEffects;

    protected override void Awake()
    {
        base.Awake();

        GameManager_.Register(GameEventType.BGPlay, BGPlay);
        GameManager_.Register(GameEventType.BGRecover, BGRecover);
        GameManager_.Register(GameEventType.ButtonAudio, ButtonAudio);
        GameManager_.Register(GameEventType.SoundEffects, SoundEffects);
        GameManager_.Register(GameEventType.VaultSoundEffects, VaultSoundEffects);

        CGC(ref _bgAS, "Main Camera");
        CGC(ref _buttonAS, "Main Camera/ButtonAudio");
        CGC(ref _soundEffects, "Main Camera/SoundEffects");
        CGC(ref _vaultSoundEffects, "Main Camera/VaultSoundEffects");
    }

    private void BGPlay(string[] data)
    {
        int bgID = int.Parse(data[0]);

        if (bgID != BGIndex)
        {
            if (-2 == bgID)
            {
                _bgAS.UnPause();
            }
            else if (-1 == bgID)
            {
                _bgAS.Pause();
            }
            else
            {
                _lastBGIndex = BGIndex;
                _bgAS.clip = DataManager_.BGAudioClipArray[BGIndex = bgID];
                _bgAS.Play();
            }
        }
    }
    private void ButtonAudio(string[] _) => _buttonAS.Play();
    private void BGRecover(string[] _)
    {
        _bgAS.clip = DataManager_.BGAudioClipArray[BGIndex = _lastBGIndex];
        _bgAS.Play();
    }
    private void SoundEffects(string[] data)
    {
        _soundEffects.clip = DataManager_.SoundEffectsDic[data[0]];
        _soundEffects.Play();
    }
    private void VaultSoundEffects(string[] data)
    {
        _vaultSoundEffects.clip = DataManager_.SoundEffectsDic[data[0]];
        _vaultSoundEffects.Play();
    }
}