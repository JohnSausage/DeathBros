using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StaticAttackStates/Tilt")]
public class StaticAttackStateTilt : StaticAttackStateSO
{
    public override void Init()
    {
        attackState = new SCS_TiltAttack();

        base.Init();
    }

    public override SCS_Attack CreateAttackState()
    {
        SCS_TiltAttack tiltAttack = new SCS_TiltAttack();
        tiltAttack.animationName = animationName;
        tiltAttack.attackBuff = new AttackBuff();
        return tiltAttack;
    }
}

public class SCS_TiltAttack : SCS_Attack
{

    public override void Enter(Character chr)
    {
        base.Enter(chr);
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        chr.FrozenInputX *= 0.8f;

        chr.SetInputs(new Vector2(chr.FrozenInputX, 0));

        if (chr.Ctr.OnLedge)
            chr.SetInputs(Vector2.zero);

        if (chr.Anim.animationOver)
        {
            chr.SCS_Idle();
        }
    }
}