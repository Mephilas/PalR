using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 数据基类
/// </summary>
public abstract class DataBase
{
    /// <summary>
    /// 数字ID
    /// </summary>
    public readonly int ID;

    /// <summary>
    /// 全长ID
    /// </summary>
    public string FullID { get; private set; }

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="data">数据</param>
    public DataBase(string[] data) => FullID = (ID = int.Parse(data[0])).ToString();

    /// <summary>
    /// 全长ID补齐
    /// </summary>
    /// <param name="times">倍率</param>
    protected void FullIDCompletion(FullIDTimes times)
    {
        if (ID < (int)times)
        {
            int count = (int)Mathf.Log10((int)times);
            if (0 != ID) count -= (int)Mathf.Log10(ID);

            for (int i = 0; i != count; i++)
                FullID = "0" + FullID;
        }
    }

    /// <summary>
    /// 全长ID补齐倍数
    /// </summary>
    protected enum FullIDTimes
    {
        Tens = 10,
        Hundreds = 100,
        Thousands = 1000,
        TenThousands = 10000
    }
}


/// <summary>
/// 任务数据
/// </summary>
public sealed class MissionData : DataBase
{
    //Property
    #region
    /// <summary>
    /// 是否主线
    /// </summary>
    public readonly bool IsMain;

    /// <summary>
    /// 名称 除妖
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// 说明 击败罗煞鬼婆
    /// </summary>
    public readonly string Description;

    /// <summary>
    /// 任务类型
    /// </summary>
    public readonly MissionType Type;

    /// <summary>
    /// 需求集合
    /// </summary>
    public readonly string[] RequireArray;

    /// <summary>
    /// 事件集合
    /// </summary>
    public readonly GameEventData[] EventArray;

    /// <summary>
    /// 任务状态
    /// </summary>
    public MissionState State { get; set; }
    #endregion

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="data">数据</param>
    public MissionData(string[] data) : base(data)
    {
        int index = 1;

        IsMain = bool.Parse(data[index++]);
        Name = data[index++];
        Description = data[index++];
        Type = data[index++].S2E<MissionType>();
        RequireArray = data[index++].Split(Const.SPLIT_1);

        if (data.Length != index)
        {
            EventArray = new GameEventData[data.Length - index];

            for (int i = 0; i != EventArray.Length; i++)
            {
                EventArray[i] = data[i + index].S2GE();
            }
        }
    }
}


/// <summary>
/// 任务类型
/// </summary>
public enum MissionType
{
    /// <summary>
    /// 台词
    /// </summary>
    Dialogue,

    /// <summary>
    /// 物品
    /// </summary>
    Item,

    /// <summary>
    /// 怪物
    /// </summary>
    Monster,
}


/// <summary>
/// 任务状态
/// </summary>
public enum MissionState
{
    /// <summary>
    /// 未激活
    /// </summary>
    Inactive,

    /// <summary>
    /// 激活
    /// </summary>
    Active,

    /// <summary>
    /// 进行中
    /// </summary>
    InProgress,

    /// <summary>
    /// 完成
    /// </summary>
    Complete,

    /// <summary>
    /// 结束
    /// </summary>
    Finish
}


/// <summary>
/// 角色数据
/// </summary>
public sealed class RoleData : DataBase
{
    //Property
    #region
    /// <summary>
    /// 最大经验值
    /// </summary>
    private const int MAX_EXPERIENCE = 32000;

    /// <summary>
    /// 经验需求集合
    /// </summary>
    public static readonly int[] EXPERIENCE_REQUIRE_ARRAY = new int[Role.MAX_LEVEL];

    //分离成战法坦职业后分别放入职业数据表格中读取

    /// <summary>
    /// 体力成长
    /// </summary>
    public const int HP_GROW_MIN = 10,
                     HP_GROW_MAX = 17;

    /// <summary>
    /// 真气成长
    /// </summary>
    public const int MP_GROW_MIN = 8,
                     MP_GROW_MAX = 13;

    /// <summary>
    /// 武术成长
    /// </summary>
    public const int ATTACK_GROW_MIN = 4,
                     ATTACK_GROW_MAX = 5;

