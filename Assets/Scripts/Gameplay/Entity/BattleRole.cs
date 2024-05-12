using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;

/// <summary>
/// 战斗角色，模仿原版战斗切换
/// </summary>
public sealed class BattleRole : Role
{
    #region
    private const int BLOCK_CHANCE = 25;

    /// <summary>
    /// 近战及特殊行动移动时长，可能改成速度更平滑
    /// </summary>
    private const float MELEE_ACTION_MOVE_DURATION = 0.25f;

    /// <summary>
    /// 近战及特殊行动移动目标间距
    /// </summary>
    private const float MELEE_ACTION_MOVE_DISTANCE = 0.5f;

    /// <summary>
    /// 击退时长
    /// </summary>
    private const float ATTACK_BACK_DURATION = 0.25f;

    /// <summary>
    /// 敌人死亡时长
    /// </summary>
    private const int HOSTILE_DECEASE_DURATION = 1;

    /// <summary>
    /// 击退方向
    /// </summary>
    private static readonly Vector3 HIT_DIRECTION = new(-0.1f, 0.1f);

    /// <summary>
    /// 受击颜色集合
    /// </summary>
    private static readonly Dictionary<NatureType, Color> HIT_COLOR_DIC = new()
    {
        { NatureType.PhysX, Color.red },
        { NatureType.Wind, Color.white },
        { NatureType.Thunder, Color.yellow },
        { NatureType.Water, Color.blue },
        { NatureType.Fire, Const.FIRE },
        { NatureType.Soil, Const.SOIL },
        { NatureType.Poison, Color.magenta },
        { NatureType.Witchery, Const.GREYISH },
        { NatureType.Recover, Color.green }
    };

    /// <summary>
    /// 行动处理集合
    /// </summary>
    private readonly Dictionary<BattleActionType, UnityEngine.Events.UnityAction> _actionDic = new();

    /// <summary>
    /// 初始位置
    /// </summary>
    private Vector2 _initialP;

    /// <summary>
    /// 音源
    /// </summary>
    private AudioSource _audioS;

    /// <summary>
    /// 我方实体角色，用于换装/战果写回
    /// </summary>
    public Role RoleEntity { get; private set; }

    /// <summary>
    /// 部署完毕
    /// </summary>
    public bool Deployed { get; set; }

    /// <summary>
    /// 行动中
    /// </summary>
    public bool Actioning { get; private set; }

    /// <summary>
    /// 受击中
    /// </summary>
    public bool Hitting { get; private set; }

    /// <summary>
    /// 战斗状态
    /// </summary>
    public RoleBattleState BattleState { get; private set; }

    /// <summary>
    /// 目标
    /// </summary>
    private BattleRole _target;

    /// <summary>
    /// 战斗行动
    /// </summary>
    private BattleAction _battleAction;

    /// <summary>
    /// 受击方向
    /// </summary>
    private Vector3 _hitDirection;

    /// <summary>
    /// 防御姿态，你的护甲毫无作用，你的信仰一文不值
    /// </summary>
    public bool Defensing { get; set; }

    /// <summary>
    /// 格挡
    /// </summary>
    private bool _block;

    /// <summary>
    /// 受击伤害类型
    /// </summary>
    private NatureType _hitDamageType;

    /// <summary>
    /// 受击伤害值
    /// </summary>
    private int _hitDamageValue;

    /// <summary>
    /// 受击动画时长
    /// </summary>
    private float _hitDuration;

    /// <summary>
    /// 暴击
    /// </summary>
    private bool _critical;

    /// <summary>
    /// Buff集合
    /// </summary>
    public readonly List<BuffData> BuffList = new();

    private TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> _tempCore;

    private Sprite[] _roleAnim;

    private ItemData _itemData;

    private SkillData _skillData;

    private Sprite[] _skillAnim;

    private List<BattleRole> _targetList;

    private Vector3 _offsetP = new Vector3(0, 0, 0.4f);

    private SpriteRenderer _effect;

