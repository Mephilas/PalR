using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// 角色
/// </summary>
public partial class Role : SpriteBase, System.IComparable<Role>
{
    //Property
    #region
    /// <summary>
    /// 移动速度
    /// </summary>
    protected const float MOVE_SPEED = 1.5f;

    /// <summary>
    /// 移动象限
    /// </summary>
    public const int MOVE_DIRECTION_COUNT = 4;

    /// <summary>
    /// 传送时间
    /// </summary>
    private const float TRANSFERRED_TIME = 0.05f;

    /// <summary>
    /// 跟随偏移
    /// </summary>
    //private static readonly Vector3 FOLLOW_OFFSET = new(0, 0, -0.09f);

    /// <summary>
    /// 移动输入集合
    /// </summary>
    public static readonly Dictionary<InputType, Vector3> MOVE_INPUT_DIC = new(MOVE_DIRECTION_COUNT)
    {
        { InputType.Up, new Vector3(1, 0, 0.6f) },
        { InputType.Down, new Vector3(-1, 0, -0.6f) },
        { InputType.Left, new Vector3(-1, 0, 0.6f) },
        { InputType.Right, new Vector3(1, 0, -0.6f) }
    };

    /// <summary>
    /// 角色控制器
    /// </summary>
    protected CharacterController CharacterC { get; private set; }

    /// <summary>
    /// 特殊动画循环/单次
    /// </summary>
    private bool _isLoop;

    /// <summary>
    /// 当前动画集合
    /// </summary>
    protected Sprite[] CurrentAnimArray { get; set; }

    /// <summary>
    /// 上帧输入
    /// </summary>
    protected InputType LastKeyCode { get; set; }

    /// <summary>
    /// 当前输入
    /// </summary>
    protected InputType CurrentKeyCode { get; set; }

    /// <summary>
    /// 角色数据
    /// </summary>
    public RoleData RoleData { get; private set; }

    /// <summary>
    /// 当前状态序号
    /// </summary>
    public int CurrentState { get; private set; }

    /// <summary>
    /// 角色效果处理集合
    /// </summary>
    private readonly Dictionary<RoleEffectType, UnityAction<int>> RoleEffectDic = new();

    /// <summary>
    /// 最大等级
    /// </summary>
    public const int MAX_LEVEL = 99;

    /// <summary>
    /// 最大属性值
    /// </summary>
    private const int MAX_PROPERTY = 999;

    /// <summary>
    /// 修行
    /// </summary>
    public int Level { get; private set; }

    /// <summary>
    /// 经验值
    /// </summary>
    public int Experience { get; private set; }

    /// <summary>
    /// 存活
    /// </summary>
    public bool IsAlive { get { return 0 < HP; } }

    /// <summary>
    /// 体力
    /// </summary>
    public int HP { get; private set; }

    /// <summary>
    /// 最大体力
    /// </summary>
    public int HPMax { get; private set; }

    /// <summary>
    /// 真气
    /// </summary>
    public int MP { get; private set; }

    /// <summary>
    /// 最大真气
    /// </summary>
    public int MPMax { get; private set; }

    /// <summary>
    /// 武术
    /// </summary>
    public int Attack { get; private set; }

    /// <summary>
    /// 灵力
    /// </summary>
    public int Magic { get; private set; }

    /// <summary>
    /// 防御
    /// </summary>
    public int Defense { get; private set; }

    /// <summary>
    /// 身法
    /// </summary>
    public int Speed { get; private set; }

    /// <summary>
    /// 吉运
    /// </summary>
    public int Luck { get; private set; }

    /// <summary>
    /// 装备集合
    /// </summary>
    public readonly Dictionary<OutfitType, int> OutfitDic = new();

    /// <summary>
    /// 仙术集合
    /// </summary>
    public readonly List<int> SkillList = new();

    /// <summary>
    /// 存档数据
    /// </summary>
    public string SaveData { get { return RoleData.ID.ToString() + "_" + CurrentState; } }

    /// <summary>
    /// 移动功能
    /// </summary>
    protected static bool CanMove { get; private set; }

