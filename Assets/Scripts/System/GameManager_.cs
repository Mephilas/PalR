using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// 游戏管理器
/// </summary>
public sealed class GameManager_ : SingletonBase<GameManager_>
{
    /// <summary>
    /// 地图根节点
    /// </summary>
    public const string MAP_ROOT = "Map";

    /// <summary>
    /// 游戏事件处理器集合
    /// </summary>
    private static readonly Dictionary<GameEventType, UnityAction<string[]>> _geHandlerDic = new()
    {
        { GameEventType.Clear, Clear },
        { GameEventType.GOSwitch, (string[] data) => Root_.Instance.CGT(MAP_ROOT + Const.SPLIT_3 + data[1]).gameObject.SetActive(bool.Parse(data[0])) },
        { GameEventType.GOFade, (string[] data) => Root_.Instance.CGC<SpriteRenderer>(MAP_ROOT + Const.SPLIT_3 + data[2]).DOFade(float.Parse(data[0]), float.Parse(data[1])) },
        { GameEventType.TimeChange, TimeChange },
        { GameEventType.PlayerJoin, PlayerJoin },
        { GameEventType.PlayerLeave, PlayerLeave },
        { GameEventType.LeaderChange, LeaderChange },
        { GameEventType.RoleFollow, (string[] data) => RoleList[int.Parse(data[0])].RoleFollow(data) },
        { GameEventType.HPAdd, (string[] data) => RoleList[int.Parse(data[0])].Trigger(RoleEffectType.HPAdd, int.Parse(data[1])) },
        { GameEventType.MPAdd, (string[] data) => RoleList[int.Parse(data[0])].Trigger(RoleEffectType.MPAdd, int.Parse(data[1])) },
        { GameEventType.ItemUse, (string[] data) => RoleList[int.Parse(data[0])].ItemUse(data) },
        { GameEventType.ItemEquip, (string[] data) => RoleList[int.Parse(data[0])].ItemEquip(data) },
        { GameEventType.SkillLearn, (string[] data) => RoleList[int.Parse(data[0])].Trigger(RoleEffectType.SkillLearn, int.Parse(data[1])) },
        { GameEventType.SkillCast, (string[] data) => RoleList[int.Parse(data[0])].SkillCast(data) },
        { GameEventType.LocationChange, (string[] data) => LocationChange(int.Parse(data[0])) },
        { GameEventType.CopperAdd, (string[] data) => CopperAdd(int.Parse(data[0])) },
        { GameEventType.ItemAdd, ItemAdd },
        { GameEventType.Anim, (string[] data) => RoleList[int.Parse(data[0])].Anim(data) },
        { GameEventType.SpecialAnim, (string[] data) => RoleList[int.Parse(data[0])].SpecialAnim(data) },
        { GameEventType.RoleState, (string[] data) => RoleList[int.Parse(data[0])].RoleState(data) },
        { GameEventType.RoleTransfer, (string[] data) => RoleList[int.Parse(data[0])].RoleTransfer(data) },
        { GameEventType.RoleMove, (string[] data) => RoleList[int.Parse(data[0])].RoleMove(data) },
        { GameEventType.RoleRotate, (string[] data) => RoleList[int.Parse(data[0])].RoleRotate(data) },
        { GameEventType.RoleFade, (string[] data) => RoleList[int.Parse(data[0])].RoleFade(data) },
        { GameEventType.RoleGradual, (string[] data) => RoleList[int.Parse(data[0])].RoleGradual(data) },
        { GameEventType.RoleFlip, (string[] data) => RoleList[int.Parse(data[0])].RoleFlip(data) },
        { GameEventType.Skin, (string[] data) => RoleList[int.Parse(data[0])].Skin(data) },
        { GameEventType.Pseudonym, (string[] data) => RoleList[int.Parse(data[0])].Pseudonym(data) },
        { GameEventType.DialogueIndex, (string[] data) => RoleList[int.Parse(data[0])].DialogueIndex(data) },
        { GameEventType.Load2Player, (string[] data) => RoleList[int.Parse(data[0])].Load(data) }
    };

    /// <summary>
    /// 金钱计数刷新
    /// </summary>
    public static UnityAction<int> CopperTUpdate { get; set; }

