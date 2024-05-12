using System;
//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
//using UnityEngine.Networking;

/// <summary>
/// 数据管理器
/// </summary>
public sealed class DataManager_ : SingletonBase<DataManager_>
{
    /// <summary>
    /// 存档路径
    /// </summary>
    private static string SAVE_PATH { get { return Application.persistentDataPath + "/SaveData_{0}.csv"; } }

    /// <summary>
    /// 存档物体Tag集合
    /// </summary>
    private static readonly string[] SAVE_GO_TAG_ARRAY = new string[] { "Trigger", "Portal", nameof(Device), nameof(Stuff) };

    /// <summary>
    /// 视频CG集合
    /// </summary>
    public static VideoClip[] VideoCGArray { get; private set; }

    /// <summary>
    /// 序列帧CG集合
    /// </summary>
    public static readonly List<Sprite[]> SequenceCGList = new();

    /// <summary>
    /// 物品数据集合
    /// </summary>
    public static ItemData[] ItemDataArray { get; private set; }

    /// <summary>
    /// 仙术特效集合
    /// </summary>
    public static List<Sprite[]> SkillEffectList = new();

    /// <summary>
    /// 仙术数据集合
    /// </summary>
    public static SkillData[] SkillDataArray { get; private set; }

    /// <summary>
    /// Buff数据集合
    /// </summary>
    public static BuffData[] BuffDataArray { get; private set; }

    /// <summary>
    /// 台词数据集合
    /// </summary>
    public static DialogueData[] DialogueDataArray { get; private set; }

    /// <summary>
    /// 提示数据集合
    /// </summary>
    public static TipData[] TipDataArray { get; private set; }

    /// <summary>
    /// 选择数据集合
    /// </summary>
    public static ChooseData[] ChooseDataArray { get; private set; }

    /// <summary>
    /// 任务数据集合
    /// </summary>
    public static MissionData[] MissionDataArray { get; private set; }

    /// <summary>
    /// 战斗背景集合
    /// </summary>
    public static Sprite[] BattleGroundArray { get; private set; }

    /// <summary>
    /// 角色数据集合
    /// </summary>
    public static RoleData[] RoleDataArray { get; private set; }

    /// <summary>
    /// 背景音频集合
    /// </summary>
    public static AudioClip[] BGAudioClipArray { get; private set; }

    /// <summary>
    /// 音效集合
    /// </summary>
    public static readonly Dictionary<string, AudioClip> SoundEffectsDic = new();

    /// <summary>
    /// 地图事件数据集合
    /// </summary>
    public static readonly Dictionary<string, GameEventData[]> MapEventDataDic = new();

    /// <summary>
    /// 传送门数据集合
    /// </summary>
    public static readonly Dictionary<string, string> PortalDataDic = new();

    /// <summary>
    /// 角色预制体
    /// </summary>
    private static GameObject _rolePrefab;

    /// <summary>
    /// 角色根节点
    /// </summary>
    private static Transform _roleParent;

    protected override void Awake()
    {
        base.Awake();

        GameManager_.Register(GameEventType.Save, Save);
        GameManager_.Register(GameEventType.Load, Load);

        VideoCGArray = Resources.LoadAll<VideoClip>("CG/Videos");

        for (int i = 0; i != 99; i++)
        {
            Sprite[] sequenceArray = Resources.LoadAll<Sprite>("CG/Sequences/" + i);

            if (0 != sequenceArray.Length) SequenceCGList.Add(sequenceArray);
            else break;
        }

        ItemDataArray = DataLoad<ItemData>();
        SkillEffectLoad();
        SkillDataArray = DataLoad<SkillData>();
        BuffDataArray = DataLoad<BuffData>();
        DialogueDataArray = DataLoad<DialogueData>();
        TipDataArray = DataLoad<TipData>();
        ChooseDataArray = DataLoad<ChooseData>();
        MissionDataArray = DataLoad<MissionData>();
        BattleGroundArray = Resources.LoadAll<Sprite>("BattleGround");
        RoleDataArray = DataLoad<RoleData>();
        BGAudioClipArray = Resources.LoadAll<AudioClip>("Audios/BG");
        DataLoad(SoundEffectsDic, "Audios/SoundEffects");
        MapDataLoad();
        PortalDataLoad();

        //StartCoroutine(nameof(LoadFormStreamingAssets));

        _rolePrefab = Resources.Load<GameObject>("Prefabs/" + nameof(Role));

        _roleParent = CGT(nameof(Role));

        for (int i = 0; i != RoleDataArray.Length; i++)
        {
            if (0 == RoleDataArray[i].LevelLearnSkillDic.Count)
            {
                if (null == RoleDataArray[i].BattleIDGroup) RoleCreate2GM<Role>(RoleDataArray[i]);
                else RoleCreate2GM<Hostile>(RoleDataArray[i]);
            }
            else RoleCreate2GM<Player>(RoleDataArray[i]);
        }
    }

