using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/Actions/Attack")]
public class AIActionAttack : AIActionSO
{
    public bool normalAttack = true;
    public bool specialAttack = false;

    public override void Enter(AIController aiCtr)
    {
        base.Enter(aiCtr);

        aiCtr.inputDirection = aiCtr.TargetDirection;
    }

    public override void Execute(AIController aiCtr)
    {
        aiCtr.inputDirection = aiCtr.TargetDirection;

        aiCtr.Enemy.Attack = normalAttack;
        aiCtr.Enemy.Special = specialAttack;
    }
}