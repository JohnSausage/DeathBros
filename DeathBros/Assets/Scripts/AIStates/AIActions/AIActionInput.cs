using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/Actions/Input")]
public class AIActionInput : AIActionSO
{
    public Vector2 directionalInput = Vector2.zero;

    public bool directionalInputXToTarget = false;
    public bool directionalInputYToTarget = false;

    public bool attack = false;
    public bool special = false;
    public bool jump = false;
    public bool holdJump = false;
    public bool shield = false;
    public bool holdShield = false;

    protected Vector2 input;
    public override void Enter(AIController aiCtr)
    {
        base.Enter(aiCtr);
    }

    public override void Execute(AIController aiCtr)
    {
        if (directionalInput != Vector2.zero)
        {
            input = directionalInput;
        }

        if (directionalInputXToTarget == true)
        {
            input.x *= aiCtr.TargetDirection().x;
        }

        if (directionalInputYToTarget == true)
        {
            input.y *= aiCtr.TargetDirection().y;
        }

        aiCtr.Enemy.SetInputs(input);


        aiCtr.Enemy.Attack = attack;
        aiCtr.Enemy.Special = special;
        aiCtr.Enemy.Jump = jump;
        aiCtr.Enemy.Shield= shield;
        aiCtr.Enemy.HoldJump = holdJump;
        aiCtr.Enemy.HoldShield = holdShield;
    }

    public override void Exit(AIController aiCtr)
    {
        aiCtr.Enemy.Special = false;
        aiCtr.Enemy.HoldJump = false;
        aiCtr.Enemy.Shield = false;
        aiCtr.Enemy.HoldShield = false;
    }
}