using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/Actions/Attack")]
public class AIActionAttack : AIActionSO
{
    public bool normalAttack = true;
    public bool specialAttack = false;

    public override void Execute(AIController aiCtr)
    {
        aiCtr.Enemy.SetInputs(aiCtr.TargetDirection);

        aiCtr.Enemy.Attack = normalAttack;
        aiCtr.Enemy.Special = specialAttack;
    }

    public override void Exit(AIController aiCtr)
    {
        aiCtr.Enemy.Attack = false;
        aiCtr.Enemy.Special = false;
    }
}