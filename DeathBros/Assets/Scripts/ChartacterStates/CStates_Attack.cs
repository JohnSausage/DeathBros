using System;
using UnityEngine;

[System.Serializable]
public class CStates_Attack
{
    //public CS_TiltAttack testAttack;
    public CS_SoulAttack uSoul;
    public CS_SoulAttack dSoul;
    public CS_SoulAttack fSoul;

    [Space]

    public CS_JabAttack jab1;
    public CS_JabAttack jab2;

    public CS_TiltAttack uTilt;
    public CS_TiltAttack dTilt;
    public CS_TiltAttack fTilt;

    [Space]

    public CS_Aerial nAir;
    public CS_Aerial uAir;
    public CS_Aerial dAir;
    public CS_Aerial fAir;
    public CS_Aerial bAir;

    [Space]

    public CS_SpecialAttack nSpec;
    public CS_SpecialAttack uSpec;
    public CS_SpecialAttack dSpec;
    public CS_SpecialAttack fSpec;

    public virtual void Init(Character chr)
    {
        uSoul.Init(chr);
        dSoul.Init(chr);
        fSoul.Init(chr);

        jab1.Init(chr);
        jab2.Init(chr);

        uTilt.Init(chr);
        dTilt.Init(chr);
        fTilt.Init(chr);

        nAir.Init(chr);
        uAir.Init(chr);
        dAir.Init(chr);
        fAir.Init(chr);
        bAir.Init(chr);

        nSpec.Init(chr);
        uSpec.Init(chr);
        dSpec.Init(chr);
        fSpec.Init(chr);
    }
}

public enum EAttackType { Jab1, FTilt, DTilt, UTilt, DashAtk, NAir, FAir, DAir, UAir, BAir, FSoul, DSoul, USoul, Jab2, NSpec, DSpec, USpec, FSpec, None }

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

    public override void Enter()
    {
        base.Enter();

        if (chr is Enemy) chr.Flash(EffectManager.ColorAttack, 2);
    }
}

[System.Serializable]
public class CS_JabAttack : CS_TiltAttack
{
    [SerializeField]
    private int jabNr;

    [SerializeField]
    private int cancelTime = 15;
    private int timer;

    public override void Enter()
    {
        base.Enter();

        timer = 0;
    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        if (jabNr == 1 && chr.Attack && timer > cancelTime)
        {
            ChangeState(chr.GetAttackState(EAttackType.Jab2));
        }
    }
}

[System.Serializable]
public class CS_TiltAttack : CS_Attack
{
    private float dirX;

    public override void Enter()
    {
        base.Enter();

        dirX = 0;
        if (chr.DirectionalInput.x != 0)
            dirX = chr.DirectionalInput.x;
    }

    public override void Execute()
    {
        base.Execute();

        dirX *= 0.8f;
        chr.SetInputs(new Vector2(dirX, 0));

        if (chr.Ctr.onLedge)
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
    private int minChargeTime = 10;
    private int timer = 0;

    protected float dirX;

    public override void Init(Character chr)
    {
        base.Init(chr);

        chargeAnimation = chr.Anim.GetAnimation(chargeAnimationName);

        chr.Anim.AnimationOver += SetIdleAfterAttack;
    }

    public override void Enter()
    {
        base.Enter();

        chr.Anim.ChangeAnimation(chargeAnimation);
        charging = true;
        chr.Spr.color = Color.red;
        timer = 0;

        dirX = 0;
        if (chr.DirectionalInput.x != 0)
            dirX = chr.DirectionalInput.x;
    }

    public override void Execute()
    {
        base.Execute();

        if (charging && timer % 10 == 0) chr.Flash(EffectManager.ColorAttack, 2);

        timer++;

        dirX *= 0.8f;
        chr.SetInputs(new Vector2(dirX, 0));

        if (chr.Ctr.onLedge)
            chr.SetInputs(Vector2.zero);

        chr.CS_CheckIfStillGrounded();

        if (chr.HoldAttack)
        {
            chr.ModSoulMeter(-chargeRate);

            if (chr is Player)
            {
                Player player = (Player)chr;

                player.soulCharge += chargeRate;
            }
        }
        else
        {
            if (timer > minChargeTime)
            {
                chr.Anim.ChangeAnimation(animation);
                charging = false;
                chr.Spr.color = Color.white;
            }
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

    private void SetIdleAfterAttack(FrameAnimation animation)
    {
        if (this.animation == animation)
        {
            chr.CS_SetIdle();
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

[System.Serializable]
public class CS_SpecialAttack : CS_Attack
{
    [SerializeField]
    protected bool consumeSoulOverTime = false;

    [SerializeField]
    protected float soulCost = 0;

    [SerializeField]
    protected StatChange changeStatOnExit;

    [SerializeField]
    protected float addSoulsOnExit = 0;

    protected bool soulCostMet = false;

    public override void Enter()
    {
        base.Enter();



        soulCostMet = false;

        if (soulCost > chr.soulMeter)
        {
            if (chr.Ctr.IsGrounded)
                ChangeState(typeof(CS_Idle));
            else
                ChangeState(typeof(CS_Jumping));
        }
        else
        {
            soulCostMet = true;

            if (!consumeSoulOverTime)
            {
                chr.ModSoulMeter(-soulCost);
            }

        }
    }

    public override void Execute()
    {
        base.Execute();

        if (consumeSoulOverTime)
        {
            chr.ModSoulMeter(-soulCost / chr.Anim.currentAnimation.Length());
        }

        if (chr.Anim.animationOver)
        {
            if (chr.Ctr.IsGrounded)
                ChangeState(typeof(CS_Idle));
            else
                ChangeState(typeof(CS_Jumping));
        }
    }

    public override void Exit()
    {
        base.Exit();

        if (soulCostMet)
        {
            if (changeStatOnExit.statName != "")
                changeStatOnExit.ExecuteStatChange(chr);

            chr.ModSouls(addSoulsOnExit);
        }
    }
}