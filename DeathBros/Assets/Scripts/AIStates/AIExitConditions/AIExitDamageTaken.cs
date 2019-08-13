using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/ExitConditions/DamageTaken")]
public class AIExitDamageTaken : AIExitConditionSO
{
    public override bool CheckForExit(AIController aiCtr)
    {
        if (aiCtr.Enemy.IsTakingDamage == true)
        {
            aiCtr.ChangeState(exitState);
            return true;
        }

        return false;
    }
}
