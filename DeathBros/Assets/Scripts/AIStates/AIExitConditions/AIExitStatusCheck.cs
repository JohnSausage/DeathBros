﻿using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/ExitConditions/StatusCheck")]
public class AIExitStatusCheck : AIExitConditionSO
{
    public int waitFramesBeforeChecking = 0;

    [Space]

    public bool isGrounded;
    public bool hasCollided;
    public bool isOnWall;
    public bool isOnLedge;
    public bool isTakingDamage;
    public bool isInControl;
    public bool isInTumble;
    

    public override void CheckForExit(AIController aiCtr)
    {
        if(aiCtr.Timer < waitFramesBeforeChecking)
        {
            return;
        }

        if (isGrounded)
        {
            if (aiCtr.Enemy.Ctr.IsGrounded == true)
            {
                aiCtr.ChangeState(exitState);
            }
        }

        if (hasCollided)
        {
            if (aiCtr.Enemy.Ctr.HasCollided == true)
            {
                aiCtr.ChangeState(exitState);
            }
        }

        if (isOnWall)
        {
            if (aiCtr.Enemy.Ctr.OnWall == true)
            {
                aiCtr.ChangeState(exitState);
            }
        }

        if (isOnLedge)
        {
            if (aiCtr.Enemy.Ctr.OnLedge == true)
            {
                aiCtr.ChangeState(exitState);
            }
        }

        if (isTakingDamage)
        {
            if (aiCtr.Enemy.isTakingDamage == true)
            {
                aiCtr.ChangeState(exitState);
            }
        }

        if (isInControl)
        {
            if (aiCtr.Enemy.isInControl == true)
            {
                aiCtr.ChangeState(exitState);
            }
        }

        if (isInTumble)
        {
            if (aiCtr.Enemy.Ctr.IsInTumble == true)
            {
                aiCtr.ChangeState(exitState);
            }
        }
    }
}