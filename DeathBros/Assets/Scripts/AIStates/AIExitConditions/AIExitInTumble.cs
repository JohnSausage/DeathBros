﻿using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/ExitConditions/InTumble")]
public class AIExitInTumble : AIExitConditionSO
{
    public override bool CheckForExit(AIController aiCtr)
    {
        if (aiCtr.Enemy.Ctr.IsInTumble == true)
        {
            aiCtr.ChangeState(exitState);
            return true;
        }

        return false;
    }
}