    /// <summary>
    /// 跟随开关
    /// </summary>
    private bool _followSwitch;

    /// <summary>
    /// 传送位置
    /// </summary>
    private Vector3 _portalP;

    /// <summary>
    /// 玩家存档数据
    /// </summary>
    public string PlayerSaveData
    {
        get
        {
            string saveData =
                RoleData.ID.ToString() + Const.SPLIT_2 +
                Transform.localPosition.V2S_Y(Const.SPLIT_2) + Const.SPLIT_2 + CurrentKeyCode + Const.SPLIT_2 +
                Experience + Const.SPLIT_2 +
                Level + Const.SPLIT_2 +
                HP + Const.SPLIT_2 +
                HPMax + Const.SPLIT_2 +
                MP + Const.SPLIT_2 +
                MPMax + Const.SPLIT_2 +
                Attack + Const.SPLIT_2 +
                Magic + Const.SPLIT_2 +
                Defense + Const.SPLIT_2 +
                Speed + Const.SPLIT_2 +
                Luck + Const.SPLIT_2;

            foreach (OutfitType outfitType in OutfitDic.Keys)
            {
                saveData += OutfitDic[outfitType].ToString() + Const.SPLIT_3;
            }
            saveData = saveData.TrimEnd(Const.SPLIT_3) + Const.SPLIT_2;

            for (int i = 0; i != SkillList.Count; i++)
            {
                saveData += SkillList[i].ToString() + Const.SPLIT_3;
            }
            saveData = saveData.TrimEnd(Const.SPLIT_3) + Const.SPLIT_2;

            return saveData.TrimEnd(Const.SPLIT_2);

            //return saveData.TrimEnd(Const.SPLIT_3) + Const.SPLIT_2 + ToolsE.IA2S(SkillList.ToArray(), Const.SPLIT_3);
        }
    }
    #endregion

    //Function
    #region
    protected override void Awake()
    {
        base.Awake();

        gameObject.layer = LayerMask.NameToLayer(tag = GetType().Name);

        CharacterC = GC<CharacterController>();

        RoleEffectDic.Add(RoleEffectType.Dialogue, Dialogue);
        RoleEffectDic.Add(RoleEffectType.ExperienceAdd, ExperienceAdd);
        RoleEffectDic.Add(RoleEffectType.LevelAdd, LevelAdd);
        RoleEffectDic.Add(RoleEffectType.SkillLearn, SkillLearn);
        RoleEffectDic.Add(RoleEffectType.BuffAdd, BuffAdd);
        RoleEffectDic.Add(RoleEffectType.HPAdd, HPAdd);
        RoleEffectDic.Add(RoleEffectType.HPMaxAdd, HPMaxAdd);
        RoleEffectDic.Add(RoleEffectType.MPAdd, MPAdd);
        RoleEffectDic.Add(RoleEffectType.MPMaxAdd, MPMaxAdd);
        RoleEffectDic.Add(RoleEffectType.AttackAdd, AttackAdd);
        RoleEffectDic.Add(RoleEffectType.MagicAdd, MagicAdd);
        RoleEffectDic.Add(RoleEffectType.DefenseAdd, DefenseAdd);
        RoleEffectDic.Add(RoleEffectType.SpeedAdd, SpeedAdd);
        RoleEffectDic.Add(RoleEffectType.LuckAdd, LuckAdd);
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        Follow();

        if (IsMoving) SortingOrder();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        //ToolsE.LogWarning(other.gameObject);

        if (other.gameObject.CompareTag("Portal"))
        {
            string[] exitPath = DataManager_.PortalDataDic[ColliderPath()].Split(Const.SPLIT_1);
            _portalP = Root_.Instance.CGC<Transform>(exitPath[0]).position;
            GameManager_.Trigger(new(GameEventType.RoleTransfer, new string[] { RoleData.ID.ToString(), _portalP.x.ToString(), _portalP.y.ToString(), (_portalP.z * 0.833).ToString() }));
        }
        else if (other.gameObject.CompareTag("Trigger") && CompareTag(nameof(Player)))
            GameManager_.TriggerAll(DataManager_.MapEventDataDic[ColliderPath()]);


        string ColliderPath() => other.transform.parent.name + Const.SPLIT_3 + other.name;
    }

