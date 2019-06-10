using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StaticAttackStates/Aerial")]
public class StaticAttackStateAerial : StaticAttackStateSO
{
    public int landingLag = 0;

    public override SCS_Attack CreateAttackState()
    {
        SCS_AerialAttack aerialAttack = new SCS_AerialAttack();
        aerialAttack.animationName = animationName;
        aerialAttack.landingLag = landingLag;
        aerialAttack.attackBuff = new AttackBuff();
        return aerialAttack;
    }
}

public class SCS_AerialAttack : SCS_Attack
{
    public int landingLag { get; set; }

    public override void Enter(Character chr)
    {
        base.Enter(chr);
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        chr.GetInputs();

        if (chr.Ctr.velocity.y < 0 && chr.StrongInputs.y < -0.5f)
        {
            chr.Ctr.fastFall = true;
        }

        if (chr.Anim.animationOver)
        {
            chr.SCS_ChangeState(StaticStates.jumping);
        }

        if (chr.Ctr.IsGrounded)
        {
            chr.LandingLag += landingLag;
            chr.SCS_ChangeState(StaticStates.landing);
        }
    }
}