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
        typeof(AudioManager_),
        typeof(UIManager_),
        typeof(GameManager_),
        typeof(MissionManager_)
    };

    protected override void Awake()
    {
        base.Awake();

        Application.logMessageReceived += Bug;

        Application.targetFrameRate = 120;

        //StartCoroutine(nameof(FPS), CGC<UnityEngine.UI.Text>("Canvas_/FPS"));

        for (int i = 0; i != MANAGER_ARRAY.Length; i++)
            gameObject.AddComponent(MANAGER_ARRAY[i]);

        //ToolsE.LogWarning(Application.consoleLogPath);
        //ToolsE.LogWarning(Application.dataPath);
        //ToolsE.LogWarning(Application.persistentDataPath);
        //ToolsE.LogWarning(Application.streamingAssetsPath);
        //ToolsE.LogWarning(Application.temporaryCachePath);
    }

    /*private System.Collections.IEnumerator FPS(UnityEngine.UI.Text text)
    {
        while (true)
        {
            text.text = ((int)(1 / Time.deltaTime)).ToString();

            yield return Const.WAIT_FOR_1S;
        }
    }*/

    /// <summary>
    /// 除错
    /// </summary>
    /// <param name="log"></param>
    /// <param name="stackTrace"></param>
    /// <param name="type"></param>
    private static void Bug(string log, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
        {
            ToolsE.RuntimeDebug(log + Const.SPLIT_RN + Const.SPLIT_RN + stackTrace);

            //Application.Quit();
        }
    }
}