using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StaticAttackStates/Special")]
public class StaticAttackStateSpecial : StaticAttackStateSO
{
    public int aerialLimit = 0;
    public int comboPowerCost = 5;

    public override SCS_Attack CreateAttackState()
    {
        SCS_SpecialAttack specialAttack = new SCS_SpecialAttack();
        specialAttack.animationName = animationName;
        specialAttack.aerialLimit = aerialLimit;
        specialAttack.attackBuff = new AttackBuff();
        
        return specialAttack;
    }

    public virtual SCS_SpecialAttack CreateAttackState(ESpecial type)
    {
        SCS_SpecialAttack specialAttack = new SCS_SpecialAttack();
        specialAttack.animationName = animationName;
        specialAttack.aerialLimit = aerialLimit;
        specialAttack.attackBuff = new AttackBuff();
        specialAttack.type = type;
        specialAttack.comboPowerCost = comboPowerCost;
        return specialAttack;
    }
}

public enum ESpecial { NONE, NEUTRAL, SIDE, UP, DOWN };

public class SCS_SpecialAttack : SCS_Attack
{
    public ESpecial type;
    public int comboPowerCost = 0;
    public int aerialLimit = 0;

    protected int aerialCount = 0;
    protected bool waveBounced;

    public override void Enter(Character chr)
    {
        base.Enter(chr);

        waveBounced = false;


        if (chr.DirectionalInput.x != 0)
        {
            if (Mathf.Sign(chr.DirectionalInput.x) != chr.Direction)
            {
                chr.Direction = Mathf.Sign(chr.DirectionalInput.x);
            }
        }

        chr.SCS_CountSpecial(type); // counts how often the special is used in the air, is reset on landing
    }

    public override void Execute(Character chr)

    {
        base.Execute(chr);

        CheckForWaveBounce(chr);


        if (chr.Ctr.IsGrounded)
        {
            chr.FrozenInputX *= 0.9f;
        }
        else
        {
            if (chr.Timer <= 3)
            {
                chr.GetInputs();
                chr.FrozenInputX = chr.DirectionalInput.x;
            }
            else
            {
                chr.GetInputs();
                chr.SetInputs(chr.DirectionalInput * 0.5f);
            }
        }


        if (chr.Anim.animationOver)
        {
            if (chr.Ctr.IsGrounded)
            {
                chr.jumpsUsed = 0;
            }
            chr.SCS_Idle();
        }
    }

    protected void CheckForWaveBounce(Character chr)
    {
        if (chr.Timer >= 3)
        {
            return;
        }

        if (waveBounced == false)
        {
            if (chr.StrongInputs.x != 0 && chr.Direction != Mathf.Sign(chr.StrongInputs.x))
            {
                chr.Direction = -chr.Direction;
                chr.Ctr.velocity.x *= -1;
                waveBounced = true;
            }
        }
    }
}