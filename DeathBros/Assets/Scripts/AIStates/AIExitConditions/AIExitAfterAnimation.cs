using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/ExitConditions/AnimationOver")]
public class AIExitAfterAnimation : AIExitConditionSO
{
    public string animationName = "";

    public override bool CheckForExit(AIController aiCtr)
    {
        if (animationName == "")
        {

            if (aiCtr.Enemy.Anim.animationOver)
            {
                aiCtr.ChangeState(exitState);
                return true;
            }
        }
        else
        {
            if (aiCtr.Enemy.Anim.currentAnimation.name == animationName)
            {
                aiCtr.ChangeState(exitState);
                return true;
            }
        }

        return false;
    }
}
