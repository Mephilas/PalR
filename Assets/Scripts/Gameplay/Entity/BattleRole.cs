using UnityEngine;

/// <summary>
/// 战斗角色，模仿原版战斗切换
/// </summary>
public sealed class BattleRole : Role
{
    /// <summary>
    /// 我方实体角色，支持换装/战果写回
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
    /// 战斗初始化
    /// </summary>
    public void BattleInit(Vector2 position, Role roleEntity = null)
    {
        Transform.SetLocalPositionAndRotation(position, Quaternion.identity);

        SortingOrder();

        Anim(new string[] { RoleData.ID.ToString(), BattleAnimType.Idle.ToString() });

        BattleDataClone(null == roleEntity ? null : RoleEntity = roleEntity);
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
    public BattleAction AIAction()
    {
        Deployed = true;

        return new BattleAction(BattleActionType.NormalAttack, Random.Range(0, BattleField.HostileList.Count));
    }

    /// <summary>
    /// 敌人行动
    /// </summary>
    public BattleAction HostileAction()
    {
        Deployed = true;

        return new BattleAction(BattleActionType.NormalAttack, Random.Range(0, BattleField.PlayerList.Count));
    }

    /// <summary>
    /// 行动
    /// </summary>
    public void Action(BattleAction battleAction)
    {
        Actioning = true;

        ToolsE.LogWarning(RoleData.Name + "  action  " + battleAction.ActionType + "  " + battleAction.SourceID + "  " + battleAction.Target);

        //Actioning = false;
        Invoke(nameof(TTTT), 2);
    }

    private void TTTT() => Actioning = false;

    /// <summary>
    /// 敌人选择
    /// </summary>
    /// <param name="sw1tch">开关</param>
    public void HostileSelect(bool sw1tch)
    {
        if (sw1tch) StartCoroutine(nameof(FlashI));
        else
        {
            StopCoroutine(nameof(FlashI));

            SpriteRenderer.color = Color.white;
        }
    }

    private System.Collections.IEnumerator FlashI()
    {
        while (true)
        {
            SpriteRenderer.color = Const.GREYISH;

            yield return Const.WAIT_FOR_POINT_1S;

            SpriteRenderer.color = Color.white;

            yield return Const.WAIT_FOR_POINT_1S;
        }
    }

    protected override void SortingOrder() => SpriteRenderer.sortingOrder = (short)(Transform.localPosition.y * -100 + 30100);
}