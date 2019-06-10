using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StaticAttackStateSO : ScriptableObject
{
    public string animationName;


    public SCS_Attack attackState;

    public virtual void Init()
    {
        attackState.animationName = animationName;
    }

    public virtual SCS_Attack CreateAttackState()
    {
        SCS_Attack attack = new SCS_Attack();
        attack.animationName = animationName;
        attack.attackBuff = new AttackBuff();
        return attack;
    }
}

public class SCS_Attack : SCState
{
    public string animationName { get; set; }
    
    public AttackBuff attackBuff { get; set; }

    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.Anim.ChangeAnimation(animationName);
        chr.CurrentAttackBuff = attackBuff;
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);
    }

    public override void Exit(Character chr)
    {
        base.Exit(chr);

        chr.CurrentAttackBuff = null;
    }
}