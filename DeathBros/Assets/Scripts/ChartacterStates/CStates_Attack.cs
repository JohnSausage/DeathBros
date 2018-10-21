using System;
using UnityEngine;

[System.Serializable]
public class CStates_Attack
{
    public CS_TiltAttack testAttack;
    public CS_SoulAttack uSoul;

    [Space]

    public CS_TiltAttack uTilt;
    public CS_TiltAttack dTilt;
    public CS_TiltAttack fTilt;

    [Space]

    public CS_Aerial nAir;
    public CS_Aerial uAir;
    public CS_Aerial dAir;
    public CS_Aerial fAir;
    public CS_Aerial bAir;

    public virtual void Init(Character chr)
    {
        testAttack.Init(chr);
        uSoul.Init(chr);

        uTilt.Init(chr);
        dTilt.Init(chr);
        fTilt.Init(chr);

        nAir.Init(chr);
        uAir.Init(chr);
        dAir.Init(chr);
        fAir.Init(chr);
        bAir.Init(chr);
    }
}

public enum EAttackType { Jab, FTilt, DTilt, UTilt, DashAtk, NAir, FAir, DAir, UAir, BAir, FSoul, DSoul, USoul }

[System.Serializable]
public class CS_Attack : CState
{
    public EAttackType attackType;

    public event Action<CS_Attack> AttackOver;

    public override void Exit()
    {
        base.Exit();

        if (AttackOver != null) AttackOver(this);
    }
}

[System.Serializable]
public class CS_TiltAttack : CS_Attack
{
    public override void Execute()
    {
        base.Execute();

        chr.SetInputs(Vector2.zero);

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


    [SerializeField]
    private float chargeRate = 1f;

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

        if (chr.HoldAttack)
        {
            chr.ModSoulMeter(-chargeRate);

            if (chr is Player)
            {
                Player player = (Player)chr;

                player.soulCharge += chargeRate;
            }
        }

        if (!chr.HoldAttack || chr.soulMeter <= 1f)
        {
            chr.Anim.ChangeAnimation(animation);
            charging = false;
            chr.Spr.color = Color.white;
        }

        if (!charging && chr.Anim.animationOver)
        {
            chr.CS_SetIdle();
        }
    }

    public override void Exit()
    {
        base.Exit();

        chr.Spr.color = Color.white;

        if (chr is Player)
        {
            Player player = (Player)chr;

            player.soulCharge = 0f;
        }
    }
}

[System.Serializable]
public class CS_Aerial : CS_Attack
{
    public override void Execute()
    {
        base.Execute();

        chr.GetInputs();

        if (chr.Ctr.velocity.y < 0 && chr.DirectionalInput.y < -0.5f)
        {
            chr.Ctr.fastFall = true;
        }

        if (chr.Anim.animationOver)
        {
            ChangeState(typeof(CS_Jumping));
        }

        if (chr.Ctr.IsGrounded)
        {
            ChangeState(typeof(CS_Landing));
        }
    }
}