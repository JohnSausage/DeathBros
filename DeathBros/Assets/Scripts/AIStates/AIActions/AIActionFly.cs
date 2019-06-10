using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/Actions/Fly")]
public class AIActionFly : AIActionSO
{
    public bool turnOnWall = true;
    public float yMovement = 1;
    public float yMovementDurationInSec = 1;

    protected Vector2 right = Vector2.right;
    protected Vector2 left = Vector2.left;

    public override void Enter(AIController aiCtr)
    {
        base.Enter(aiCtr);

        if (aiCtr.inputDirection != Vector2.right && aiCtr.inputDirection != Vector2.left)
            aiCtr.inputDirection = right;

        aiCtr.Timer = 0;
    }

    public override void Execute(AIController aiCtr)
    {
        base.Execute(aiCtr);

        aiCtr.Timer++;

        if (aiCtr.Enemy.Ctr.IsGrounded)
        {
            aiCtr.inputDirection.y = 1f;
        }
        else
        {
            if (aiCtr.Timer < yMovementDurationInSec * 60)
            {
                aiCtr.inputDirection.y = yMovement;
            }
            else if (aiCtr.Timer < 2 * yMovementDurationInSec * 60)
            {
                aiCtr.inputDirection.y = -yMovement;
            }
            else
            {
                aiCtr.Timer = 0;
            }
        }

        if (turnOnWall)
        {
            if (aiCtr.Enemy.Ctr.OnWall)
            {
                if (aiCtr.inputDirection.x == 1) aiCtr.inputDirection.x = -1;
                else aiCtr.inputDirection.x = 1;
            }
        }

        aiCtr.Enemy.SetInputs(aiCtr.inputDirection);
    }
}