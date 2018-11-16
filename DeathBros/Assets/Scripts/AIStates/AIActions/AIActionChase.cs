using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/Actions/Chase")]
public class AIActionChase : AIActionSO
{
    public Vector2 targetOffset = Vector2.zero;

    public override void Execute(AIController aiCtr)
    {
        aiCtr.Enemy.SetInputs(aiCtr.TargetDirection(targetOffset));
    }
}
