using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/Actions/Input")]
public class AIActionInput : AIActionSO
{
    public Vector2 directionalInput = Vector2.zero;

    [Space]

    public bool directionalInputXToDirection = false;
    public bool changeDirectionOnWall = false;
    public bool changeDirectionOnLedge = false;

    [Space]

    public bool directionalInputXToTarget = false;
    public bool directionalInputYToTarget = false;

    [Space]

    public bool attack = false;
    public bool special = false;
    public bool jump = false;
    public bool holdJump = false;
    public bool shield = false;
    public bool holdShield = false;

    [Space]

    public int stopInputsAfterFrame = 0;




    protected Vector2 input;
    public override void Enter(AIController aiCtr)
    {
        base.Enter(aiCtr);
    }

    public override void Execute(AIController aiCtr)
    {
        base.Execute(aiCtr);

        if (directionalInput != Vector2.zero)
        {
            input = directionalInput;
        }

        if(directionalInputXToDirection == true)
        {
            if(changeDirectionOnWall == true)
            {
                if(aiCtr.Enemy.Ctr.OnWall == true)
                {
                    aiCtr.Enemy.Direction = -aiCtr.Enemy.Direction;
                }
            }

            if (changeDirectionOnLedge == true)
            {
                if (aiCtr.Enemy.Ctr.OnLedge == true)
                {
                    aiCtr.Enemy.Direction = -aiCtr.Enemy.Direction;
                }
            }

            input.x *= aiCtr.Enemy.Direction;
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

        if(stopInputsAfterFrame != 0)
        {
            if(aiCtr.Timer > stopInputsAfterFrame || aiCtr.Timer == 0)
            {

                aiCtr.Enemy.Attack = false;
                aiCtr.Enemy.Special = false;
                aiCtr.Enemy.Jump = false;
                aiCtr.Enemy.Shield = false;
                aiCtr.Enemy.HoldJump = false;
                aiCtr.Enemy.HoldShield = false;

                return;
            }
        }

        aiCtr.Enemy.Attack = attack;
        aiCtr.Enemy.Special = special;
        aiCtr.Enemy.Jump = jump;
        aiCtr.Enemy.Shield= shield;
        aiCtr.Enemy.HoldJump = holdJump;
        aiCtr.Enemy.HoldShield = holdShield;
    }

    public override void Exit(AIController aiCtr)
    {
        base.Exit(aiCtr);

        aiCtr.Enemy.Special = false;
        aiCtr.Enemy.HoldJump = false;
        aiCtr.Enemy.Shield = false;
        aiCtr.Enemy.HoldShield = false;
    }
}