    /// <summary>
    /// 实体初始化
    /// </summary>
    /// <param name="roleData">角色数据</param>
    public void Init(RoleData roleData) => name = (RoleData = roleData).Name;

    /// <summary>
    /// 数据初始化
    /// </summary>
    public void DataInit()
    {
        foreach (int outfitID in OutfitDic.Values)
            Equip(DataManager_.ItemDataArray[outfitID], false);

        OutfitDic.Clear();
        SkillList.Clear();

        GameManager_.Trigger(new(GameEventType.RoleState, new string[] { RoleData.ID.ToString(), "0" }));
        Experience = 0;
        Level = 1;
        BattleDataClone();

        if (null != RoleData.DefaultOutfit)
        {
            for (int i = 0; i != RoleData.DefaultOutfit.Length; i++)
            {
                GameManager_.Trigger(new(GameEventType.ItemAdd, RoleData.DefaultOutfit[i].ToString()));
                GameManager_.Trigger(new(GameEventType.ItemEquip, new string[] { RoleData.ID.ToString(), RoleData.DefaultOutfit[i].ToString() }));
            }
        }

        if (null != RoleData.DefaultSkill)
        {
            for (int i = 0; i != RoleData.DefaultSkill.Length; i++)
                SkillList.Add(RoleData.DefaultSkill[i]);
        }
    }

    /// <summary>
    /// 战斗数据复制
    /// </summary>
    protected void BattleDataClone(Role source = null)
    {
        HP = HPMax = RoleData.HPBase;
        MP = MPMax = RoleData.MPBase;
        Attack = RoleData.AttackBase;
        Magic = RoleData.MagicBase;
        Defense = RoleData.DefenseBase;
        Speed = RoleData.SpeedBase;
        Luck = RoleData.LuckBase;

        if (null != source)
        {
            HP = source.HP;
            MP = source.MP;

            if (OutfitDic.ContainsKey(OutfitType.Bracers))
                OutfitDic[OutfitType.Bracers] = source.OutfitDic[OutfitType.Bracers];
            else
                OutfitDic.Add(OutfitType.Bracers, source.OutfitDic[OutfitType.Bracers]);

            for (int i = 0; i != source.SkillList.Count; i++)
                SkillList.Add(source.SkillList[i]);
        }
    }

    /// <summary>
    /// 移动开关
    /// </summary>
    /// <param name="sw1tch">开关</param>
    public void MovementSwitch(bool sw1tch)
    {
        if (!(CanMove = sw1tch)) Idle();
    }

    /// <summary>
    /// 闲置
    /// </summary>
    protected virtual void Idle() { }

    /// <summary>
    /// 侦察
    /// </summary>
    public virtual void Detect() { }

    /// <summary>
    /// 屏幕射线发起
    /// </summary>
    public virtual void ScreenRaycast() { }

    /// <summary>
    /// 输入处理
    /// </summary>
    /// <param name="keyCode">按键</param>
    public virtual void InputHandle(InputType keyCode) { }

    /// <summary>
    /// 是否移动
    /// </summary>
    protected virtual bool IsMoving { get; set; }

    /// <summary>
    /// 跟随
    /// </summary>
    private void Follow()
    {
        if (_followSwitch)
        {
            Transform.localPosition = GameManager_.Leader.Transform.localPosition - MOVE_INPUT_DIC[GameManager_.Leader.CurrentKeyCode].normalized * 0.25f;
            SpriteRenderer.sprite = RoleData.CurrentAnimDic[GameManager_.Leader.CurrentKeyCode][int.Parse(GameManager_.Leader.SpriteRenderer.sprite.name)];
        }
    }

