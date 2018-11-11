using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/Actions/Idle")]
public class AIActionIdle : AIActionSO
{
    public string nextState = "walk";

    public float minTimeInSec = 1;
    public float maxTimeInSec = 2;

    public override void Enter(AIController aiCtr)
    {
        base.Enter(aiCtr);

        aiCtr.Timer = 0;
        aiCtr.RandomTimerNumber = Random.Range(minTimeInSec * 60, maxTimeInSec * 60);
    }
    public override void Execute(AIController aiCtr)
    {
        aiCtr.Timer++;

        aiCtr.Enemy.SetInputs(Vector2.zero);

        if (aiCtr.Timer > aiCtr.RandomTimerNumber) aiCtr.ChangeState(nextState);
    }
}
