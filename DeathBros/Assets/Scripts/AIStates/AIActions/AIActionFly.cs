using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/Actions/Fly")]
public class AIActionFly : AIActionSO
{
    public bool turnOnWall = true;
    public float yMovement = 1;
    public float yMovementDurationInSec = 1;

    [Space]

    public float collsisionCheckRange = 2;
    public LayerMask collisionMask;

    protected Vector2 right = Vector2.right;
    protected Vector2 left = Vector2.left;

    public override void Enter(AIController aiCtr)
    {
        base.Enter(aiCtr);

        if (aiCtr.inputDirection != Vector2.right && aiCtr.inputDirection != Vector2.left)
        {
            aiCtr.inputDirection = right;
        }

        aiCtr.MovementTimer = 0;

        aiCtr.Enemy.Shield = false;
        aiCtr.Enemy.HoldShield = false;
    }

    public override void Execute(AIController aiCtr)
    {
        base.Execute(aiCtr);

        aiCtr.MovementTimer++;

        if (aiCtr.Enemy.Ctr.IsGrounded)
        {
            aiCtr.inputDirection.y = 1f;
            aiCtr.Enemy.Jump = true;
        }
        else
        {
            aiCtr.Enemy.Jump = false;

            if (aiCtr.MovementTimer < yMovementDurationInSec * 60)
            {
                aiCtr.inputDirection.y = yMovement;
            }
            else if (aiCtr.MovementTimer < 2 * yMovementDurationInSec * 60)
            {
                aiCtr.inputDirection.y = -yMovement;
            }
            else
            {
                aiCtr.MovementTimer = 0;
            }
        }

        if (turnOnWall)
        {
            if (aiCtr.Enemy.Ctr.OnWall)
            {
                TurnAround(aiCtr);
            }
        }

        if (collsisionCheckRange > 0)
        {
            RaycastHit2D collisionSearch = Physics2D.CircleCast(aiCtr.Enemy.Position, collsisionCheckRange, Vector2.zero, 0, collisionMask);

            if (collisionSearch == true)
            {
                aiCtr.inputDirection.y = collisionSearch.normal.y;

                if (collisionSearch.normal.x != 0)
                {
                    aiCtr.inputDirection.x = collisionSearch.normal.x;
                }
            }
            else
            {
                aiCtr.inputDirection.x = aiCtr.Enemy.Direction;
            }
        }

        aiCtr.Enemy.SetInputs(aiCtr.inputDirection);
    }

    protected void TurnAround(AIController aiCtr)
    {
        if (aiCtr.inputDirection.x != 0)
        {
            aiCtr.inputDirection.x *= -1;
        }
    }
}