    /// <summary>
    /// 灵力成长
    /// </summary>
    public const int MAGIC_GROW_MIN = 4,
                     MAGIC_GROW_MAX = 5;

    /// <summary>
    /// 防御成长
    /// </summary>
    public const int DEFENSE_GROW_MIN = 2,
                     DEFENSE_GROW_MAX = 3;

    /// <summary>
    /// 身法成长
    /// </summary>
    public const int SPEED_GROW_MIN = 2,
                     SPEED_GROW_MAX = 3;

    /// <summary>
    /// 吉运成长
    /// </summary>
    public const int LUCK_GROW_MIN = 2,
                     LUCK_GROW_MAX = 2;

    /// <summary>
    /// 动画集合
    /// </summary>
    private readonly List<Dictionary<InputType, Sprite[]>> AnimList = new();

    /// <summary>
    /// 当前动画集合
    /// </summary>
    public Dictionary<InputType, Sprite[]> CurrentAnimDic { get { return AnimList[SkinIndex]; } }

    /// <summary>
    /// 特殊动画集合
    /// </summary>
    public readonly Dictionary<SpecialAnimType, Sprite[]> SpecialAnimDic = new();

    /// <summary>
    /// 头像集合
    /// </summary>
    public readonly Dictionary<ExpressionType, Sprite> ProfilePictureDic = new();

    /// <summary>
    /// 状态事件集合
    /// </summary>
    public readonly GameEventData[][] StateEventArray;

    /// <summary>
    /// 名字
    /// </summary>
    public string Name { get { return string.IsNullOrEmpty(Pseudonym) ? _name : Pseudonym; } }

    /// <summary>
    /// 名字
    /// </summary>
    private readonly string _name;

    /// <summary>
    /// 假名
    /// </summary>
    public string Pseudonym { get; set; }

    /// <summary>
    /// 皮肤序号
    /// </summary>
    public int SkinIndex { get; set; }

    /// <summary>
    /// 台词序号
    /// </summary>
    public int DialogueIndex { get; set; } = -1;

    /// <summary>
    /// 等级学习技能集合
    /// </summary>
    public readonly Dictionary<int, int> LevelSkillDic = new();

    /// <summary>
    /// 基础体力
    /// </summary>
    public readonly int HPBase;

    /// <summary>
    /// 基础真气
    /// </summary>
    public readonly int MPBase;

    /// <summary>
    /// 基础武术
    /// </summary>
    public readonly int AttackBase;

    /// <summary>
    /// 基础灵力
    /// </summary>
    public readonly int MagicBase;

    /// <summary>
    /// 基础防御
    /// </summary>
    public readonly int DefenseBase;

    /// <summary>
    /// 基础身法
    /// </summary>
    public readonly int SpeedBase;

    /// <summary>
    /// 基础吉运
    /// </summary>
    public readonly int LuckBase;

    /// <summary>
    /// 战斗角色ID集合
    /// </summary>
    public readonly int[] BattleIDGroup;

    /// <summary>
    /// 默认装备
    /// </summary>
    public readonly int[] DefaultOutfit;

    /// <summary>
    /// 默认仙术
    /// </summary>
    public readonly int[] DefaultSkill;

    /// <summary>
    /// 出售物品
    /// </summary>
    public readonly int[] SellItem;  //拆分为单人多商店并带有购买条件

    /// <summary>
    /// 飞龙探云手掉落
    /// </summary>
    public readonly int[] StealItem;

