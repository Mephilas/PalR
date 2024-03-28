using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ݻ���
/// </summary>
public abstract class DataBase
{
    /// <summary>
    /// ����ID
    /// </summary>
    public readonly int ID;

    /// <summary>
    /// ȫ��ID
    /// </summary>
    public string FullID { get; private set; }

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="data">����</param>
    public DataBase(string[] data) => FullID = (ID = int.Parse(data[0])).ToString();

    /// <summary>
    /// ȫ��ID����
    /// </summary>
    /// <param name="times">����</param>
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
    /// ȫ��ID���뱶��
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
/// ��������
/// </summary>
public sealed class MissionData : DataBase
{
    //Property
    #region
    /// <summary>
    /// �Ƿ�����
    /// </summary>
    public readonly bool IsMain;

    /// <summary>
    /// ���� ����
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// ˵�� ������ɷ����
    /// </summary>
    public readonly string Description;

    /// <summary>
    /// ��������
    /// </summary>
    public readonly MissionType Type;

    /// <summary>
    /// ���󼯺�
    /// </summary>
    public readonly string[] RequireArray;

    /// <summary>
    /// �¼�����
    /// </summary>
    public readonly GameEventData[] EventArray;

    /// <summary>
    /// ����״̬
    /// </summary>
    public MissionState State { get; set; }
    #endregion

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="data">����</param>
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
/// ��������
/// </summary>
public enum MissionType
{
    /// <summary>
    /// ̨��
    /// </summary>
    Dialogue,

    /// <summary>
    /// ��Ʒ
    /// </summary>
    Item,

    /// <summary>
    /// ����
    /// </summary>
    Monster,
}


/// <summary>
/// ����״̬
/// </summary>
public enum MissionState
{
    /// <summary>
    /// δ����
    /// </summary>
    Inactive,

    /// <summary>
    /// ����
    /// </summary>
    Active,

    /// <summary>
    /// ������
    /// </summary>
    InProgress,

    /// <summary>
    /// ���
    /// </summary>
    Complete,

    /// <summary>
    /// ����
    /// </summary>
    Finish
}


/// <summary>
/// ��ɫ����
/// </summary>
public sealed class RoleData : DataBase
{
    //Property
    #region
    /// <summary>
    /// �����ֵ
    /// </summary>
    private const int MAX_EXPERIENCE = 32000;

    /// <summary>
    /// �������󼯺�
    /// </summary>
    public static readonly int[] EXPERIENCE_REQUIRE_ARRAY = new int[Role.MAX_LEVEL];

    //�����ս��ְ̹ҵ��ֱ����ְҵ���ݱ���ж�ȡ

    /// <summary>
    /// �����ɳ�
    /// </summary>
    public const int HP_GROW_MIN = 10,
                     HP_GROW_MAX = 17;

    /// <summary>
    /// �����ɳ�
    /// </summary>
    public const int MP_GROW_MIN = 8,
                     MP_GROW_MAX = 13;

    /// <summary>
    /// �����ɳ�
    /// </summary>
    public const int ATTACK_GROW_MIN = 4,
                     ATTACK_GROW_MAX = 5;

    /// <summary>
    /// �����ɳ�
    /// </summary>
    public const int MAGIC_GROW_MIN = 4,
                     MAGIC_GROW_MAX = 5;

    /// <summary>
    /// �����ɳ�
    /// </summary>
    public const int DEFENSE_GROW_MIN = 2,
                     DEFENSE_GROW_MAX = 3;

    /// <summary>
    /// ���ɳ�
    /// </summary>
    public const int SPEED_GROW_MIN = 2,
                     SPEED_GROW_MAX = 3;

    /// <summary>
    /// ���˳ɳ�
    /// </summary>
    public const int LUCK_GROW_MIN = 2,
                     LUCK_GROW_MAX = 2;

    /// <summary>
    /// ��������
    /// </summary>
    private readonly List<Dictionary<InputType, Sprite[]>> AnimList = new();

    /// <summary>
    /// ��ǰ��������
    /// </summary>
    public Dictionary<InputType, Sprite[]> CurrentAnimDic { get { return AnimList[SkinIndex]; } }

    /// <summary>
    /// ���⶯������
    /// </summary>
    public readonly Dictionary<SpecialAnimType, Sprite[]> SpecialAnimDic = new();

    /// <summary>
    /// ͷ�񼯺�
    /// </summary>
    public readonly Dictionary<ExpressionType, Sprite> ProfilePictureDic = new();

