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
    public DataBase(params string[] data) => FullID = (ID = int.Parse(data[0])).ToString();

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
    public MissionData(params string[] data) : base(data)
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
    /// 战斗动画集合
    /// </summary>
    private readonly List<Dictionary<RoleBattleState, Sprite[]>> BattleAnimList = new();

    /// <summary>
    /// 当前战斗动画集合
    /// </summary>
    public Dictionary<RoleBattleState, Sprite[]> CurrentBattleAnimDic { get { return BattleAnimList[SkinIndex]; } }

    /// <summary>
    /// 头像集合
    /// </summary>
    public readonly Dictionary<ExpressionType, Sprite> ProfilePictureDic = new();

    /// <summary>
    /// 近战音效
    /// </summary>
    public readonly AudioClip MeleeAudio;

    /// <summary>
    /// 暴击音效
    /// </summary>
    public readonly AudioClip CriticalAudio;

    /// <summary>
    /// 远程音效
    /// </summary>
    public readonly AudioClip RangedAudio;

    /// <summary>
    /// 死亡音效
    /// </summary>
    public readonly AudioClip DeceaseAudio;

    /// <summary>
    /// 当前近战音效关键帧
    /// </summary>
    public int CurrentMeleeAudioKeyFrame { get { return MeleeKeyFrameArray[SkinIndex][0]; } }

    /// <summary>
    /// 当前近战移动关键帧
    /// </summary>
    public int CurrentMeleeMoveKeyFrame { get { return MeleeKeyFrameArray[SkinIndex][1]; } }

    /// <summary>
    /// 当前近战判定关键帧
    /// </summary>
    public int CurrentMeleeAttackKeyFrame { get { return MeleeKeyFrameArray[SkinIndex][2]; } }

    /// <summary>
    /// 当前远程音效关键帧
    /// </summary>
    public int CurrentRangedAudioKeyFrame { get { return RangedAudioKeyFrameArray[SkinIndex]; } }

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
    /// 等级学习仙术集合
    /// </summary>
    public readonly Dictionary<int, int> LevelLearnSkillDic = new();

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
    /// 暴击
    /// </summary>
    public readonly int Critical;

    /// <summary>
    /// 战斗音乐
    /// </summary>
    public readonly string BattleBG;

    /// <summary>
    /// 战斗角色ID集合
    /// </summary>
    public readonly int[] BattleIDGroup;

    /// <summary>
    /// 战斗台词
    /// </summary>
    public readonly int BattleDialogue = -1;

    /// <summary>
    /// 击败台词
    /// </summary>
    public readonly int BeatDialogue = -1;

    /// <summary>
    /// 默认装备
    /// </summary>
    public readonly int[] DefaultOutfit;

    /// <summary>
    /// 默认仙术
    /// </summary>
    public readonly int[] DefaultSkill;

    /// <summary>
    /// 二次行动
    /// </summary>
    public readonly bool DoubleKill;

    /// <summary>
    /// 普攻附带Buff概率
    /// </summary>
    public readonly int BuffChance;

    /// <summary>
    /// Buff ID
    /// </summary>
    public readonly int BuffID;

    /// <summary>
    /// 召唤ID，与自身相同则分裂
    /// </summary>
    public readonly int Summon;

    /// <summary>
    /// 变身ID
    /// </summary>
    public readonly int HenShin;

    /// <summary>
    /// 合击仙术
    /// </summary>
    public readonly int JointSkillID;

    /// <summary>
    /// 飞龙探云手掉落
    /// </summary>
    public readonly int[] StealItem;

    /// <summary>
    /// 近战关键帧集合
    /// </summary>
    public readonly int[][] MeleeKeyFrameArray;

    /// <summary>
    /// 远程音效关键帧集合
    /// </summary>
    public readonly int[] RangedAudioKeyFrameArray;

    /// <summary>
    /// 基础物理抗性
    /// </summary>
    public readonly int PhiResistanceBase;

    /// <summary>
    /// 基础风抗性
    /// </summary>
    public readonly int WindResistanceBase;

    /// <summary>
    /// 基础雷抗性
    /// </summary>
    public readonly int ThunderResistanceBase;

    /// <summary>
    /// 基础水抗性
    /// </summary>
    public readonly int WaterResistanceBase;

    /// <summary>
    /// 基础火抗性
    /// </summary>
    public readonly int FireResistanceBase;

    /// <summary>
    /// 基础土抗性
    /// </summary>
    public readonly int SoilResistanceBase;

    /// <summary>
    /// 基础毒抗性
    /// </summary>
    public readonly int PoisonResistanceBase;

    /// <summary>
    /// 基础巫术抗性
    /// </summary>
    public readonly int WitcheryResistanceBase;

    /// <summary>
    /// 击败经验
    /// </summary>
    public readonly int BeatExperience;

    /// <summary>
    /// 击败掉落
    /// </summary>
    public readonly int[] BeatItemArray;

    /// <summary>
    /// 出售物品
    /// </summary>
    public readonly int[] SellItem;  //拆分为单人多商店并带有购买条件
    #endregion

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="data">数据</param>
    public RoleData(params string[] data) : base(data)
    {
        FullIDCompletion(FullIDTimes.Hundreds);

        for (int i = 0; i != 99; i++)
        {
            if (null != Application.dataPath + "/Resources/Role/" + FullID + "/Skin" + i + "/" + InputType.Up + "/0.bmp")
            {
                Dictionary<InputType, Sprite[]> tempAnimDic = new();
                foreach (InputType keyCode in Role.MOVE_INPUT_DIC.Keys)
                    tempAnimDic.Add(keyCode, Resources.LoadAll<Sprite>("Role/" + FullID + "/Skin" + i + "/" + keyCode));

                AnimList.Add(tempAnimDic);

                Dictionary<RoleBattleState, Sprite[]> tempBattleAnimDic = new();
                Array battleAnimArray = Enum.GetValues(typeof(RoleBattleState));
                foreach (RoleBattleState battleAnimType in battleAnimArray)
                {
                    Sprite[] animArray = Resources.LoadAll<Sprite>("Role/" + FullID + "/Skin" + i + "/" + battleAnimType);

                    if (0 != animArray.Length)
                        tempBattleAnimDic.Add(battleAnimType, animArray);
                }

                BattleAnimList.Add(tempBattleAnimDic);
            }
            else break;
        }

        Array specialAnimArray = Enum.GetValues(typeof(SpecialAnimType));
        foreach (SpecialAnimType specialAnimType in specialAnimArray)
            SpecialAnimDic.Add(specialAnimType, Resources.LoadAll<Sprite>("Role/" + FullID + "/Special/" + specialAnimType));

        Sprite[] tempSPA = Resources.LoadAll<Sprite>("Role/" + FullID + "/ProfilePicture");
        for (int i = 0; i != tempSPA.Length; i++)
            ProfilePictureDic.Add((ExpressionType)i, tempSPA[i]);

        MeleeAudio = Resources.Load<AudioClip>("Role/" + FullID + "SoundEffect/" + nameof(MeleeAudio));
        CriticalAudio = Resources.Load<AudioClip>("Role/" + FullID + "SoundEffect/" + nameof(CriticalAudio));
        RangedAudio = Resources.Load<AudioClip>("Role/" + FullID + "SoundEffect/" + nameof(RangedAudio));
        DeceaseAudio = Resources.Load<AudioClip>("Role/" + FullID + "SoundEffect/" + nameof(DeceaseAudio));

        int index = 1;
        _name = data[index++];

        if (!string.IsNullOrEmpty(data[index++]))
        {
            int[] tempIA = data[index - 1].Split(Const.SPLIT_1).SA2IA();

            for (int i = 0; i != EXPERIENCE_REQUIRE_ARRAY.Length; i++)
                EXPERIENCE_REQUIRE_ARRAY[i] = i < tempIA.Length ? tempIA[i] : MAX_EXPERIENCE;

            EXPERIENCE_REQUIRE_ARRAY[0] = int.MinValue;
        }

        if (!string.IsNullOrEmpty(data[index++])) LevelLearnSkillDic.S2DI(data[index - 1]);

        if (string.IsNullOrEmpty(data[index]))
        {
            index += 25;  //无战斗数据略过长度
        }
        else
        {
            if (!string.IsNullOrEmpty(data[index++])) HPBase = int.Parse(data[index - 1]);
            if (!string.IsNullOrEmpty(data[index++])) MPBase = int.Parse(data[index - 1]);
            if (!string.IsNullOrEmpty(data[index++])) AttackBase = int.Parse(data[index - 1]);
            if (!string.IsNullOrEmpty(data[index++])) MagicBase = int.Parse(data[index - 1]);
            if (!string.IsNullOrEmpty(data[index++])) DefenseBase = int.Parse(data[index - 1]);
            if (!string.IsNullOrEmpty(data[index++])) SpeedBase = int.Parse(data[index - 1]);
            if (!string.IsNullOrEmpty(data[index++])) LuckBase = int.Parse(data[index - 1]);
            if (!string.IsNullOrEmpty(data[index++]))
            {
                int index_ = 0;
                int[] resistanceArray = data[index - 1].S2IA();

                PhiResistanceBase = resistanceArray[index_++];
                WindResistanceBase = resistanceArray[index_++];
                ThunderResistanceBase = resistanceArray[index_++];
                WaterResistanceBase = resistanceArray[index_++];
                FireResistanceBase = resistanceArray[index_++];
                SoilResistanceBase = resistanceArray[index_++];
                PoisonResistanceBase = resistanceArray[index_++];
                WitcheryResistanceBase = resistanceArray[index_++];
            }
            if (!string.IsNullOrEmpty(data[index++])) Critical = int.Parse(data[index - 1]);
            if (!string.IsNullOrEmpty(data[index++])) BattleBG = data[index - 1];
            if (!string.IsNullOrEmpty(data[index++])) BattleIDGroup = data[index - 1].S2IA();
            if (!string.IsNullOrEmpty(data[index++])) BattleDialogue = int.Parse(data[index - 1]);
            if (!string.IsNullOrEmpty(data[index++])) BeatDialogue = int.Parse(data[index - 1]);
            if (!string.IsNullOrEmpty(data[index++])) DefaultOutfit = data[index - 1].S2IA();
            if (!string.IsNullOrEmpty(data[index++])) DefaultSkill = data[index - 1].S2IA();
            if (!string.IsNullOrEmpty(data[index++])) DoubleKill = bool.Parse(data[index - 1]);
            if (!string.IsNullOrEmpty(data[index++])) { BuffChance = int.Parse(data[index - 1].Split(Const.SPLIT_1)[0]); BuffID = int.Parse(data[index - 1].Split(Const.SPLIT_1)[0]); }
            if (!string.IsNullOrEmpty(data[index++])) Summon = int.Parse(data[index - 1]);
            if (!string.IsNullOrEmpty(data[index++])) HenShin = int.Parse(data[index - 1]);
            if (!string.IsNullOrEmpty(data[index++])) JointSkillID = int.Parse(data[index - 1]);
            if (!string.IsNullOrEmpty(data[index++])) StealItem = data[index - 1].S2IA();
            if (!string.IsNullOrEmpty(data[index++])) MeleeKeyFrameArray = data[index - 1].S2IAA();
            if (!string.IsNullOrEmpty(data[index++])) RangedAudioKeyFrameArray = data[index - 1].S2IA();
            if (!string.IsNullOrEmpty(data[index++])) BeatExperience = int.Parse(data[index - 1]);
            if (!string.IsNullOrEmpty(data[index++])) BeatItemArray = data[index - 1].S2IA();
        }

        if (!string.IsNullOrEmpty(data[index++])) SellItem = data[index - 1].S2IA();

        StateEventArray = new GameEventData[data.Length - index][];
        string[] tempSA;
        for (int i = 0; i != StateEventArray.Length; i++)
        {
            tempSA = data[index + i].Split(Const.SPLIT_1);

            StateEventArray[i] = new GameEventData[tempSA.Length];
            for (int j = 0; j != StateEventArray[i].Length; j++)
            {
                StateEventArray[i][j] = tempSA[j].S2GE(Const.SPLIT_2, Const.SPLIT_3);
            }
        }
    }
}


