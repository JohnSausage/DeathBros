using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CStates_Attack
{
    public CS_TiltAttack testAttack;
    public CS_SoulAttack uSoul;

    public virtual void Init(Character chr)
    {
        testAttack.Init(chr);
        uSoul.Init(chr);
    }
}

public enum EAttackType { Jab, FTilt, DTilt, UTilt, DashAtk, NAir, FAir, DAir, UAir, BAir, FSoul, DSoul, USoul}

[System.Serializable]
public class CS_Attack : CState
{
    public EAttackType attackType;
}

[System.Serializable]
public class CS_TiltAttack : CS_Attack
{
    public override void Execute()
    {
        base.Execute();

        if (chr.Anim.animationOver)
        {
            chr.CS_SetIdle();
        }
    }
}

[System.Serializable]
public class CS_SoulAttack : CS_Attack
{
    [SerializeField]
    private string chargeAnimationName = "idle";

    private FrameAnimation chargeAnimation;
    private bool charging = true;

    public override void Init(Character chr)
    {
        base.Init(chr);

        chargeAnimation = chr.Anim.GetAnimation(chargeAnimationName);
    }

    public override void Enter()
    {
        chr.Anim.ChangeAnimation(chargeAnimation);
        charging = true;
        chr.Spr.color = Color.yellow;
    }

    public override void Execute()
    {
        base.Execute();

        if(!chr.HoldAttack)
        {
            chr.Anim.ChangeAnimation(animation);
            charging = false;
            chr.Spr.color = Color.white;
        }

        if(!charging && chr.Anim.animationOver)
        {
            chr.CS_SetIdle();
        }
    }

    public override void Exit()
    {
        base.Exit();
        chr.Spr.color = Color.white;
    }
}