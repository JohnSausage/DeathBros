using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/ExitConditions/TargetOutOfRange")]
public class AIExitTargetOutOfRange : AIExitConditionSO
{
    public float targetRange;

    public override bool CheckForExit(AIController aiCtr)
    {
        if (aiCtr.DistanceToTarget > targetRange)
        {
            aiCtr.ChangeState(exitState);
            return true;
        }

        return false;
    }
}