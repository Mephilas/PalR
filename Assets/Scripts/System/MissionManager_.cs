using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// 任务管理器
/// </summary>
public sealed class MissionManager_ : SingletonBase<MissionManager_>
{
    /// <summary>
    /// 主线任务，必须存在至少一个主线任务
    /// </summary>
    public static MissionData MainMission { get; private set; }

    /// <summary>
    /// 任务列表
    /// </summary>
    public static readonly List<MissionData> MissionList = new();

    private static readonly List<MissionData> _tempList = new();

    private static MissionData _tempM;

    /// <summary>
    /// 任务注册方法
    /// </summary>
    private static readonly Dictionary<MissionType, UnityAction<string[]>> MissionRegisterDic = new()
    {
        { MissionType.Dialogue, DialogueMissionHandle },
        { MissionType.Item, ItemMissionHandle },
        { MissionType.Monster, MonsterMissionHandle },
    };

    protected override void Awake()
    {
        base.Awake();

        GameManager_.Register(GameEventType.MissionBegin, MissionBegin);
        GameManager_.Register(GameEventType.MissionUpdate, (string[] missionData) => { MissionRegisterDic[missionData[0].S2E<MissionType>()](missionData); });
    }

    /// <summary>
    /// 任务启动
    /// </summary>
    private static void MissionBegin(string[] missionData)
    {
        _tempM = DataManager_.MissionDataArray[int.Parse(missionData[0])];
        ToolsE.Log("任务 " + _tempM.Name + " 启动");

        if (_tempM.IsMain) MainMission = _tempM;
        MissionList.Add(_tempM);
        _tempM.State = MissionState.InProgress;
        GameManager_.MissionTUpdate(_tempM.Description);
    }

    /// <summary>
    /// 任务完成
    /// </summary>
    private static void MissionComplete(MissionData missionData)
    {
        _tempList.Add(missionData);
        ToolsE.Log("任务 " + missionData.Name + " 完成");

        if (null != missionData.EventArray)
        {
            _tempM = missionData;
            _tempM.State = MissionState.Finish;
            GameManager_.TriggerAll(_tempM.EventArray);
        }
    }

    /// <summary>
    /// 任务列表整理
    /// </summary>
    private static void MissionListClean()
    {
        for (int i = 0; i != _tempList.Count; i++) MissionList.Remove(_tempList[i]);
    }

    /// <summary>
    /// 对话任务处理
    /// </summary>
    /// <param name="RequireArray">需求集合</param>
    private static void DialogueMissionHandle(string[] RequireArray)
    {
        for (int i = 0; i != MissionList.Count; i++)
        {
            if (MissionType.Dialogue == MissionList[i].Type && MissionList[i].RequireArray[0] == RequireArray[1])
            {
                MissionComplete(MissionList[i]);
            }
        }

        MissionListClean();
    }

    /// <summary>
    /// 物品任务处理
    /// </summary>
    /// <param name="argumentArray"></param>
    private static void ItemMissionHandle(string[] argumentArray)
    {

    }

    /// <summary>
    /// 怪物任务处理
    /// </summary>
    /// <param name="argumentArray"></param>
    private static void MonsterMissionHandle(string[] argumentArray)
    {

    }
}