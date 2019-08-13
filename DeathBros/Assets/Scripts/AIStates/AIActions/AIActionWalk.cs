using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/Actions/Walk")]
public class AIActionWalk : AIActionSO
{
    public bool turnOnLedge = true;
    public bool turnOnWall = true;

    protected Vector2 right = Vector2.right;
    protected Vector2 left = Vector2.left;

    public string nextState = "";

    public float minTimeInSec = 1;
    public float maxTimeInSec = 2;

    public override void Enter(AIController aiCtr)
    {
        base.Enter(aiCtr);

        if (aiCtr.inputDirection != Vector2.right && aiCtr.inputDirection != Vector2.left)
        {
            aiCtr.inputDirection = right;
        }

        aiCtr.RandomTimerNumber = Random.Range(minTimeInSec * 60, maxTimeInSec * 60);
    }

    public override void Execute(AIController aiCtr)
    {
        base.Execute(aiCtr);


        if (turnOnLedge)
        {
            if (aiCtr.Enemy.Ctr.OnLedge)
            {
                if (aiCtr.inputDirection == right) aiCtr.inputDirection = left;
                else aiCtr.inputDirection = right;
            }
        }

        if (turnOnWall)
        {
            if (aiCtr.Enemy.Ctr.OnWall)
            {
                if (aiCtr.inputDirection == right) aiCtr.inputDirection = left;
                else aiCtr.inputDirection = right;
            }
        }

        aiCtr.Enemy.SetInputs(aiCtr.inputDirection);

        if (nextState != "")
        {
            if (aiCtr.Timer > aiCtr.RandomTimerNumber)
            {
                aiCtr.ChangeState(nextState);
            }
        }
    }
}