    /// <summary>
    /// 交互
    /// </summary>
    public void Interact()
    {
        if (-1 != RoleData.DialogueIndex) GameManager_.Trigger(new(GameEventType.Dialogue, RoleData.DialogueIndex.ToString()));
    }

    /// <summary>
    /// 玩家切换
    /// </summary>
    /// <param name="isPlayer">是否玩家</param>
    public void PlayerSwitch(bool isPlayer) => gameObject.layer = LayerMask.NameToLayer(tag = isPlayer ? nameof(Player) : nameof(Role));

    /// <summary>
    /// 离队
    /// </summary>
    public void Leave() => PlayerSwitch(IsMoving = _followSwitch = false);

    /// <summary>
    /// 移动完成
    /// </summary>
    private void Transferred()
    {
        if (UIManager_.PanelCompare(UIPanel.BasicPanel)) MovementSwitch(true);
    }

    /// <summary>
    /// 昼夜变化
    /// </summary>
    /// <param name="dayOrNight">日夜</param>
    public void TimeChange(bool dayOrNight)
    {
        Color tempC;
        float alpha = SpriteRenderer.color.a;

        tempC = (dayOrNight ? Color.white : Const.NIGHT_COLOR).ColorModifyA(alpha);
        SpriteRenderer.DOColor(tempC, Const.TIME_CHANGE_DURATION);
    }

    /// <summary>
    /// 角色效果触发
    /// </summary>
    /// <param name="roleEffectType">角色效果</param>
    /// <param name="value">效果数值</param>
    public void Trigger(RoleEffectType roleEffectType, int value)
    {
        RoleEffectDic[roleEffectType].Invoke(value);

        /*ToolsE.LogWarning(GetType() + " || " + GetType().GetMethod(roleEffectType.ToString(), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

        GetType().GetMethod(roleEffectType.ToString(), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Invoke(this, new object[] { data });*/
    }

    /// <summary>
    /// 角色效果触发
    /// </summary>
    /// <param name="roleEffectType">角色效果</param>
    /// <param name="value">效果数值</param>
    public void Trigger(ItemEvent itemEvent) => RoleEffectDic[itemEvent.RoleEffectType].Invoke(itemEvent.EffectValue);

    /// <summary>
    /// 角色效果全部触发
    /// </summary>
    /// <param name="roleEffectArray">角色效果集合</param>
    /// <param name="valueArray">效果数值集合</param>
    public void Trigger(List<ItemEvent> itemEventList)
    {
        for (int i = 0; i != itemEventList.Count; i++)
            Trigger(itemEventList[i]);
    }

    /// <summary>
    /// 升级
    /// </summary>
    /// <param name="ExperienceClear">经验清零</param>
    private void LevelUp(bool ExperienceClean = false)
    {
        if (ExperienceClean) Experience = 0;

        if (RoleData.LevelLearnSkillDic.ContainsKey(++Level))
            SkillList.Add(RoleData.LevelLearnSkillDic[Level]);

        HP = Random.Range(RoleData.HP_GROW_MIN, RoleData.HP_GROW_MAX + 1);
        MP = Random.Range(RoleData.MP_GROW_MIN, RoleData.MP_GROW_MAX + 1);
        Attack = Random.Range(RoleData.ATTACK_GROW_MIN, RoleData.ATTACK_GROW_MAX + 1);
        Magic = Random.Range(RoleData.MAGIC_GROW_MIN, RoleData.MAGIC_GROW_MAX + 1);
        Defense = Random.Range(RoleData.DEFENSE_GROW_MIN, RoleData.DEFENSE_GROW_MAX + 1);
        Speed = Random.Range(RoleData.SPEED_GROW_MIN, RoleData.SPEED_GROW_MAX + 1);
        Luck = Random.Range(RoleData.LUCK_GROW_MIN, RoleData.LUCK_GROW_MAX + 1);
    }

    /// <summary>
    /// 动画协程
    /// </summary>
    /// <returns></returns>
    protected IEnumerator AnimationC()
    {
        if (0 == CurrentAnimArray.Length) throw new System.Exception("Anim array length is 0, ID : " + RoleData.ID);

        while (true)
        {
            for (int i = 0; i != CurrentAnimArray.Length; i++)
            {
                SpriteRenderer.sprite = CurrentAnimArray[i];

                yield return Const.ANIMATION_PLAY_SPEED;
            }
        }
    }