    /// <summary>
    /// 加载数据，StreamingAssets便于后续随意修改 -> Lua创意工坊或Unity扩展剧情地图编辑工具
    /// </summary>
    /// <returns></returns>
    /*private IEnumerator LoadFormStreamingAssetsC()
    {
        UnityWebRequest unityWebRequest = new(Application.streamingAssetsPath + "/SaveData_NewGame.csv") { downloadHandler = new DownloadHandlerBuffer() };

        yield return unityWebRequest.SendWebRequest();

        ToolsE.LogWarning(unityWebRequest.downloadHandler.text);

        StopCoroutine(nameof(LoadFormStreamingAssets));
    }*/

    /// <summary>
    /// 数据读取，性能
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <returns>数据数组</returns>
    private static T[] DataLoad<T>() where T : DataBase
    {
        string tempS = Resources.Load<TextAsset>("Data/" + typeof(T).Name).text;
        string[] tempSA_0 = tempS.Remove(0, tempS.IndexOf(Const.SPLIT_N) + 1).Split(Const.SPLIT_N), tempSA_1;
        T[] dataArray = new T[tempSA_0.Length];
        int index;

        for (int i = 0; i != dataArray.Length; i++)
        {
            tempSA_1 = tempSA_0[i].TrimEnd(Const.SPLIT_R, Const.SPLIT_0).Split(Const.SPLIT_0);
            index = int.Parse(tempSA_1[0]);
            if (null != dataArray[index]) ToolsE.Log(typeof(T).Name + " index repetition at " + index);
            dataArray[index] = Activator.CreateInstance(typeof(T), new object[] { tempSA_1 }) as T;
        }

        return dataArray;
    }

    /*/// <summary>
    /// 数据读取，极简
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="dataDic">数据集合</param>
    private static void DataLoad<T>(Dictionary<int, T> dataDic) where T : DataBase
    {
        string tempS = Resources.Load<TextAsset>("Data/" + typeof(T).Name).text;
        string[] tempSA_0 = tempS.Remove(0, tempS.IndexOf(Const.SPLIT_N) + 1).Split(Const.SPLIT_N), tempSA_1;

        for (int i = 0; i != tempSA_0.Length; i++)
        {
            tempSA_1 = tempSA_0[i].TrimEnd(Const.SPLIT_R).Split(Const.SPLIT_0);

            dataDic.Add(int.Parse(tempSA_1[0]), Activator.CreateInstance(typeof(T), new object[] { tempSA_1 }) as T); 
        }
    }*/

    /// <summary>
    /// 数据读取
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="dataDic">集合</param>
    /// <param name="path">路径</param>
    private static void DataLoad<T>(Dictionary<string, T> dataDic, string path) where T : UnityEngine.Object
    {
        T[] tArray = Resources.LoadAll<T>(path);

        for (int i = 0; i != tArray.Length; i++)
            dataDic.Add(tArray[i].name, tArray[i]);
    }

    /// <summary>
    /// 地图事件数据读取
    /// </summary>
    private static void MapDataLoad()
    {
        string[] tempSA_0 = DataLoad("MapData"), tempSA_1;

        for (int i = 0; i != tempSA_0.Length; i++)
        {
            tempSA_1 = tempSA_0[i].Split(Const.SPLIT_0);
            GameEventData[] gameEventDataArray = new GameEventData[tempSA_1.Last()];

            for (int j = 0; j != gameEventDataArray.Length; j++)
            {
                gameEventDataArray[j] = tempSA_1[j + 1].S2GE(Const.SPLIT_1, Const.SPLIT_2);
            }

            MapEventDataDic.Add(tempSA_1[0], gameEventDataArray);
        }
    }

    /// <summary>
    /// 传送门数据读取
    /// </summary>
    private static void PortalDataLoad()
    {
        string[] tempSA_0 = DataLoad("PortalData"), tempSA_1;

        for (int i = 0; i != tempSA_0.Length; i++)
        {
            tempSA_1 = tempSA_0[i].Split(Const.SPLIT_0);

            PortalDataDic.Add(tempSA_1[0], GameManager_.MAP_ROOT + Const.SPLIT_3 + tempSA_1[1]);
        }
    }

    /// <summary>
    /// 仙术特效读取
    /// </summary>
    private static void SkillEffectLoad()
    {
        string folder;
        Sprite[] spriteArray;

        for (int i = 0; i != 99; i++)
        {
            folder = i.ToString();
            if (i < 10) folder = "0" + folder;

            spriteArray = Resources.LoadAll<Sprite>("Skill/" + folder);

            if (0 == spriteArray.Length) return;
            else SkillEffectList.Add(spriteArray);
        }
    }