    /// <summary>
    /// 任务介绍刷新
    /// </summary>
    public static UnityAction<string> MissionTUpdate { get; set; }

    /// <summary>
    /// 角色集合
    /// </summary>
    public static readonly List<Role> RoleList = new();

    /// <summary>
    /// 玩家集合
    /// </summary>
    public static readonly List<Role> PlayerList = new();
    
    /// <summary>
    /// 领队ID
    /// </summary>
    private static int _leaderID;

    /// <summary>
    /// 主角
    /// </summary>
    public static Role Leader { get { return RoleList[_leaderID]; } }

    /// <summary>
    /// 战场ID
    /// </summary>
    public static int BattleGroundID { get; private set; }

    /// <summary>
    /// 金钱
    /// </summary>
    public static int Copper { get; private set; }

    /// <summary>
    /// 背包
    /// </summary>
    public static readonly Dictionary<int, int> Bag = new();

    /// <summary>
    /// 是否游戏中
    /// </summary>
    public static bool InGame { get; set; }

    /// <summary>
    /// 日夜
    /// </summary>
    public static bool DayOrNight { get; private set; }

    /// <summary>
    /// 是新游戏哦
    /// </summary>
    public static void NewGame()
    {
        Trigger(GameEventType.VideoCG, "2");
        Trigger(GameEventType.MissionBegin, "0");
        Trigger(GameEventType.Curtain, "1", "1", "0");
        Trigger(GameEventType.GOFade, "0", "0", "YuHang/YuHangInnRoom");
        Trigger(GameEventType.RoleFade, "5", "0", "0");

        Clear(null);
        Trigger(GameEventType.PlayerJoin, (_leaderID = 0).ToString());

        /*Bag.Add(0, 88);
        Bag.Add(61, 2);
        Bag.Add(62, 2);*/
        for (int i = 0; i != DataManager_.ItemDataArray.Length; i++) Bag.Add(i, i + 1);

        if (1 == PlayerPrefs.GetInt(nameof(NewGame), 1))
        {
            PlayerPrefs.SetInt(nameof(NewGame), 0);
            Trigger(GameEventType.Save, nameof(NewGame));
        }
    }

    /// <summary>
    /// 清空
    /// </summary>
    /// <param name="data">数据</param>
    private static void Clear(string[] data)
    {
        for (int i = 0; i != RoleList.Count; i++) RoleList[i].DataInit();
        for (int i = 0; i != PlayerList.Count; i++) PlayerList[i].Leave();
        PlayerList.Clear();
        Bag.Clear();
        Copper = 0;
    }

    private static void PlayerJoin(string[] data)
    {
        Role player = RoleList[int.Parse(data[0])];
        player.PlayerSwitch(true);

        for (int i = 0; i != PlayerList.Count; i++)
        {
            if (player.RoleData.ID < PlayerList[i].RoleData.ID)
            {
                PlayerList.Insert(i, player);

                return;
            }
        }

        PlayerList.Add(RoleList[int.Parse(data[0])]);
    }

    private static void PlayerLeave(string[] data)
    {
        for (int i = 0; i != PlayerList.Count; i++)
        {
            if (PlayerList[i].RoleData.ID == int.Parse(data[0]))
            {
                PlayerList[i].Leave();

                PlayerList.Remove(PlayerList[i]);

                return;
            }
        }
    }

    private static void LeaderChange(string[] data)
    {
        PlayerList[_leaderID].MovementSwitch(false);
        PlayerList[_leaderID = int.Parse(data[0])].MovementSwitch(true);

        for (int i = 0; i != PlayerList.Count; i++)
        {
            Trigger(GameEventType.RoleFollow, PlayerList[i].RoleData.ID.ToString(), (_leaderID != PlayerList[i].RoleData.ID).ToString());
        }
    }

    private static void LocationChange(int locationID) => BattleGroundID = locationID;

    public static bool CopperAdd(int count)
    {
        //ToolsE.LogWarning("Copper add : " + count);
        
        if (count < 0 && Copper + count < 0) return false;
        else
        {
            Copper += count;

            if (Copper < 0) Copper = 0;  //防止改钱溢出

            CopperTUpdate(Copper);

            return true;
        }
    }