    /// <summary>
    /// 特殊动画协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpecialAnimC()
    {
        do
        {
            for (int i = 0; i != CurrentAnimArray.Length; i++)
            {
                SpriteRenderer.sprite = CurrentAnimArray[i];

                yield return Const.SPECIAL_ANIMATION_PLAY_SPEED;
            }
        } while (_isLoop);

        StopCoroutine(nameof(SpecialAnimC));
    }

    /// <summary>
    /// 仙术施放
    /// </summary>
    public void SkillCast(string[] data)
    {
        SkillData skill = DataManager_.SkillDataArray[int.Parse(data[1])];
        MP -= skill.Cost;
        GameManager_.Trigger(new(GameEventType.HPAdd, new string[] { data[2], skill.Value.ToString() }));
    }

    /// <summary>
    /// 使用物品
    /// </summary>
    /// <param name="data">数据</param>
    public void ItemUse(string[] data)
    {
        ItemData item = DataManager_.ItemDataArray[int.Parse(data[1])];

        Trigger(item.EventList);

        GameManager_.Trigger(new(GameEventType.ItemAdd, new string[] { item.ID.ToString(), "-1" }));
    }

    /// <summary>
    /// 穿戴装备
    /// </summary>
    /// <param name="data">数据</param>
    public void ItemEquip(string[] data)
    {
        ItemData outfit = DataManager_.ItemDataArray[int.Parse(data[1])];

        if (OutfitDic.ContainsKey(outfit.OutfitType))
        {
            Equip(DataManager_.ItemDataArray[OutfitDic[outfit.OutfitType]], false);
            OutfitDic[outfit.OutfitType] = outfit.ID;
        }
        else OutfitDic.Add(outfit.OutfitType, outfit.ID);

        Equip(outfit, true);
    }

    /// <summary>
    /// 装备
    /// </summary>
    /// <param name="outfit">装备</param>
    /// <param name="equipOrRemove">穿/卸</param>
    private void Equip(ItemData outfit, bool equipOrRemove)
    {
        for (int i = 0; i != outfit.EventList.Count; i++)
            Trigger(outfit.EventList[i].RoleEffectType, equipOrRemove ? outfit.EventList[i].EffectValue : -outfit.EventList[i].EffectValue);

        GameManager_.Trigger(new(GameEventType.ItemAdd, new string[] { outfit.ID.ToString(), (equipOrRemove ? -1 : 1).ToString() }));
        if (!equipOrRemove) ItemPanel.SelectItem = DataManager_.ItemDataArray[outfit.ID];
    }

    /// <summary>
    /// 排序，后续根据身法调整
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    public int CompareTo(Role role) => RoleData.ID - role.RoleData.ID;

