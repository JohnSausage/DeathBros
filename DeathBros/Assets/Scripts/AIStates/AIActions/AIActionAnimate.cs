using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/Actions/Animate")]
public class AIActionAnimate : AIActionSO
{
    public string animationName;

    public bool lookAtTarget = false;

    public override void Enter(AIController aiCtr)
    {
        base.Enter(aiCtr);

        if(lookAtTarget == true)
        {
            aiCtr.Enemy.Direction = Mathf.Sign(aiCtr.TargetDirection().x);
        }
        aiCtr.Enemy.queuedAnimation = animationName;
    }
}