using System;
using UnityEngine;

/// <summary>
/// 根节点
/// </summary>
public sealed class Root_ : SingletonBase<Root_>
{
    /// <summary>
    /// 管理器集合
    /// </summary>
    private static readonly Type[] MANAGER_ARRAY = new Type[]
    {
        typeof(DataManager_),
        typeof(UIManager_),
        typeof(GameManager_),
        typeof(AudioManager_),
        typeof(MissionManager_)
    };

    private static UnityEngine.UI.Text _fpsT, _logT;

    protected override void Awake()
    {
        base.Awake();

        Application.logMessageReceived += Bug;

        Application.targetFrameRate = 120;

        _fpsT = CGC<UnityEngine.UI.Text>("DevelopCanvas/FPS");

        _logT = CGC<UnityEngine.UI.Text>("DevelopCanvas/Log");

        StartCoroutine(nameof(FPS));

        for (int i = 0; i != MANAGER_ARRAY.Length; i++)
            gameObject.AddComponent(MANAGER_ARRAY[i]);

        /*ToolsE.LogWarning(Application.consoleLogPath);
        ToolsE.LogWarning(Application.dataPath);
        ToolsE.LogWarning(Application.persistentDataPath);
        ToolsE.LogWarning(Application.streamingAssetsPath);
        ToolsE.LogWarning(Application.temporaryCachePath);*/
    }

    private System.Collections.IEnumerator FPS()
    {
        while (true)
        {
            _fpsT.text = ((int)(1 / Time.deltaTime)).ToString();

            yield return Const.WAIT_FOR_1S;
        }
    }

    /// <summary>
    /// 屏幕输出
    /// </summary>
    /// <param name="text"></param>
    public static void ScreenLog(string text) => _logT.text += text + Const.SPLIT_RN;

    /// <summary>
    /// 除错
    /// </summary>
    /// <param name="log"></param>
    /// <param name="stackTrace"></param>
    /// <param name="type"></param>
    private static void Bug(string log, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            ToolsE.RuntimeDebug(log + Const.SPLIT_RN + Const.SPLIT_RN + stackTrace);
    }
}