    /// <summary>
    /// ״̬�¼�����
    /// </summary>
    public readonly GameEventData[][] StateEventArray;

    /// <summary>
    /// ����
    /// </summary>
    public string Name { get { return string.IsNullOrEmpty(Pseudonym) ? _name : Pseudonym; } }

    /// <summary>
    /// ����
    /// </summary>
    private readonly string _name;

    /// <summary>
    /// ����
    /// </summary>
    public string Pseudonym { get; set; }

    /// <summary>
    /// Ƥ�����
    /// </summary>
    public int SkinIndex { get; set; }

    /// <summary>
    /// ̨�����
    /// </summary>
    public int DialogueIndex { get; set; } = -1;

    /// <summary>
    /// �ȼ�ѧϰ���ܼ���
    /// </summary>
    public readonly Dictionary<int, int> LevelSkillDic = new();

    /// <summary>
    /// ��������
    /// </summary>
    public readonly int HPBase;

    /// <summary>
    /// ��������
    /// </summary>
    public readonly int MPBase;

    /// <summary>
    /// ��������
    /// </summary>
    public readonly int AttackBase;

    /// <summary>
    /// ��������
    /// </summary>
    public readonly int MagicBase;

    /// <summary>
    /// ��������
    /// </summary>
    public readonly int DefenseBase;

    /// <summary>
    /// ������
    /// </summary>
    public readonly int SpeedBase;

    /// <summary>
    /// ��������
    /// </summary>
    public readonly int LuckBase;

    /// <summary>
    /// ս����ɫID����
    /// </summary>
    public readonly int[] BattleIDGroup;

    /// <summary>
    /// Ĭ��װ��
    /// </summary>
    public readonly int[] DefaultOutfit;

    /// <summary>
    /// Ĭ������
    /// </summary>
    public readonly int[] DefaultSkill;

    /// <summary>
    /// ������Ʒ
    /// </summary>
    public readonly int[] SellItem;  //���Ϊ���˶��̵겢���й�������

    /// <summary>
    /// ����̽���ֵ���
    /// </summary>
    public readonly int[] StealItem;

    /// <summary>
    /// ���ܵ���
    /// </summary>
    public readonly int[] DefeatItem;
    #endregion

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="data">����</param>
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
            index += 12;  //��ս�������Թ�����
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
/// ��Ʒ����
/// </summary>
public sealed class ItemData : DataBase
{
    /// <summary>
    /// �ѵ�����
    /// </summary>
    public const int PILE_COUNT = 99;

    /// <summary>
    /// ͼ��
    /// </summary>
    public readonly Sprite Icon;

    /// <summary>
    /// ����
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// ����
    /// </summary>
    public readonly string Description;

    /// <summary>
    /// �������
    /// </summary>
    public readonly bool StoryItem;

    /// <summary>
    /// ����
    /// </summary>
    public readonly bool Reduce = true;

    /// <summary>
    /// �Ƿ�װ��
    /// </summary>
    public readonly bool OutfitOrConsumable;

    /// <summary>
    /// װ����λ
    /// </summary>
    public readonly OutfitType OutfitType;

    /// <summary>
    /// ��ɫ����
    /// </summary>
    public readonly int[] RoleRequireArray;

    /// <summary>
    /// �ȼ�����
    /// </summary>
    public readonly int LevelRequire;

    /// <summary>
    /// ���
    /// </summary>
    public readonly int BoughtPrice;

    /// <summary>
    /// �ۼ�
    /// </summary>
    public readonly int SellPrice;

    /// <summary>
    /// ������
    /// </summary>
    public bool CanSell { get { return 0 == SellPrice; } }

    /// <summary>
    /// ��ƷЧ������
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
/// װ����λ
/// </summary>
public enum OutfitType
{
    /// <summary>
    /// ͷ��
    /// </summary>
    Casque,

    /// <summary>
    /// ����
    /// </summary>
    Cape,

    /// <summary>
    /// ����
    /// </summary>
    Armor,

    /// <summary>
    /// ����
    /// </summary>
    Weapon,

    /// <summary>
    /// ѥ��
    /// </summary>
    Boots,

    /// <summary>
    /// ����
    /// </summary>
    Bracers
}


/// <summary>
/// ��ɫЧ������
/// </summary>
public enum RoleEffectType
{
    /// <summary>
    /// ̨��
    /// </summary>
    Dialogue,