    /// <summary>
    /// 击败掉落
    /// </summary>
    public readonly int[] DefeatItem;
    #endregion

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="data">数据</param>
    public RoleData(string[] data) : base(data)
    {
        FullIDCompletion(FullIDTimes.Hundreds);

        for (int i = 0; i != 99; i++)
        {
            if (null != Application.dataPath + "/Resources/Role/" + FullID + "/Skin" + i + "/" + InputType.Up + "/0.bmp")
            {
                Dictionary<InputType, Sprite[]> tempAnimDic = new();

                foreach (InputType keyCode in Role.MOVE_INPUT_DIC.Keys)
                    tempAnimDic.Add(keyCode, Resources.LoadAll<Sprite>("Role/" + FullID + "/Skin" + i.ToString() + "/" + keyCode));

                AnimList.Add(tempAnimDic);
            }
            else break;
        }

        foreach (SpecialAnimType specialAnimType in Enum.GetValues(typeof(SpecialAnimType)))
            SpecialAnimDic.Add(specialAnimType, Resources.LoadAll<Sprite>("Role/" + FullID + "/Special/" + specialAnimType));

        Sprite[] tempSPA = Resources.LoadAll<Sprite>("Role/" + FullID + "/ProfilePicture");
        for (int i = 0; i != tempSPA.Length; i++)
            ProfilePictureDic.Add((ExpressionType)i, tempSPA[i]);

        int index = 1;
        _name = data[index++];

        if (!string.IsNullOrEmpty(data[index++]))
        {
            int[] tempIA = ToolsE.SA2IA(data[index - 1].Split(Const.SPLIT_1));

            for (int i = 0; i != EXPERIENCE_REQUIRE_ARRAY.Length; i++)
                EXPERIENCE_REQUIRE_ARRAY[i] = i < tempIA.Length ? tempIA[i] : MAX_EXPERIENCE;

            EXPERIENCE_REQUIRE_ARRAY[0] = int.MinValue;
        }

        if (!string.IsNullOrEmpty(data[index++])) LevelSkillDic.S2DI(data[index - 1]);

        if (string.IsNullOrEmpty(data[index]))
        {
            index += 12;  //无战斗数据略过长度
        }
        else
        {
            HPBase = int.Parse(data[index++]);
            MPBase = int.Parse(data[index++]);
            AttackBase = int.Parse(data[index++]);
            MagicBase = int.Parse(data[index++]);
            DefenseBase = int.Parse(data[index++]);
            SpeedBase = int.Parse(data[index++]);
            LuckBase = int.Parse(data[index++]);
            if (!string.IsNullOrEmpty(data[index++])) BattleIDGroup = ToolsE.S2IA(data[index - 1]);
            if (!string.IsNullOrEmpty(data[index++])) DefaultOutfit = ToolsE.S2IA(data[index - 1]);
            DefaultSkill = ToolsE.S2IA(data[index++]);
            if (!string.IsNullOrEmpty(data[index++])) StealItem = ToolsE.S2IA(data[index - 1]);
            if (!string.IsNullOrEmpty(data[index++])) DefeatItem = ToolsE.S2IA(data[index - 1]);
        }

        if (!string.IsNullOrEmpty(data[index++])) SellItem = ToolsE.S2IA(data[index - 1]);

        StateEventArray = new GameEventData[data.Length - index][];
        string[] tempSA;
        for (int i = 0; i != StateEventArray.Length; i++)
        {
            tempSA = data[index + i].Split(Const.SPLIT_1);

            StateEventArray[i] = new GameEventData[tempSA.Length];
            for (int j = 0; j != StateEventArray[i].Length; j++)
                StateEventArray[i][j] = tempSA[j].S2GE(Const.SPLIT_2, Const.SPLIT_3);
        }
    }
}


/// <summary>
/// 物品数据
/// </summary>
public sealed class ItemData : DataBase
{
    /// <summary>
    /// 堆叠数量
    /// </summary>
    public const int PILE_COUNT = 99;

    /// <summary>
    /// 图标
    /// </summary>
    public readonly Sprite Icon;

    /// <summary>
    /// 名称
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// 描述
    /// </summary>
    public readonly string Description;

    /// <summary>
    /// 剧情道具
    /// </summary>
    public readonly bool StoryItem;

    /// <summary>
    /// 消耗
    /// </summary>
    public readonly bool Reduce = true;

    /// <summary>
    /// 是否装备
    /// </summary>
    public readonly bool OutfitOrConsumable;

    /// <summary>
    /// 装备部位
    /// </summary>
    public readonly OutfitType OutfitType;

    /// <summary>
    /// 角色需求
    /// </summary>
    public readonly int[] RoleRequireArray;

    /// <summary>
    /// 等级需求
    /// </summary>
    public readonly int LevelRequire;

    /// <summary>
    /// 买价
    /// </summary>
    public readonly int BoughtPrice;

