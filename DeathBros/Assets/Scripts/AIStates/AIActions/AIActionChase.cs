using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/Actions/Chase")]
public class AIActionChase : AIActionSO
{
    public Vector2 targetOffset = Vector2.zero;

    [Space]

    public bool jumpOnWall;

    /// <summary>
    /// If this value is set to greater than 0, the AI will check for other enemies in the value range and walk away from them to keep the distance.
    /// </summary>
    public float keepDistanceToOthers = 0;

    public LayerMask keepDistanceMask;
    private int keepDistanceCounter = 0;

    public override void Execute(AIController aiCtr)
    {
        base.Execute(aiCtr);

        aiCtr.Enemy.SetInputs(new Vector2(Mathf.Sign(aiCtr.TargetDirection(targetOffset).x), 0));

        //only check for others if keepDistance > 0
        if (keepDistanceToOthers > 0)
        {
            //check every 20 frames for others
            keepDistanceCounter++;
            if (keepDistanceCounter >= 20)
            {
                keepDistanceCounter = 0;
                aiCtr.savedDirX = GetDirectionAwayFromOthers(aiCtr);
            }

            //overwrite inputs if keepDistance was set
            if(aiCtr.savedDirX != 0)
            {
                aiCtr.Enemy.SetInputs(new Vector2(aiCtr.savedDirX, 0));
            }
        }

        //check for walls and jump
        if (jumpOnWall == true)
        {
            if (aiCtr.Enemy.Ctr.OnWall == true)
            {
                aiCtr.Enemy.Jump = true;
                aiCtr.Enemy.HoldJump = true;
            }
            else
            {
                aiCtr.Enemy.Jump = false;
                aiCtr.Enemy.HoldJump = false;
            }
        }
    }

    protected float GetDirectionAwayFromOthers(AIController aiCtr)
    {
        RaycastHit2D[] checkForOthers = Physics2D.CircleCastAll(aiCtr.Enemy.Position, keepDistanceToOthers, Vector2.zero, 0, keepDistanceMask);

        float retVal = 0f;

        RaycastHit2D closest = new RaycastHit2D
        {
            distance = keepDistanceToOthers
        };

        foreach (RaycastHit2D hit in checkForOthers)
        {
            if (hit.transform.IsChildOf(aiCtr.Enemy.transform))
            {
                //continue if hit object is child of AI
                continue;
            }
            else
            {
                //else set new inputs and break out of the loop
                if(hit.distance < closest.distance)
                {
                    closest = hit;
                }
            }
        }

        //only set retVal to other than 0 if a new closest was found
        if(closest.distance < keepDistanceToOthers)
        {
            retVal = Mathf.Sign(aiCtr.Enemy.Position.x - closest.point.x);
        }

        return retVal;
    }
}