    /// <summary>
    /// ��������
    /// </summary>
    ExperienceAdd,

    /// <summary>
    /// �ȼ�����
    /// </summary>
    LevelAdd,

    /// <summary>
    /// ����ѧϰ
    /// </summary>
    SkillLearn,

    /// <summary>
    /// Buff���
    /// </summary>
    BuffAdd,

    /// <summary>
    /// ��������
    /// </summary>
    HPAdd,

    /// <summary>
    /// ������������
    /// </summary>
    HPMaxAdd,

    /// <summary>
    /// ��������
    /// </summary>
    MPAdd,

    /// <summary>
    /// ������������
    /// </summary>
    MPMaxAdd,

    /// <summary>
    /// ��������
    /// </summary>
    AttackAdd,

    /// <summary>
    /// ��������
    /// </summary>
    MagicAdd,

    /// <summary>
    /// ��������
    /// </summary>
    DefenseAdd,

    /// <summary>
    /// ������
    /// </summary>
    SpeedAdd,

    /// <summary>
    /// ��������
    /// </summary>
    LuckAdd
}


/// <summary>
/// ��������
/// </summary>
public sealed class SkillData : DataBase
{
    /// <summary>
    /// ��Ч����
    /// </summary>
    public readonly Sprite[] EffectArray;

    /// <summary>
    /// ����
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// ����
    /// </summary>
    public readonly string Description;

    /// <summary>
    /// ��/Ⱥ��
    /// </summary>
    public readonly bool IsSingle;

    /// <summary>
    /// ��������
    /// </summary>
    public readonly int Cost;

    /// <summary>
    /// ��ֵ
    /// </summary>
    public readonly int Value;

    /// <summary>
    /// Buff ID
    /// </summary>
    public readonly int BuffID;

    /// <summary>
    /// Buff��ӳɹ���
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
/// ̨������
/// </summary>
public sealed class DialogueData : DataBase
{
    //Property
    #region
    /// <summary>
    /// �Զ�����
    /// </summary>
    public readonly bool AutoPlay;

    /// <summary>
    /// �Ի���
    /// </summary>
    private readonly int _speaker = -1;

    /// <summary>
    /// �Ի���
    /// </summary>
    public RoleData Speaker { get { return -1 == _speaker ? null : DataManager_.RoleDataArray[_speaker]; } }

    /// <summary>
    /// ̨��ͷ������
    /// </summary>
    public readonly ExpressionType ExpressionType = ExpressionType.Nil;

    /// <summary>
    /// ̨���ı�
    /// </summary>
    public readonly string DialogueText;

    /// <summary>
    /// ��ʼ�¼�
    /// </summary>
    public readonly GameEventData StartEvent;

    /// <summary>
    /// ��ʼ�¼�
    /// </summary>
    public readonly GameEventData StartEvent_;

    /// <summary>
    /// �����¼�����
    /// </summary>
    public readonly GameEventData[] EventArray;
    #endregion

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="data">����</param>
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
/// ��������
/// </summary>
public enum ExpressionType
{
    /// <summary>
    /// ��һ�ж������ް�...
    /// </summary>
    Nil = -1,

    /// <summary>
    /// ��ͨ
    /// </summary>
    Normal,

    /// <summary>
    /// ΢Ц
    /// </summary>
    Smile,

    /// <summary>
    /// �����������ǲ��еģ��ҽ������ǵ���������һ�£�
    /// </summary>
    Angry,

    /// <summary>
    /// ����
    /// </summary>
    Sad,
}


/// <summary>
/// ��ʾ����
/// </summary>
public sealed class TipData : DataBase
{
    /// <summary>
    /// ��ʾ�ı�
    /// </summary>
    public readonly string TipText;

    /// <summary>
    /// �¼�����
    /// </summary>
    public readonly GameEventData[] EventArray;

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="data">����</param>
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
/// ѡ������
/// </summary>
public sealed class ChooseData : DataBase
{
    /// <summary>
    /// �����¼�����
    /// </summary>
    public readonly GameEventData[] AcceptEventArray;

    /// <summary>
    /// �ܾ��¼�����
    /// </summary>
    public readonly GameEventData[] RefuseEventArray;

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="data">����</param>
    public ChooseData(string[] data) : base(data)
    {
        int index = 1;

        AcceptEventArray = data[index++].S2GA();
        RefuseEventArray = data[index++].S2GA();
    }
}