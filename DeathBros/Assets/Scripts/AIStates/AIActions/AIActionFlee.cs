using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/Actions/Flee")]
public class AIActionFlee : AIActionSO
{
    public Vector2 targetOffset = Vector2.zero;

    public override void Execute(AIController aiCtr)
    {
        base.Execute(aiCtr);

        aiCtr.Enemy.SetInputs(-aiCtr.TargetDirection(targetOffset));
    }
}
