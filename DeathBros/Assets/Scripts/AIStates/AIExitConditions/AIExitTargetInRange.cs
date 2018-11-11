using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/ExitConditions/TargetInRange")]
public class AIExitTargetInRange : AIExitConditionSO
{
    public float targetRange;

    public override void CheckForExit(AIController aiCtr)
    {
        if (aiCtr.DistanceToTarget <= targetRange) aiCtr.ChangeState(exitState);
    }
}