    /// <summary>
    /// 售价
    /// </summary>
    public readonly int SellPrice;

    /// <summary>
    /// 可卖出
    /// </summary>
    public bool CanSell { get { return 0 == SellPrice; } }

    /// <summary>
    /// 物品效果集合
    /// </summary>
    public readonly Dictionary<RoleEffectType, int> EventDic = new();

    public ItemData(string[] data) : base(data)
    {
        FullIDCompletion(FullIDTimes.Hundreds);
        Icon = Resources.Load<Sprite>("Item/" + FullID);

        int index = 1;
        Name = data[index++];
        Description = data[index++].Replace(Const.SPLIT_P, Const.SPLIT_N);
        if (!string.IsNullOrEmpty(data[index++])) StoryItem = bool.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) Reduce = bool.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) OutfitOrConsumable = bool.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) OutfitType = data[index - 1].S2E<OutfitType>();
        if (!string.IsNullOrEmpty(data[index++])) RoleRequireArray = ToolsE.S2IA(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) LevelRequire = int.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) BoughtPrice = int.Parse(data[index - 1]);
        SellPrice = int.Parse(data[index++]);

        int count = data.Length - index;
        string[] tempSA;
        for (int i = 0; i != count; i++)
        {
            tempSA = data[index + i].Split(Const.SPLIT_1);

            EventDic.Add(tempSA[0].S2E<RoleEffectType>(), int.Parse(tempSA[1]));
        }
    }
}


/// <summary>
/// 装备部位
/// </summary>
public enum OutfitType
{
    /// <summary>
    /// 头盔
    /// </summary>
    Casque,

    /// <summary>
    /// 披风
    /// </summary>
    Cape,

    /// <summary>
    /// 护甲
    /// </summary>
    Armor,

    /// <summary>
    /// 武器
    /// </summary>
    Weapon,

    /// <summary>
    /// 靴子
    /// </summary>
    Boots,

    /// <summary>
    /// 护腕
    /// </summary>
    Bracers
}


/// <summary>
/// 角色效果类型
/// </summary>
public enum RoleEffectType
{
    /// <summary>
    /// 台词
    /// </summary>
    Dialogue,

    /// <summary>
    /// 经验增加
    /// </summary>
    ExperienceAdd,

    /// <summary>
    /// 等级增加
    /// </summary>
    LevelAdd,

    /// <summary>
    /// 仙术学习
    /// </summary>
    SkillLearn,

    /// <summary>
    /// Buff添加
    /// </summary>
    BuffAdd,

    /// <summary>
    /// 体力增加
    /// </summary>
    HPAdd,

    /// <summary>
    /// 体力上限增加
    /// </summary>
    HPMaxAdd,

    /// <summary>
    /// 真气增加
    /// </summary>
    MPAdd,

    /// <summary>
    /// 真气上限增加
    /// </summary>
    MPMaxAdd,

    /// <summary>
    /// 武术增加
    /// </summary>
    AttackAdd,

    /// <summary>
    /// 灵力增加
    /// </summary>
    MagicAdd,

    /// <summary>
    /// 防御增加
    /// </summary>
    DefenseAdd,

    /// <summary>
    /// 身法增加
    /// </summary>
    SpeedAdd,

    /// <summary>
    /// 吉运增加
    /// </summary>
    LuckAdd
}


/// <summary>
/// 仙术数据
/// </summary>
public sealed class SkillData : DataBase
{
    /// <summary>
    /// 特效集合
    /// </summary>
    public readonly Sprite[] EffectArray;

    /// <summary>
    /// 名称
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// 描述
    /// </summary>
    public readonly string Description;

    /// <summary>
    /// 单/群体
    /// </summary>
    public readonly bool IsSingle;

    /// <summary>
    /// 真气消耗
    /// </summary>
    public readonly int Cost;

    /// <summary>
    /// 数值
    /// </summary>
    public readonly int Value;

    /// <summary>
    /// Buff ID
    /// </summary>
    public readonly int BuffID;

    /// <summary>
    /// Buff添加成功率
    /// </summary>
    public readonly uint SuccessRate;