/// <summary>
/// 物品数据
/// </summary>
public sealed class ItemData : DataBase
{
    #region
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
    /// 买价
    /// </summary>
    public readonly int BoughtPrice = -1;

    /// <summary>
    /// 售价
    /// </summary>
    public readonly int SellPrice = -1;

    /// <summary>
    /// 装备
    /// </summary>
    public readonly bool Outfit;

    /// <summary>
    /// 装备部位
    /// </summary>
    public readonly OutfitType OutfitType;

    /// <summary>
    /// 远程武器
    /// </summary>
    public readonly bool Ranged;

    /// <summary>
    /// 双手武器二段攻击
    /// </summary>
    public readonly bool TwoHand;

    /// <summary>
    /// 合击仙术
    /// </summary>
    public readonly int JointSkillID = -1;

    /// <summary>
    /// 角色需求
    /// </summary>
    public readonly int[] RoleRequireArray;

    /// <summary>
    /// 等级需求
    /// </summary>
    public readonly int LevelRequire;

    /// <summary>
    /// 投掷物
    /// </summary>
    public readonly bool Throw;

    /// <summary>
    /// 伤害值
    /// </summary>
    public readonly int Damage;

    /// <summary>
    /// 特效ID取自仙术
    /// </summary>
    public readonly int EffectSkillID = -1;

