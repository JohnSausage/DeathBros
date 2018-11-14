using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/Actions/Attack")]
public class AIActionAttack : AIActionSO
{
    public bool normalAttack = true;
    public bool specialAttack = false;

    [Space]

    public Vector2 targetOffset = Vector2.zero;
    public bool moveToEnemy = true;

    public override void Enter(AIController aiCtr)
    {
        base.Enter(aiCtr);

        aiCtr.Enemy.SetInputs(aiCtr.TargetDirection(targetOffset));
    }

    public override void Execute(AIController aiCtr)
    {
        if (moveToEnemy)
            aiCtr.Enemy.SetInputs(aiCtr.TargetDirection(targetOffset));
        else
            aiCtr.Enemy.SetInputs(Vector2.zero);

        aiCtr.Enemy.Attack = normalAttack;
        aiCtr.Enemy.Special = specialAttack;
    }

    public override void Exit(AIController aiCtr)
    {
        aiCtr.Enemy.Attack = false;
        aiCtr.Enemy.Special = false;
    }
}