    public SkillData(string[] data) : base(data)
    {
        FullIDCompletion(FullIDTimes.Tens);
        EffectArray = Resources.LoadAll<Sprite>("Skill/" + FullID);

        int index = 1;
        Name = data[index++];
        Description = data[index++].Replace(Const.SPLIT_P, Const.SPLIT_N);
        IsSingle = bool.Parse(data[index++]);
        Cost = int.Parse(data[index++]);
        Value = int.Parse(data[index++]);
    }
}


/// <summary>
/// 台词数据
/// </summary>
public sealed class DialogueData : DataBase
{
    //Property
    #region
    /// <summary>
    /// 自动播放
    /// </summary>
    public readonly bool AutoPlay;

    /// <summary>
    /// 对话人
    /// </summary>
    private readonly int _speaker = -1;

    /// <summary>
    /// 对话人
    /// </summary>
    public RoleData Speaker { get { return -1 == _speaker ? null : DataManager_.RoleDataArray[_speaker]; } }

    /// <summary>
    /// 台词头像类型
    /// </summary>
    public readonly ExpressionType ExpressionType = ExpressionType.Nil;

    /// <summary>
    /// 台词文本
    /// </summary>
    public readonly string DialogueText;

    /// <summary>
    /// 开始事件
    /// </summary>
    public readonly GameEventData StartEvent;

    /// <summary>
    /// 开始事件
    /// </summary>
    public readonly GameEventData StartEvent_;

    /// <summary>
    /// 结束事件集合
    /// </summary>
    public readonly GameEventData[] EventArray;
    #endregion

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="data">数据</param>
    public DialogueData(string[] data) : base(data)
    {
        int index = 1;

        AutoPlay = bool.Parse(data[index++]);
        if (!string.IsNullOrEmpty(data[index++])) _speaker = int.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) ExpressionType = data[index - 1].S2E<ExpressionType>();
        if (!string.IsNullOrEmpty(data[index++])) StartEvent = data[index - 1].S2GE();
        if (!string.IsNullOrEmpty(data[index++])) StartEvent_ = data[index - 1].S2GE();
        DialogueText = data[index++].Replace(Const.SPLIT_P, Const.SPLIT_N);

        if (data.Length != index)
        {
            EventArray = new GameEventData[data.Length - index];

            for (int i = 0; i != EventArray.Length; i++)
                EventArray[i] = data[i + index].S2GE();
        }
    }
}


/// <summary>
/// 表情类型
/// </summary>
public enum ExpressionType
{
    /// <summary>
    /// 让一切都归于无吧...
    /// </summary>
    Nil = -1,

    /// <summary>
    /// 普通
    /// </summary>
    Normal,

    /// <summary>
    /// 微笑
    /// </summary>
    Smile,

    /// <summary>
    /// 你们这样子是不行的！我今天算是得罪了你们一下！
    /// </summary>
    Angry,

    /// <summary>
    /// 悲伤
    /// </summary>
    Sad,
}


/// <summary>
/// 提示数据
/// </summary>
public sealed class TipData : DataBase
{
    /// <summary>
    /// 提示文本
    /// </summary>
    public readonly string TipText;

    /// <summary>
    /// 事件集合
    /// </summary>
    public readonly GameEventData[] EventArray;

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="data">数据</param>
    public TipData(string[] data) : base(data)
    {
        int index = 1;

        TipText = data[index++].Replace(Const.SPLIT_P, Const.SPLIT_N);

        if (data.Length != index)
        {
            EventArray = new GameEventData[data.Length - index];
            for (int i = 0; i != EventArray.Length; i++)
                EventArray[i] = data[i + index].S2GE();
        }
    }
}


/// <summary>
/// 选择数据
/// </summary>
public sealed class ChooseData : DataBase
{
    /// <summary>
    /// 接受事件集合
    /// </summary>
    public readonly GameEventData[] AcceptEventArray;

    /// <summary>
    /// 拒绝事件集合
    /// </summary>
    public readonly GameEventData[] RefuseEventArray;

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="data">数据</param>
    public ChooseData(string[] data) : base(data)
    {
        int index = 1;

        AcceptEventArray = data[index++].S2GA();
        RefuseEventArray = data[index++].S2GA();
    }
}