    /// <summary>
    /// Buff添加概率
    /// </summary>
    public readonly int BuffChance;

    /// <summary>
    /// Buff ID
    /// </summary>
    public readonly int BuffID = -1;

    /// <summary>
    /// 全队效果
    /// </summary>
    public readonly bool Effect2All;

    /// <summary>
    /// 物品效果集合
    /// </summary>
    public readonly RoleEventData[] EventArray;
    #endregion

    public ItemData(params string[] data) : base(data)
    {
        FullIDCompletion(FullIDTimes.Hundreds);
        Icon = Resources.Load<Sprite>("Item/" + FullID);

        int index = 1;
        Name = data[index++];
        Description = data[index++].Replace(Const.SPLIT_P, Const.SPLIT_N);
        if (!string.IsNullOrEmpty(data[index++])) BoughtPrice = int.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) SellPrice = int.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) Outfit = bool.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) OutfitType = data[index - 1].S2E<OutfitType>();
        if (!string.IsNullOrEmpty(data[index++])) Ranged = bool.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) TwoHand = bool.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) JointSkillID = int.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) RoleRequireArray = data[index - 1].S2IA();
        if (!string.IsNullOrEmpty(data[index++])) LevelRequire = int.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) Throw = bool.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) Damage = int.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) EffectSkillID = int.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) BuffChance = int.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) BuffID = int.Parse(data[index - 1]);
        Effect2All = bool.Parse(data[index++]);

        int count = data.Length - index;
        EventArray = new RoleEventData[count];
        for (int i = 0; i != count; i++)
            EventArray[i] = data[index + i].S2RE();
    }
}