    private bool _twoHand;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        
        _actionDic.Add(BattleActionType.NormalAttack, () => StartCoroutine(nameof(NormalAttackActionC)));
        _actionDic.Add(BattleActionType.Item, () => StartCoroutine(nameof(ItemActionC)));
        //_actionDic.Add(BattleActionType.Defense, () => Defensing = true);
        _actionDic.Add(BattleActionType.Retreat, () => StartCoroutine(nameof(RetreatActionC)));
        _actionDic.Add(BattleActionType.Skill, () => StartCoroutine(nameof(SkillActionC)));
        _actionDic.Add(BattleActionType.JointAttack, () => StartCoroutine(nameof(SkillActionC)));

        GC(ref _audioS);
    }

    protected override void Start()
    {
        base.Start();

        BattleField.ActionStageEnd += RoundEnd;
    }

    /// <summary>
    /// 战斗初始化
    /// </summary>
    public void BattleInit(Vector2 position, Role roleEntity = null)
    {
        Transform.SetLocalPositionAndRotation(position, Quaternion.identity);
        _initialP = Transform.localPosition;

        BattleDataClone(null == roleEntity ? null : RoleEntity = roleEntity);
        BattleStateAnimUpdate();
        SortingOrder();
    }

    /// <summary>
    /// 战斗状态更新
    /// </summary>
    private void BattleStateUpdate()
    {
        if (0 == HP)
            BattleState = RoleBattleState.Decease;
        else if (HP * 5 < HPMax)
            BattleState = RoleBattleState.Dying;
        else
            BattleState = RoleBattleState.Idle;
    }

    /// <summary>
    /// 战斗状态动画更新
    /// </summary>
    private void BattleStateAnimUpdate()
    {
        BattleStateUpdate();

        Anim(new string[] { RoleData.ID.ToString(), (null == RoleEntity ? RoleBattleState.Idle : BattleState).ToString() });
    }

    /// <summary>
    /// 合击仙术
    /// </summary>
    public SkillData JointSkill
    {
        get
        {
            int skillID = DataManager_.ItemDataArray[OutfitDic[OutfitType.Bracers]].JointSkillID;
            if (-1 == skillID) skillID = RoleData.JointSkillID;

            return DataManager_.SkillDataArray[skillID];
        }
    }

    /// <summary>
    /// AI行动
    /// </summary>
    /// <returns></returns>
    public void AIAction()
    {
        BattleField.Confirm(new(this, BattleActionType.NormalAttack, -1, BattleField.HostileList.Random()));
    }

    /// <summary>
    /// 敌人行动
    /// </summary>
    public void HostileAction()
    {
        if (null == RoleData.DefaultSkill) AINormalAttack();
        else
        {
            _skillData = null;

            for (int i = 0; i != RoleData.DefaultSkill.Length; i++)
            {
                if (DataManager_.SkillDataArray[RoleData.DefaultSkill[i]].Cost <= MP)
                {
                    _skillData = DataManager_.SkillDataArray[RoleData.DefaultSkill[i]];

                    break;
                }
            }

            if (null == _skillData) AINormalAttack();
            else
            {
                for (int i = 0; i != RoleData.DefaultSkill.Length; i++)
                {
                    if (DataManager_.SkillDataArray[RoleData.DefaultSkill[i]].Cost <= MP && _skillData.Cost < DataManager_.SkillDataArray[RoleData.DefaultSkill[i]].Cost)
                    {
                        _skillData = DataManager_.SkillDataArray[RoleData.DefaultSkill[i]];
                    }
                }

                BattleField.Confirm(new(this, BattleActionType.Skill, _skillData.ID, BattleField.PlayerList.Random()));
            }
        }

        if (RoleData.DoubleKill) AINormalAttack();


        void AINormalAttack() => BattleField.Confirm(new(this, BattleActionType.NormalAttack, -1, BattleField.PlayerList.Random()));
    }

    /// <summary>
    /// 行动
    /// </summary>
    public void Action(BattleAction battleAction)
    {
        _battleAction = battleAction;

        Actioning = true;

        //ToolsE.LogWarning(RoleData.Name + "  action  " + _battleAction.ActionType + "  " + _battleAction.SourceID + "  " + _battleAction.Target);

        TargetCheck();

        _actionDic[_battleAction.ActionType]();
    }

    /// <summary>
    /// 目标检查
    /// </summary>
    private void TargetCheck()
    {
        if (null == (_target = _battleAction.Target))
        {
            _target = (null == RoleEntity ? BattleField.PlayerList : BattleField.HostileList)[0];
        }
        else
        {
            while (RoleBattleState.Decease == _target.BattleState)
                _target = null == RoleEntity ? BattleField.PlayerList.Random() : BattleField.HostileList.Random();
        }
    }

    /// <summary>
    /// Buff添加
    /// </summary>
    /// <param name="buffChance"></param>
    /// <param name="buffID"></param>
    private void BuffAdd(int buffChance, int buffID)
    {
        if (buffChance.Random())
            new BuffData(buffID, buffID.ToString()).Init(_target);
    }

    /// <summary>
    /// 普通攻击行动
    /// </summary>
    private IEnumerator NormalAttackActionC()
    {
        StopCoroutine(nameof(AnimationC));

        _critical = RoleData.Critical.Random();
        _roleAnim = RoleData.CurrentBattleAnimDic[RoleBattleState.NormalAttack];

        for (int i = 0; i != _roleAnim.Length; i++)
        {
            SpriteRenderer.sprite = _roleAnim[i];

            if (RoleData.CurrentMeleeAudioKeyFrame == i)
            {
                _audioS.clip = _critical ? RoleData.CriticalAudio : RoleData.MeleeAudio;
                _audioS.Play();
            }

            if (RoleData.CurrentMeleeMoveKeyFrame == i && (null == RoleEntity || !DataManager_.ItemDataArray[RoleEntity.OutfitDic[OutfitType.Weapon]].Ranged))
            {
                _tempCore.Kill();
                _tempCore = Transform.DOLocalMove(_target.Transform.localPosition, MELEE_ACTION_MOVE_DURATION);

                while (MELEE_ACTION_MOVE_DISTANCE < Vector3.Distance(Transform.localPosition, _target.Transform.localPosition))
                    yield return null;

                _tempCore.Kill();
            }
            
            if (RoleData.CurrentMeleeAttackKeyFrame == i)
            {
                //双手武器二段攻击，连击由战斗管理
                _twoHand = null != RoleEntity && DataManager_.ItemDataArray[RoleEntity.OutfitDic[OutfitType.Weapon]].TwoHand;

                if (null == RoleEntity || !DataManager_.ItemDataArray[RoleEntity.OutfitDic[OutfitType.Weapon]].Ranged)
                {
                    //武术/抗性/暴击计算_critical
                    _target.Hit(NatureType.PhysX, Attack - _target.Defense, ATTACK_BACK_DURATION);

                    if (RoleData.BuffChance.Random())
                        new BuffData(RoleData.BuffID, RoleData.BuffID.ToString()).Init(_target);
                }
                else
                {
                    _targetList = null == RoleEntity ? BattleField.PlayerList : BattleField.HostileList;
                    _target = _targetList[0];

                    for (int j = 0; j != _targetList.Count; j++)
                    {
                        if (RoleBattleState.Decease != _targetList[j].BattleState)
                        {
                            _targetList[j].Hit(NatureType.PhysX, Attack - _target.Defense, ATTACK_BACK_DURATION);

                            if (RoleData.BuffChance.Random())
                                new BuffData(RoleData.BuffID, RoleData.BuffID.ToString()).Init(_targetList[j]);
                        }
                    }
                }
            }

            yield return Const.ANIMATION_PLAY_SPEED;
        }

        if (_twoHand)
        {
            SpriteRenderer.sprite = _roleAnim[RoleData.CurrentMeleeAttackKeyFrame - 1];
            _tempCore.Kill();
            _tempCore = Transform.DOLocalMove(Transform.localPosition - HIT_DIRECTION * 0.5f, MELEE_ACTION_MOVE_DURATION);

            yield return Const.WAIT_FOR_HS;

            if (RoleBattleState.Decease != _target.BattleState)
            {
                SpriteRenderer.sprite = _roleAnim[RoleData.CurrentMeleeAttackKeyFrame];
                _tempCore.Kill();
                _tempCore = Transform.DOLocalMove(Transform.localPosition + HIT_DIRECTION * 0.5f, MELEE_ACTION_MOVE_DURATION);

                _target.Hit(NatureType.PhysX, Attack - _target.Defense, ATTACK_BACK_DURATION);
            }
        }

        yield return Const.SPECIAL_ANIMATION_PLAY_SPEED;
        _tempCore.Kill();
        Transform.localPosition = _initialP;
        BattleStateAnimUpdate();

        while (_target.Hitting) yield return Const.WAIT_FOR_HS;

        Actioning = false;
    }

    /// <summary>
    /// 物品行动
    /// </summary>
    private IEnumerator ItemActionC()
    {
        StopCoroutine(nameof(AnimationC));

        _roleAnim = RoleData.CurrentBattleAnimDic[RoleBattleState.Skill];
        _itemData = DataManager_.ItemDataArray[_battleAction.SourceID];
        _skillData = DataManager_.SkillDataArray[_itemData.EffectSkillID];
        _targetList = _itemData.Throw ? BattleField.HostileList : BattleField.PlayerList;
        if (_itemData.Effect2All) _target = _targetList[0];

        for (int i = 0; i != _roleAnim.Length; i++)
        {
            SpriteRenderer.sprite = _roleAnim[i];

            if (RoleData.CurrentRangedAudioKeyFrame == i)
            {
                _audioS.clip = RoleData.RangedAudio;
                _audioS.Play();

                (_effect = BattleField.EffectGet()).transform.localPosition = _target.Transform.localPosition;
                _skillAnim = DataManager_.SkillEffectList[_skillData.EffectID];
                for (int j = 0; j != _skillAnim.Length; j++)
                {
                    _effect.sprite = _skillAnim[j];

                    if (_skillData.SkillKeyFrame == j)
                    {
                        //仙术音效还没找到
                        GameManager_.Trigger(GameEventType.ItemAdd, _itemData.ID.ToString(), "-1");

                        if (_itemData.Throw)
                        {
                            if (_itemData.Effect2All)
                            {
                                for (int k = 0; k != _targetList.Count; k++)
                                    ItemThrow(_targetList[k]);
                            }
                            else ItemThrow(_target);


                            void ItemThrow(BattleRole role)
                            {
                                role.Hit(_skillData.DamageType, _itemData.Damage, ATTACK_BACK_DURATION);

                                if (_itemData.BuffChance.Random())
                                    new BuffData(_itemData.BuffID, _itemData.BuffID.ToString()).Init(role);
                            }
                        }
                        else
                        {
                            int hp = 0, mp = 0, resurrect = 0;
                            List<BuffType> debuffClearList = new();
                            ItemDataSet();

                            if (_itemData.Effect2All)
                            {
                                for (int k = 0; k != _targetList.Count; k++)
                                    ItemApply(_targetList[k]);
                            }
                            else ItemApply(_target);


                            void ItemDataSet()
                            {
                                for (int k = 0; k != _itemData.EventArray.Length; k++)
                                {
                                    switch (_itemData.EventArray[k].RoleEffectType)
                                    {
                                        case RoleEffectType.DebuffClear:
                                            debuffClearList.Add((BuffType)_itemData.EventArray[k].Value);
                                            break;
                                        case RoleEffectType.Resurrect:
                                            resurrect = _itemData.EventArray[k].Value;
                                            break;
                                        case RoleEffectType.HPAdd:
                                            hp = _itemData.EventArray[k].Value;
                                            break;
                                        case RoleEffectType.MPAdd:
                                            mp = _itemData.EventArray[k].Value;
                                            break;
                                    }
                                }
                            }


                            void ItemApply(BattleRole role)
                            {
                                if (RoleBattleState.Decease == role.BattleState)
                                {
                                    if (0 != resurrect) Resurrect(resurrect);
                                }
                                else
                                {
                                    role.Heal(_skillData.DamageType, hp, mp);

                                    if (_itemData.BuffChance.Random())
                                        new BuffData(_itemData.BuffID, _itemData.BuffID.ToString()).Init(role);

                                    for (int k = 0; k != debuffClearList.Count; k++)
                                    {
                                        for (int l = 0; l != role.BuffList.Count; l++)
                                        {
                                            if (debuffClearList[k] == role.BuffList[l].BuffType)
                                                BuffList.RemoveAt(l--);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    yield return Const.SKILL_PLAY_SPEED;
                }

                if (!_skillData.Terrain)
                    _effect.transform.position = Const.HIDDEN_P;
            }

            yield return Const.ANIMATION_PLAY_SPEED;
        }

        yield return Const.SPECIAL_ANIMATION_PLAY_SPEED;
        BattleStateAnimUpdate();

        while (_target.Hitting) yield return Const.WAIT_FOR_HS;

        Actioning = false;
    }

    /// <summary>
    /// 撤退行动
    /// </summary>
    /// <returns></returns>
    private IEnumerator RetreatActionC()
    {
        _targetList = null == RoleEntity ? BattleField.PlayerList : BattleField.HostileList;

        int luck = Random.Range(0, Luck + 1);

        for (int i = 0; i != _targetList.Count; i++)
        {
            if (luck < Random.Range(0, _targetList[i].Luck + 1))
            {
                //撤退失败动画
                Actioning = false;

                StopCoroutine(nameof(RetreatActionC));
            }
        }

        //撤退成功动画
        //BattleField.Instance.BattleEnd(true);

        yield return null;
    }

    /// <summary>
    /// 技能行动
    /// </summary>
    private IEnumerator SkillActionC()
    {
        StopCoroutine(nameof(AnimationC));

        _roleAnim = RoleData.CurrentBattleAnimDic[RoleBattleState.Skill];
        _skillData = DataManager_.SkillDataArray[_battleAction.SourceID];
        if (null == RoleEntity && !_skillData.Attack || null != RoleEntity && _skillData.Attack)
            _targetList = BattleField.HostileList;
        else
            _targetList = BattleField.HostileList;
        if (_skillData.Effect2All) _target = _targetList[0];

        if (2 == _skillData.ID)
        {
            //飞龙探云手
        }

        if (999 == _skillData.ID)
        {
            //金蝉脱壳
        }

        if (BattleActionType.JointAttack == _battleAction.ActionType && -1 == DataManager_.ItemDataArray[OutfitDic[OutfitType.Bracers]].JointSkillID)
        {
            //合击站位
        }

        for (int i = 0; i != _roleAnim.Length; i++)
        {
            SpriteRenderer.sprite = _roleAnim[i];

            if (RoleData.CurrentRangedAudioKeyFrame == i)
            {
                _audioS.clip = RoleData.RangedAudio;
                _audioS.Play();

                if (-1 == _skillData.SummonID)
                {
                    if (-1 != _skillData.EffectID)
                    {
                        (_effect = BattleField.EffectGet()).transform.localPosition = _target.Transform.localPosition;
                        _skillAnim = DataManager_.SkillEffectList[_skillData.EffectID];

                        for (int j = 0; j != _skillAnim.Length; j++)
                        {
                            _effect.sprite = _skillAnim[j];

                            if (_skillData.SkillKeyFrame == j)
                            {
                                //仙术音效还没找到
                                Trigger(RoleEffectType.MPAdd, -_skillData.Cost);

                                if (_skillData.Attack)
                                {
                                    if (_skillData.Effect2All)
                                    {
                                        for (int k = 0; k != _targetList.Count; k++)
                                            ItemThrow(_targetList[k]);
                                    }
                                    else ItemThrow(_target);


                                    void ItemThrow(BattleRole role)
                                    {
                                        role.Hit(_skillData.DamageType, _skillData.Damage, ATTACK_BACK_DURATION);

                                        if (_skillData.BuffChance.Random())
                                            new BuffData(_skillData.BuffID, _skillData.BuffID.ToString()).Init(role);
                                    }
                                }
                                else
                                {
                                    int hp = 0, mp = 0, resurrect = 0;
                                    List<BuffType> debuffClearList = new();
                                    SkillDataSet();

                                    if (_skillData.Effect2All)
                                    {
                                        for (int k = 0; k != _targetList.Count; k++)
                                            SkillApply(_targetList[k]);
                                    }
                                    else SkillApply(_target);


                                    void SkillDataSet()
                                    {
                                        for (int k = 0; k != _skillData.EventArray.Length; k++)
                                        {
                                            switch (_skillData.EventArray[k].RoleEffectType)
                                            {
                                                case RoleEffectType.DebuffClear:
                                                    debuffClearList.Add((BuffType)_skillData.EventArray[k].Value);
                                                    break;
                                                case RoleEffectType.Resurrect:
                                                    resurrect = _skillData.EventArray[k].Value;
                                                    break;
                                                case RoleEffectType.HPAdd:
                                                    hp = _skillData.EventArray[k].Value;
                                                    break;
                                                case RoleEffectType.MPAdd:
                                                    mp = _skillData.EventArray[k].Value;
                                                    break;
                                            }
                                        }
                                    }


                                    void SkillApply(BattleRole role)
                                    {
                                        if (RoleBattleState.Decease == role.BattleState)
                                        {
                                            if (0 != resurrect) Resurrect(resurrect);
                                        }
                                        else
                                        {
                                            role.Heal(_skillData.DamageType, hp, mp);

                                            if (_skillData.BuffChance.Random())
                                                new BuffData(_skillData.BuffID, _skillData.BuffID.ToString()).Init(role);

                                            for (int k = 0; k != debuffClearList.Count; k++)
                                            {
                                                for (int l = 0; l != role.BuffList.Count; l++)
                                                {
                                                    if (debuffClearList[k] == role.BuffList[l].BuffType)
                                                        BuffList.RemoveAt(l--);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            yield return Const.SKILL_PLAY_SPEED;
                        }

                        if (!_skillData.Terrain) _effect.transform.position = Const.HIDDEN_P;
                    }
                }
                else
                {
                    //召唤后施放仙术
                }
            }

            yield return Const.ANIMATION_PLAY_SPEED;
        }

        yield return Const.SPECIAL_ANIMATION_PLAY_SPEED;
        BattleStateAnimUpdate();

        while (_target.Hitting) yield return Const.WAIT_FOR_HS;

        Actioning = false;
    }

    /// <summary>
    /// 受击中
    /// </summary>
    /// <param name="damageType">伤害类型</param>
    /// <param name="damageValue">伤害值</param>
    /// <param name="duration">持续时常</param>
    public void Hit(NatureType damageType, int damageValue, float duration)
    {
        Hitting = true;

        _block = null != RoleEntity && NatureType.PhysX == damageType &&/* !HasBuff(BuffType.Coma, BuffType.Crazy)&&*/ (BLOCK_CHANCE * (Defensing ? 2 : 1)).Random();

        _hitDamageType = damageType;
        _hitDamageValue = damageValue <= 0 ? 0 : _block ? 1 : damageValue;
        _hitDuration = duration;
        _hitDirection = HIT_DIRECTION * duration;

        if (0 < _hitDamageValue) BattleField.UIDamageDisplay().Display(HIT_COLOR_DIC[_hitDamageType], ToolsE.W2S(Transform.position + _offsetP), _hitDamageValue.ToString());
        Trigger(RoleEffectType.HPAdd, -_hitDamageValue);
        
        if (null != RoleEntity) Anim(new string[] { RoleData.ID.ToString(), (_block ? RoleBattleState.Defense : RoleBattleState.Hurt).ToString() });
        _tempCore.Kill();
        (_tempCore = Transform.DOLocalMove(Transform.localPosition + (null == RoleEntity ? _hitDirection : -_hitDirection), _hitDuration)).onComplete = HitDone;

        StartCoroutine(nameof(HitFlashC));
    }

    /// <summary>
    /// 受击结束
    /// </summary>
    private void HitDone()
    {
        _tempCore.Kill();
        Transform.DOLocalMove(_initialP, _hitDuration).onComplete = () =>
        {
            BattleStateAnimUpdate();
            if (0 == HP) Decease();
            Hitting = false;
        };
    }

    /// <summary>
    /// 受击闪烁
    /// </summary>
    private IEnumerator HitFlashC()
    {
        SpriteRenderer.color = HIT_COLOR_DIC[_hitDamageType];

        yield return Const.WAIT_FOR_POINT_1S;
        yield return Const.WAIT_FOR_POINT_1S;

        SpriteRenderer.color = Color.white;
    }

    /// <summary>
    /// 恢复
    /// </summary>
    /// <param name="damageType"></param>
    /// <param name="hpValue"></param>
    private void Heal(NatureType damageType, int hpValue, int mpValue = 0)
    {
        _hitDamageType = damageType;
        _hitDamageValue = hpValue;
        if (HPMax - HP < _hitDamageValue) _hitDamageValue = HPMax - HP;
        Trigger(RoleEffectType.HPAdd, _hitDamageValue);
        if (0 != mpValue)
        {
            if (MPMax - MP < mpValue) mpValue = MPMax - MP;
            Trigger(RoleEffectType.MPAdd, mpValue);
        }

        StartCoroutine(nameof(HitFlashC));
        if (0 < _hitDamageValue) BattleField.UIDamageDisplay().Display(HIT_COLOR_DIC[_hitDamageType], ToolsE.W2S(Transform.position + _offsetP), _hitDamageValue.ToString(), 0 == mpValue ? null : mpValue.ToString());
        BattleStateAnimUpdate();
    }

    /// <summary>
    /// 死亡
    /// </summary>
    private void Decease()
    {
        StopCoroutine(nameof(AnimationC));

        BattleField.RoleDecease(this, null == RoleEntity);

        if (null == RoleEntity)
        {
            SpriteRenderer.DOFade(0, HOSTILE_DECEASE_DURATION);
        }
        else
        {
            BuffList.Clear();
        }
    }

    /// <summary>
    /// 复活
    /// </summary>
    public void Resurrect(int hp = 100)
    {
        HP = (int)(HPMax * hp * 0.01f).Round();
        Heal(NatureType.Recover, HP, 0);
        BattleStateAnimUpdate();
    }

    /// <summary>
    /// 敌人选择
    /// </summary>
    /// <param name="sw1tch">开关</param>
    public void HostileSelect(bool sw1tch)
    {
        if (sw1tch) StartCoroutine(nameof(SelectFlashC));
        else
        {
            StopCoroutine(nameof(SelectFlashC));

            SpriteRenderer.color = Color.white;
        }
    }

    /// <summary>
    /// 选择闪烁
    /// </summary>
    private IEnumerator SelectFlashC()
    {
        while (true)
        {
            SpriteRenderer.color = Const.GREYISH;

            yield return Const.WAIT_FOR_POINT_1S;

            SpriteRenderer.color = Color.white;

            yield return Const.WAIT_FOR_POINT_1S;
        }
    }

    /// <summary>
    /// 回合结束
    /// </summary>
    private void RoundEnd()
    {
        Defensing = false;

        for (int i = 0; i != BuffList.Count; i++)
        {
            if (BuffList[i].Handle())
                BuffList.RemoveAt(i--);
        }
    }

    /// <summary>
    /// Buff携带
    /// </summary>
    /// <param name="buffTypeArray">类型集合</param>
    /// <returns>是/否</returns>
    private bool HasBuff(params BuffType[] buffTypeArray)
    {
        for (int i = 0; i != buffTypeArray.Length; i++)
        {
            for (int j = 0; j != BuffList.Count; j++)
            {
                if (BuffList[j].BuffType == buffTypeArray[i])
                    return true;
            }
        }

        return false;
    }

    protected override void SortingOrder() => SpriteRenderer.sortingOrder = (short)(Transform.localPosition.y * -100 + 30100);
}


/// <summary>
/// 属性
/// </summary>
public enum NatureType
{
    /// <summary>
    /// 物
    /// </summary>
    PhysX,

    /// <summary>
    /// 风
    /// </summary>
    Wind,

    /// <summary>
    /// 雷
    /// </summary>
    Thunder,

    /// <summary>
    /// 水
    /// </summary>
    Water,

    /// <summary>
    /// 火
    /// </summary>
    Fire,

    /// <summary>
    /// 土
    /// </summary>
    Soil,

    /// <summary>
    /// 毒
    /// </summary>
    Poison,

    /// <summary>
    /// 巫
    /// </summary>
    Witchery,

    /// <summary>
    /// 恢复
    /// </summary>
    Recover
}