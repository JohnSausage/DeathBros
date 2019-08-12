using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/ExitConditions/TargetInRange")]
public class AIExitTargetInRange : AIExitConditionSO
{
    public float targetRangeRadius;
    public Vector2 targetOffset;

    protected Vector2 directionalTargetOffset;

    public override bool CheckForExit(AIController aiCtr)
    {
        directionalTargetOffset.x = targetOffset.x * aiCtr.Enemy.Direction;
        directionalTargetOffset.y = targetOffset.y;

        if ((aiCtr.TargetVector - directionalTargetOffset).magnitude <= targetRangeRadius)
        {
            aiCtr.ChangeState(exitState);
            return true;
        }

        return false;
    }
}