/// <summary>
/// 角色事件
/// </summary>
public sealed class RoleEventData
{
    public readonly RoleEffectType RoleEffectType;

    public readonly int Value;

    public RoleEventData(RoleEffectType roleEffectType, int value)
    {
        RoleEffectType = roleEffectType;
        Value = value;
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
/// 仙术数据
/// </summary>
public sealed class SkillData : DataBase
{
    #region
    /// <summary>
    /// 名称
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// 描述
    /// </summary>
    public readonly string Description;

    /// <summary>
    /// 真气消耗
    /// </summary>
    public readonly int Cost;

    /// <summary>
    /// 数值
    /// </summary>
    public readonly bool Attack;

    /// <summary>
    /// 伤害值
    /// </summary>
    public readonly int Damage;

    /// <summary>
    /// 召唤物
    /// </summary>
    public readonly int SummonID = -1;

    /// <summary>
    /// 召唤关键帧，-1为最后帧
    /// </summary>
    public readonly int SummonKeyFrame = -1;

    /// <summary>
    /// 特效ID
    /// </summary>
    public readonly int EffectID = -1;

    /// <summary>
    /// 特效偏移
    /// </summary>
    public readonly Vector2 EffectOffset;

    /// <summary>
    /// 仙术关键帧
    /// </summary>
    public readonly int SkillKeyFrame;

    /// <summary>
    /// Buff添加概率
    /// </summary>
    public readonly int BuffChance;

    /// <summary>
    /// Buff ID
    /// </summary>
    public readonly int BuffID = -1;

    /// <summary>
    /// 单/群体
    /// </summary>
    public readonly bool Effect2All;

    /// <summary>
    /// 地形残留
    /// </summary>
    public readonly bool Terrain;

    /// <summary>
    /// 伤害类型
    /// </summary>
    public readonly NatureType DamageType;

    /// <summary>
    /// 技能效果集合
    /// </summary>
    public readonly RoleEventData[] EventArray;
    #endregion

    public SkillData(params string[] data) : base(data)
    {
        FullIDCompletion(FullIDTimes.Tens);

        int index = 1;
        Name = data[index++];
        Description = data[index++].Replace(Const.SPLIT_P, Const.SPLIT_N);
        Cost = int.Parse(data[index++]);
        Attack = bool.Parse(data[index++]);
        if (!string.IsNullOrEmpty(data[index++])) Damage = int.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) SummonID = int.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) SummonKeyFrame = int.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) EffectID = int.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) EffectOffset = data[index - 1].Split(Const.SPLIT_1).SA2V2();
        if (!string.IsNullOrEmpty(data[index++])) SkillKeyFrame = int.Parse(data[index - 1]);
        else if (-1 != EffectID) SkillKeyFrame = DataManager_.SkillEffectList[EffectID].Last();
        if (!string.IsNullOrEmpty(data[index++])) BuffChance = int.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) BuffID = int.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) Effect2All = bool.Parse(data[index - 1]);
        if (!string.IsNullOrEmpty(data[index++])) Terrain = bool.Parse(data[index - 1]);
        DamageType = data[index++].S2E<NatureType>();

        int count = data.Length - index;
        EventArray = new RoleEventData[count];
        for (int i = 0; i != count; i++)
            EventArray[i] = data[index + i].S2RE();
    }
}


