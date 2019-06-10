using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/ExitConditions/DamageTaken")]
public class AIExitDamageTaken : AIExitConditionSO
{
    public override void CheckForExit(AIController aiCtr)
    {
        if (aiCtr.Enemy.isTakingDamage == true)
        {
            aiCtr.ChangeState(exitState);
        }
    }
}