    public void RoleFollow(string[] data) => IsMoving = _followSwitch = bool.Parse(data[1]);
    public void RoleState(string[] data)
    {
        RoleData.Pseudonym = null;
        StopCoroutine(nameof(SpecialAnimC));
        GameManager_.TriggerAll(RoleData.StateEventArray[CurrentState = int.Parse(data[1])]);
    }
    public void RoleTransfer(string[] data)
    {
        bool isPortal;
        if (isPortal = this == GameManager_.Leader && UIManager_.PanelCompare(UIPanel.BasicPanel)) MovementSwitch(false);

        Transform.localPosition = data.SA2V3().Planarization();
        SortingOrder();

        if (5 == data.Length)
            GameManager_.Trigger(new(GameEventType.RoleRotate, new string[] { RoleData.ID.ToString(), data[4] }));

        if (isPortal && UIManager_.PanelCompare(UIPanel.BasicPanel)) Invoke(nameof(Transferred), TRANSFERRED_TIME);
    }
    public void RoleMove(string[] data)
    {
        IsMoving = true;

        Transform.DOLocalMove(data.SA2V3(), float.Parse(data[4])).onComplete =
            () => { if (5 < data.Length) GameManager_.Trigger(new(GameEventType.Dialogue, data[5])); IsMoving = false; };

        //SortingOrder();
    }
    public void RoleRotate(string[] data)
    {
        SpriteRenderer.sprite = RoleData.CurrentAnimDic[LastKeyCode = data[1].S2E<InputType>()][0];
    }
    public void RoleFade(string[] data)
    {
        if ("0" == data[2])
        {
            SpriteRenderer.color = SpriteRenderer.color.ColorModifyA(float.Parse(data[1]));
        }
        else SpriteRenderer.DOFade(float.Parse(data[1]), float.Parse(data[2]));
    }
    public void RoleGradual(string[] _) { }
    public void Skin(string[] data) => RoleData.SkinIndex = int.Parse(data[1]);
    public void Pseudonym(string[] data) => RoleData.Pseudonym = data[1];
    public void DialogueIndex(string[] data) => RoleData.DialogueIndex = int.Parse(data[1]);
    public void RoleFlip(string[] data)
    {
        if ("x" == data[1] || "X" == data[1])
        {
            SpriteRenderer.flipX = bool.Parse(data[2]);
        }
        else if ("y" == data[1] || "y" == data[1])
        {
            SpriteRenderer.flipY = bool.Parse(data[2]);
        }
    }
    public void Anim(string[] data)
    {
        StopCoroutine(nameof(AnimationC));

        CurrentAnimArray = RoleData.CurrentBattleAnimDic[data[1].S2E<BattleAnimType>()];

        StartCoroutine(nameof(AnimationC));
    }
    public void SpecialAnim(string[] data)
    {
        StopCoroutine(nameof(SpecialAnimC));

        CurrentAnimArray = RoleData.SpecialAnimDic[data[1].S2E<SpecialAnimType>()];

        if (bool.Parse(data[2]))
            SpriteRenderer.sprite = CurrentAnimArray[0];
        else
        {
            _isLoop = bool.Parse(data[3]);
            StartCoroutine(nameof(SpecialAnimC));
        }
    }
    public void Load(string[] data)
    {
        int index = 5;

        //ToolsE.LogWarning(data);
        GameManager_.Trigger(new(GameEventType.RoleTransfer, data.SACut(index)));
        foreach (int outfitID in OutfitDic.Values)
            Equip(DataManager_.ItemDataArray[outfitID], false);

        SkillList.Clear();

        Experience = int.Parse(data[index++]);
        Level = int.Parse(data[index++]);
        HP = int.Parse(data[index++]);
        HPMax = int.Parse(data[index++]);
        MP = int.Parse(data[index++]);
        MPMax = int.Parse(data[index++]);
        Attack = int.Parse(data[index++]);
        Magic = int.Parse(data[index++]);
        Defense = int.Parse(data[index++]);
        Speed = int.Parse(data[index++]);
        Luck = int.Parse(data[index++]);

        for (int i = 0; i != RoleData.DefaultOutfit.Length; i++)
        {
            GameManager_.Trigger(new(GameEventType.ItemAdd, RoleData.DefaultOutfit[i].ToString()));
            GameManager_.Trigger(new(GameEventType.ItemEquip, new string[] { RoleData.ID.ToString(), RoleData.DefaultOutfit[i].ToString() }));

            //Equip(DataManager_.ItemDataArray[RoleData.DefaultOutfit[i]], true);
        }

        for (int i = 0; i != RoleData.DefaultSkill.Length; i++)
            SkillLearn(RoleData.DefaultSkill[i]);

        GameManager_.Trigger(new(GameEventType.PlayerJoin, RoleData.ID.ToString()));
    }