/// <summary>
/// Buff数据
/// </summary>
public sealed class BuffData : DataBase
{
    /// <summary>
    /// 名称
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// 持续时长
    /// </summary>
    public readonly int Duration;

    /// <summary>
    /// 回合数
    /// </summary>
    public int RoundCount { get; private set; } = 1;

    /// <summary>
    /// 数值集合
    /// </summary>
    public readonly int[] ValueArray;

    /// <summary>
    /// 值
    /// </summary>
    public int Value { get; private set; }

    /// <summary>
    /// 伤害类型
    /// </summary>
    public readonly BuffType BuffType;

    /// <summary>
    /// 挂载目标
    /// </summary>
    private BattleRole _battleRole;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="battleRole">战斗角色</param>
    public void Init(BattleRole battleRole)
    {
        _battleRole = battleRole;
        ToolsE.LogWarning(_battleRole.BuffList.Count, battleRole.gameObject);
        for (int i = 0; i != _battleRole.BuffList.Count; i++)
        {
            if (BuffType == _battleRole.BuffList[i].BuffType)
            {
                ToolsE.LogWarning(_battleRole.BuffList[i].RoundCount + "  " + _battleRole.BuffList[i].Duration + "  " + _battleRole.BuffList[i].ValueArray[0]);
                if (ValueArray[0] < 0) _battleRole.BuffList[i] = this;
                else _battleRole.BuffList[i].RoundCount = 1;
                ToolsE.LogWarning(_battleRole.BuffList[i].RoundCount + "  " + _battleRole.BuffList[i].Duration + "  " + _battleRole.BuffList[i].ValueArray[0]);
                return;
            }
        }

        //辅助Buff施放后立即生效
        if (0 <= ValueArray[0])
        {
            switch (BuffType)
            {
                case BuffType.Attack:
                    _battleRole.Attack += Value = (int)((null == _battleRole.RoleEntity ? _battleRole.RoleData.AttackBase : _battleRole.RoleEntity.Attack) * ValueArray[0] * 0.01f).Round();
                    break;
                case BuffType.Defense:
                    ToolsE.LogWarning(Value + "  " + _battleRole.Defense);
                    _battleRole.Defense += Value = (int)((null == _battleRole.RoleEntity ? _battleRole.RoleData.DefenseBase : _battleRole.RoleEntity.Defense) * ValueArray[0] * 0.01f).Round();
                    ToolsE.LogWarning(Value + "  " + _battleRole.Defense);
                    break;
                case BuffType.Speed:
                    _battleRole.Speed += Value = (int)((null == _battleRole.RoleEntity ? _battleRole.RoleData.SpeedBase : _battleRole.RoleEntity.Speed) * ValueArray[0] * 0.01f).Round();
                    break;
            }
        }

        _battleRole.BuffList.Add(this);
    }

