using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/Actions/Walk")]
public class AIActionWalk : AIActionSO
{
    public bool turnOnLedge = true;
    public bool turnOnWall = true;

    protected Vector2 right = Vector2.right;
    protected Vector2 left = Vector2.left;

    public override void Enter(AIController aiCtr)
    {
        base.Enter(aiCtr);

        if (aiCtr.inputDirection != Vector2.right && aiCtr.inputDirection != Vector2.left)
            aiCtr.inputDirection = right;
    }

    public override void Execute(AIController aiCtr)
    {
        base.Execute(aiCtr);


        if (turnOnLedge)
        {
            if (aiCtr.Enemy.Ctr.onLedge)
            {
                if (aiCtr.inputDirection == right) aiCtr.inputDirection = left;
                else aiCtr.inputDirection = right;
            }
        }

        if (turnOnWall)
        {
            if (aiCtr.Enemy.Ctr.onWall)
            {
                if (aiCtr.inputDirection == right) aiCtr.inputDirection = left;
                else aiCtr.inputDirection = right;
            }
        }

        aiCtr.Enemy.SetInputs(aiCtr.inputDirection);
    }
}