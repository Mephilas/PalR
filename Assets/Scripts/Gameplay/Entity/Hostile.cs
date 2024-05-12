/// <summary>
/// 敌人
/// </summary>
public sealed class Hostile : Role
{
    /// <summary>
    /// 战斗结束
    /// </summary>
    public void BattleComplete() => Invoke(nameof(Resurrect), 60);

    /// <summary>
    /// 各位父老乡亲们没想到吧
    /// </summary>
    private void Resurrect() => GameManager_.Trigger(GameEventType.RoleState, "0");
}