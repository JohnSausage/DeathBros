using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/Actions/Chase")]
public class AIActionChase : AIActionSO
{
    public Vector2 targetOffset = Vector2.zero;

    [Space]

    public bool jumpOnWall;



    public override void Execute(AIController aiCtr)
    {
        base.Execute(aiCtr);

        aiCtr.Enemy.SetInputs(new Vector2(Mathf.Sign(aiCtr.TargetDirection(targetOffset).x) , 0));

        if(aiCtr.Enemy.Ctr.OnWall == true)
        {
            aiCtr.Enemy.Jump = true;
            aiCtr.Enemy.HoldJump = true;
        }
        else
        {
            aiCtr.Enemy.Jump = false;
            aiCtr.Enemy.HoldJump = false;
        }
    }
}