    private static void ItemAdd(string[] data)
    {
        int itemID = int.Parse(data[0]), itemCount = 1 == data.Length ? 1 : int.Parse(data[1]);
        //ToolsE.LogWarning("Add : " + DataManager_.ItemDataArray[itemID].Name + " " + itemCount);
        ToolsE.Assert(0 != itemCount, "");
        if (0 < itemCount)
        {
            if (Bag.ContainsKey(itemID))
            {
                if (ItemData.PILE_COUNT < (Bag[itemID] += itemCount))
                    Bag[itemID] = ItemData.PILE_COUNT;
            }
            else Bag.Add(itemID, itemCount);
        }
        else
        {
            if (0 == (Bag[itemID] += itemCount))
                Bag.Remove(itemID);
        }
    }

    private static void TimeChange(string[] data)
    {
        DayOrNight = bool.Parse(data[1]);

        Role[] roleArray = Root_.Instance.CGT(nameof(Role)).GetComponentsInChildren<Role>();
        for (int i = 0; i != roleArray.Length; i++)
            roleArray[i].TimeChange(DayOrNight);

        Transform cityT = Root_.Instance.CGT(MAP_ROOT + Const.SPLIT_3 + data[0]);
        SpriteRenderer spriteRenderer;
        for (int i = 0; i != 999; i++)
        {
            if (null == (spriteRenderer = cityT.GetChild(i).GetComponent<SpriteRenderer>())) return;
            else
                spriteRenderer.DOColor(DayOrNight ? Color.white : Const.NIGHT_COLOR, Const.TIME_CHANGE_DURATION);
        }
    }

    /// <summary>
    /// 游戏事件处理器注册
    /// </summary>
    /// <param name="gameEvent">游戏事件</param>
    /// <param name="eventHandler">事件处理器</param>
    public static void Register(GameEventType gameEvent, UnityAction<string[]> eventHandler) => _geHandlerDic.Add(gameEvent, eventHandler);

    /// <summary>
    /// 测试优化事件触发，数组参数与可变参数性能测试
    /// </summary>
    /// <param name="gameEventType">类型</param>
    /// <param name="argumentArray">参数集合</param>
    public static void Trigger(GameEventType gameEventType, params string[] argumentArray)
    {
        if (null != argumentArray && 0 != argumentArray.Length) ToolsE.Log(gameEventType + "  " + argumentArray.SA2S());
        _geHandlerDic[gameEventType](argumentArray);
    }

    /// <summary>
    /// 游戏事件触发
    /// </summary>
    /// <param name="gameEvent">事件</param>
    public static void Trigger(GameEventData gameEvent)
    {
        if (null != gameEvent.ArgumentArray && 0 != gameEvent.ArgumentArray.Length)
            ToolsE.Log(gameEvent.GameEventType + "  " + gameEvent.ArgumentArray.SA2S());

        _geHandlerDic[gameEvent.GameEventType](gameEvent.ArgumentArray);
    }

    /// <summary>
    /// 游戏事件触发
    /// </summary>
    /// <param name="gameEventArray">事件集合</param>
    public static void TriggerAll(GameEventData[] gameEventArray)
    {
        for (int i = 0; i != gameEventArray.Length; i++)
            Trigger(gameEventArray[i].GameEventType, gameEventArray[i].ArgumentArray);
    }
}


/// <summary>
/// 游戏事件数据
/// </summary>
public sealed class GameEventData
{
    /// <summary>
    /// 类型
    /// </summary>
    public readonly GameEventType GameEventType;

    /// <summary>
    /// 参数集合
    /// </summary>
    public readonly string[] ArgumentArray;

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="gameEventType">类型</param>
    /// <param name="argumentArray">参数集合</param>
    public GameEventData(GameEventType gameEventType, params string[] argumentArray)
    {
        GameEventType = gameEventType;
        ArgumentArray = argumentArray;
    }

    public override string ToString() => GameEventType + " : " + ArgumentArray.SA2S();
}


/// <summary>
/// 游戏事件类型
/// </summary>
public enum GameEventType
{
    /// <summary>
    /// 视频CG播放
    /// </summary>
    VideoCG,

    /// <summary>
    /// 序列帧CG播放
    /// </summary>
    SequenceCG,

    /// <summary>
    /// BG播放
    /// </summary>
    BGPlay,

    /// <summary>
    /// 按钮音效
    /// </summary>
    ButtonAudio,

