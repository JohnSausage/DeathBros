using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/ExitConditions/DamageTaken")]
public class AIExitDamageTaken : AIExitConditionSO
{
    public override bool CheckForExit(AIController aiCtr)
    {
        if (aiCtr.Enemy.isTakingDamage == true)
        {
            aiCtr.ChangeState(exitState);
            return true;
        }

        return false;
    }
}