    private void Dialogue(int value) => GameManager_.Trigger(new(GameEventType.Dialogue, value.ToString()));
    private void ExperienceAdd(int value)
    {
        if (RoleData.EXPERIENCE_REQUIRE_ARRAY[Level] <= (Experience += value))
        {
            Experience -= RoleData.EXPERIENCE_REQUIRE_ARRAY[Level];

            LevelUp();
        }
    }
    private void LevelAdd(int value) => LevelUp(true);
    private void SkillLearn(int value) => SkillList.Add(value);
    private void BuffAdd(int value) { }
    private void HPAdd(int value)
    {
        if (HPMax < (HP += value))
            HP = HPMax;
    }
    private void HPMaxAdd(int value)
    {
        if (MAX_PROPERTY < (HPMax += value))
            HPMax = MAX_PROPERTY;
    }
    private void MPAdd(int value)
    {
        if (MPMax < (MP += value))
            MP = MPMax;
    }
    private void MPMaxAdd(int value)
    {
        if (MAX_PROPERTY < (MPMax += value))
            MPMax = MAX_PROPERTY;
    }
    private void AttackAdd(int value) => Attack += value;
    private void MagicAdd(int value) => Magic += value;
    private void DefenseAdd(int value) => Defense += value;
    private void SpeedAdd(int value) => Speed += value;
    private void LuckAdd(int value) => Luck += value;
    #endregion
}


/// <summary>
/// 特殊动画
/// </summary>
public enum SpecialAnimType
{
    /// <summary>
    /// 抱胸
    /// </summary>
    AcrossChest,

    /// <summary>
    /// 躲避
    /// </summary>
    Avoid,

    /// <summary>
    /// 教训
    /// </summary>
    Chide,

    /// <summary>
    /// 攀爬
    /// </summary>
    Climb,

    /// <summary>
    /// 昏迷
    /// </summary>
    Coma,

    /// <summary>
    /// 安慰
    /// </summary>
    Comfort,

    /// <summary>
    /// 哭泣
    /// </summary>
    Cry,

    /// <summary>
    /// 跳落
    /// </summary>
    Drop,

    /// <summary>
    /// 着迷
    /// </summary>
    Fascinate,

    /// <summary>
    /// 起身
    /// </summary>
    GetUp,

    /// <summary>
    /// 爆头
    /// </summary>
    HeadShot,

    /// <summary>
    /// 躲藏
    /// </summary>
    Hide,

    /// <summary>
    /// 拥抱
    /// </summary>
    Hug,

    /// <summary>
    /// 躺
    /// </summary>
    Lie,

    /// <summary>
    /// 下跪
    /// </summary>
    Knee,

    /// <summary>
    /// 磕头
    /// </summary>
    Kowtow,

    /// <summary>
    /// 引诱
    /// </summary>
    Lure,

    /// <summary>
    /// 表演
    /// </summary>
    Perform,

    /// <summary>
    /// 休闲
    /// </summary>
    Pleasure,

    /// <summary>
    /// 放下
    /// </summary>
    PutDown,

    /// <summary>
    /// 收起
    /// </summary>
    PutAway,

    /// <summary>
    /// 致敬
    /// </summary>
    Salute,

    /// <summary>
    /// 洗浴
    /// </summary>
    Shower,

    /// <summary>
    /// 悲伤
    /// </summary>
    Sigh,

    /// <summary>
    /// 坐
    /// </summary>
    Sit,

    /// <summary>
    /// 睡觉
    /// </summary>
    Sleep,

    /// <summary>
    /// 沉睡
    /// </summary>
    Slumber,
}


/// <summary>
/// 战斗动画
/// </summary>
public enum BattleAnimType
{
    /// <summary>
    /// 死亡
    /// </summary>
    Decease,

    /// <summary>
    /// 防御
    /// </summary>
    Defense,

    /// <summary>
    /// 濒死
    /// </summary>
    Dying,

    /// <summary>
    /// 受击
    /// </summary>
    Hurt,

    /// <summary>
    /// 闲置
    /// </summary>
    Idle,

    /// <summary>
    /// 普通攻击
    /// </summary>
    NormalAttack,

    /// <summary>
    /// 技能
    /// </summary>
    Skill,

    /// <summary>
    /// 特殊技能
    /// </summary>
    SpecialSkill
}