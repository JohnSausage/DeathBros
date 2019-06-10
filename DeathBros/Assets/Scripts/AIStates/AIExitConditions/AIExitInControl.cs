using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/ExitConditions/InControl")]
public class AIExitInControl : AIExitConditionSO
{
    public override void CheckForExit(AIController aiCtr)
    {
        if (aiCtr.Enemy.isInControl == true)
        {
            aiCtr.ChangeState(exitState);
        }
    }
}