    /// <summary>
    /// 数据读取
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns>数据集合</returns>
    private static string[] DataLoad(string path)
    {
        string tempS = Resources.Load<TextAsset>("Data/" + path).text;

        string[] tempSA = tempS.Remove(0, tempS.IndexOf(Const.SPLIT_N) + 1).Split(Const.SPLIT_N);

        for (int i = 0; i != tempSA.Length; i++) tempSA[i] = tempSA[i].TrimEnd(Const.SPLIT_R, Const.SPLIT_0);

        return tempSA;
    }

    /// <summary>
    /// 角色创建
    /// </summary>
    /// <param name="id">id</param>
    public T RoleCreate<T>(RoleData roleData, Transform parentT) where T : Role
    {
        T tempRole = Instantiate(_rolePrefab, parentT).AddComponent<T>();
        tempRole.Init(roleData);

        return tempRole;
    }

    /// <summary>
    /// 角色创建
    /// </summary>
    /// <param name="id">id</param>
    private void RoleCreate2GM<T>(RoleData roleData) where T : Role
    {
        GameManager_.RoleList.Add(RoleCreate<T>(roleData, _roleParent));
    }

    /// <summary>
    /// 存档
    /// </summary>
    /// <param name="index">序号，0为自动存档</param>
    private static void Save(string[] args)
    {
        ToolsE.Log(string.Format(SAVE_PATH, args[0]));

        string saveData = string.Empty;

        for (int i = 0; i != GameManager_.RoleList.Count; i++)
        {
            saveData += GameManager_.RoleList[i].SaveData + Const.SPLIT_1;
        }
        saveData = saveData.TrimEnd(Const.SPLIT_1) + Const.SPLIT_0;

        Transform[] goArray = Root_.Instance.CGT(GameManager_.MAP_ROOT).GetComponentsInChildren<Transform>(true);
        for (int i = 0; i != goArray.Length; i++)
        {
            for (int j = 0; j != SAVE_GO_TAG_ARRAY.Length; j++)
            {
                if (goArray[i].CompareTag(SAVE_GO_TAG_ARRAY[j]))
                {
                    saveData += goArray[i].gameObject.activeInHierarchy + Const.SPLIT_2.ToString() + goArray[i].transform.GOPathGet() + Const.SPLIT_1;

                    break;
                }
            }
        }
        saveData = saveData.TrimEnd(Const.SPLIT_1) + Const.SPLIT_0;

        for (int i = 0; i != GameManager_.PlayerList.Count; i++)
        {
            saveData += GameManager_.RoleList[i].PlayerSaveData + Const.SPLIT_1;
        }
        saveData = saveData.TrimEnd(Const.SPLIT_1) + Const.SPLIT_0;

        foreach (int id in GameManager_.Bag.Keys)
        {
            saveData += id.ToString() + Const.SPLIT_2 + GameManager_.Bag[id] + Const.SPLIT_1;
        }
        saveData = saveData.TrimEnd(Const.SPLIT_1) + Const.SPLIT_0;

        saveData += GameManager_.Leader.RoleData.ID + Const.SPLIT_0.ToString();

        saveData += MissionManager_.MainMission.ID + Const.SPLIT_0.ToString();

        saveData += GameManager_.BattleGroundID + Const.SPLIT_0.ToString();

        saveData += GameManager_.Copper + Const.SPLIT_0.ToString();

        saveData += AudioManager_.BGIndex.ToString();

        System.IO.File.WriteAllText(string.Format(SAVE_PATH, args[0]), saveData);
    }

    /// <summary>
    /// 读取
    /// </summary>
    /// <param name="index">序号</param>
    private static void Load(string[] args)
    {
        if (System.IO.File.Exists(string.Format(SAVE_PATH, args[0])))
        {
            ToolsE.Log(nameof(Load) + " | " + args[0]);

            GameManager_.Trigger(GameEventType.Clear);

            string[] saveData = System.IO.File.ReadAllText(string.Format(SAVE_PATH, args[0])).Split(Const.SPLIT_0);
            int index = 0;

            LoadApply(GameEventType.RoleState, saveData[index++]);
            LoadApply(GameEventType.GOSwitch, saveData[index++]);
            LoadApply(GameEventType.Load2Player, saveData[index++]);
            LoadApply(GameEventType.ItemAdd, saveData[index++]);
            LoadApply(GameEventType.LeaderChange, saveData[index++]);
            LoadApply(GameEventType.MissionBegin, saveData[index++]);
            LoadApply(GameEventType.LocationChange, saveData[index++]);
            LoadApply(GameEventType.CopperAdd, saveData[index++]);
            LoadApply(GameEventType.BGPlay, saveData[index++]);


            static void LoadApply(GameEventType gameEventType, string saveData)
            {
                string[] tempSA = saveData.Split(Const.SPLIT_1);

                for (int i = 0; i != tempSA.Length; i++)
                    GameManager_.Trigger(gameEventType, tempSA[i].Split(Const.SPLIT_2));
            }
        }
        else GameManager_.NewGame();
    }
}