    /// <summary>
    /// 处理
    /// </summary>
    /// <returns>结束</returns>
    public bool Handle()
    {
        if (ValueArray[0] < 0)
        {
            Value = ValueArray[ValueArray.Length < Duration ? ValueArray.Last() : RoundCount];
            ToolsE.LogWarning("  Buff value  " + Value);

            _battleRole.Hit(NatureType.Poison, -Value, 0);
        }
        ToolsE.LogWarning(RoundCount);
        if (Duration == RoundCount++)
        {
            switch (BuffType)
            {
                case BuffType.Attack:
                    _battleRole.Attack -= Value;
                    break;
                case BuffType.Defense:
                    ToolsE.LogWarning(Value + "  " + _battleRole.Defense);
                    _battleRole.Defense -= Value;
                    ToolsE.LogWarning(Value + "  " + _battleRole.Defense);
                    break;
                case BuffType.Speed:
                    _battleRole.Speed -= Value;
                    break;
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="data"></param>
    public BuffData(params string[] data) : base(data)
    {
        int index = 1;
        Name = data[index++];
        Duration = int.Parse(data[index++]);
        ValueArray = data[index++].S2IA();
        BuffType = data[index].S2E<BuffType>();
    }

    /// <summary>
    /// 拷贝构造
    /// </summary>
    /// <param name="templateID"></param>
    /// <param name="data"></param>
    public BuffData(int templateID, params string[] data) : base(data)
    {
        BuffData buffData = DataManager_.BuffDataArray[templateID];

        Name = buffData.Name;
        Duration = buffData.Duration;
        ValueArray = buffData.ValueArray;
        BuffType = buffData.BuffType;
    }
}


/// <summary>
/// Buff类型  有些毒玩了20年都不知道它是什么
/// </summary>
public enum BuffType
{
    /// <summary>
    /// 赤毒
    /// </summary>
    Poison0,

    /// <summary>
    /// 尸毒
    /// </summary>
    Poison1,

    /// <summary>
    /// 瘴毒
    /// </summary>
    Poison2,

    /// <summary>
    /// 毒丝
    /// </summary>
    Poison3,

    /// <summary>
    /// 鹤顶红
    /// </summary>
    Poison4,

    /// <summary>
    /// 孔雀胆
    /// </summary>
    Poison5,

    /// <summary>
    /// 血海棠
    /// </summary>
    Poison6,

    /// <summary>
    /// 断肠草
    /// </summary>
    Poison7,

    /// <summary>
    /// 无影毒
    /// </summary>
    Poison8,

    /// <summary>
    /// 三尸蛊
    /// </summary>
    Poison9,

    /// <summary>
    /// 金蚕蛊
    /// </summary>
    Poison10,

    /// <summary>
    /// 定身
    /// </summary>
    ActionLock,

    /// <summary>
    /// 昏睡
    /// </summary>
    Coma,

    /// <summary>
    /// 疯魔
    /// </summary>
    Crazy,

    /// <summary>
    /// 咒封
    /// </summary>
    SkillLock,

    /// <summary>
    /// 加攻
    /// </summary>
    Attack,

    /// <summary>
    /// 加防
    /// </summary>
    Defense,

    /// <summary>
    /// 加速
    /// </summary>
    Speed,

    /// <summary>
    /// 二次攻击
    /// </summary>
    DoubleKill
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
    public DialogueData(params string[] data) : base(data)
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
    public TipData(params string[] data) : base(data)
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
    public ChooseData(params string[] data) : base(data)
    {
        int index = 1;

        AcceptEventArray = data[index++].S2GA();
        RefuseEventArray = data[index++].S2GA();
    }
}