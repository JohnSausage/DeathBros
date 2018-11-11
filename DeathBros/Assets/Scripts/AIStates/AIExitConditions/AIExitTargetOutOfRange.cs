using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/ExitConditions/TargetOutOfRange")]
public class AIExitTargetOutOfRange : AIExitConditionSO
{
    public float targetRange;

    public override void CheckForExit(AIController aiCtr)
    {
        if (aiCtr.DistanceToTarget > targetRange) aiCtr.ChangeState(exitState);
    }
}