    /// <summary>
    /// 音效
    /// </summary>
    SoundEffects,

    /// <summary>
    /// 宝藏音效
    /// </summary>
    VaultSoundEffects,

    /// <summary>
    /// BG恢复
    /// </summary>
    BGRecover,

    /// <summary>
    /// 摄像机跟随
    /// </summary>
    CameraFollow,

    /// <summary>
    /// 摄像机移至玩家
    /// </summary>
    CameraMove2Leader,

    /// <summary>
    /// 摄像机移动
    /// </summary>
    CameraMove,

    /// <summary>
    /// 摄像机缩放
    /// </summary>
    CameraScale,

    /// <summary>
    /// 摄像机动画
    /// </summary>
    CameraRotate,

    /// <summary>
    /// 任务启动
    /// </summary>
    MissionBegin,

    /// <summary>
    /// 任务更新
    /// </summary>
    MissionUpdate,

    /// <summary>
    /// UI面板切换
    /// </summary>
    UIPanel,

    /// <summary>
    /// UI面板返回
    /// </summary>
    UIPanelReturn,

    /// <summary>
    /// 幕布切换
    /// </summary>
    Curtain,

    /// <summary>
    /// 台词播放
    /// </summary>
    Dialogue,

    /// <summary>
    /// 提示
    /// </summary>
    Tip,

    /// <summary>
    /// 动画
    /// </summary>
    Anim,

    /// <summary>
    /// 特殊动画
    /// </summary>
    SpecialAnim,

    /// <summary>
    /// 玩家入队
    /// </summary>
    PlayerJoin,

    /// <summary>
    /// 玩家离队
    /// </summary>
    PlayerLeave,

    /// <summary>
    /// 领队修改
    /// </summary>
    LeaderChange,

    /// <summary>
    /// 角色跟随开关
    /// </summary>
    RoleFollow,

    /// <summary>
    /// 角色状态修改
    /// </summary>
    RoleState,

    /// <summary>
    /// 角色转移
    /// </summary>
    RoleTransfer,

    /// <summary>
    /// 角色移动
    /// </summary>
    RoleMove,

    /// <summary>
    /// 角色转动
    /// </summary>
    RoleRotate,

    /// <summary>
    /// 角色渐隐
    /// </summary>
    RoleFade,

    /// <summary>
    /// 角色渐变
    /// </summary>
    RoleGradual,

    /// <summary>
    /// 角色翻转
    /// </summary>
    RoleFlip,

    /// <summary>
    /// 选择
    /// </summary>
    Choose,

    /// <summary>
    /// 体力增加
    /// </summary>
    HPAdd,

    /// <summary>
    /// 法力增加
    /// </summary>
    MPAdd,

    /// <summary>
    /// 仙术学习
    /// </summary>
    SkillLearn,

    /// <summary>
    /// 仙术施放
    /// </summary>
    SkillCast,

    /// <summary>
    /// 位置修改，战斗
    /// </summary>
    LocationChange,

    /// <summary>
    /// 金钱增加
    /// </summary>
    CopperAdd,

    /// <summary>
    /// 物品加减 a bottle of water
    /// </summary>
    ItemAdd,

    /// <summary>
    /// 物品使用
    /// </summary>
    ItemUse,

    /// <summary>
    /// 物品装备
    /// </summary>
    ItemEquip,

    /// <summary>
    /// 战斗
    /// </summary>
    Battle,

    /// <summary>
    /// 物体开关
    /// </summary>
    GOSwitch,

    /// <summary>
    /// 物体隐现
    /// </summary>
    GOFade,

    /// <summary>
    /// 皮肤更新
    /// </summary>
    Skin,

    /// <summary>
    /// 时间
    /// </summary>
    TimeChange,

    /// <summary>
    /// 假名更新
    /// </summary>
    Pseudonym,

    /// <summary>
    /// 台词序号更新
    /// </summary>
    DialogueIndex,

    /// <summary>
    /// 存档
    /// </summary>
    Save,

    /// <summary>
    /// 读档
    /// </summary>
    Load,

    /// <summary>
    /// 读档玩家数据
    /// </summary>
    Load2Player,

    /// <summary>
    /// 清空游戏数据
    /// </summary>
    Clear,
}