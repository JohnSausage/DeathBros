using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCState : IState
{
    //private static StaticCState instance;
    //public static StaticCState Instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //        {
    //            instance = new StaticCState();
    //        }
    //        return instance;
    //    }
    //}

    protected Character chr;
    protected FrameAnimation animation;

    public void Enter()
    {
        Enter(chr);
    }

    public void Execute()
    {
        Execute(chr);
    }

    public void Exit()
    {
        Exit(chr);
    }

    public virtual void Enter(Character chr)
    {
        this.chr = chr;

    }

    public virtual void Execute(Character chr)
    {
        this.chr = chr;
    }

    public virtual void Exit(Character chr)
    {
        this.chr = chr;
    }

    protected void ChangeState(StaticCState newState)
    {
        if (newState == null) return;

        chr.CSMachine.ChangeState(newState);
    }
}

public class SCS_Idle : StaticCState
{
    private static SCS_Idle instance;
    public static SCS_Idle InstanceP
    {
        get
        {
            if (instance == null)
            {
                instance = new SCS_Idle();
            }
            return instance;
        }
    }

    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.Anim.ChangeAnimation(chr.Anim.GetAnimation(chr.CStateParamtetersSO.idleP.animationName));

        chr.jumpsUsed = 0;

        chr.Anim.animationSpeed = 1;
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        chr.GetInputs();

        if (chr.DirectionalInput.y < -0.5f)
            ChangeState(SCS_Crouch.Instance);

        if (Mathf.Abs(chr.DirectionalInput.x) != 0)
            ChangeState(SCS_Walk.Instance);

        if (chr.StrongInputs.x != 0)
            ChangeState(SCS_Dash.Instance);

        if (chr.HoldShield)
            ChangeState(SCS_Shield.Instance);

        if (chr.Jump)
            ChangeState(SCS_JumpSquat.Instance);

        chr.CS_CheckIfStillGrounded();

        if (chr.CheckForSpecialAttacks() == false)
        {
            if (chr.CheckForSoulAttacks() == false)
            {
                chr.CheckForTiltAttacks();
            }
        }
    }
}

public class SCS_Walk : StaticCState
{
    private static SCS_Walk instance;
    public static SCS_Walk Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SCS_Walk();
            }
            return instance;
        }
    }

    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.csDirection = Mathf.Sign(chr.DirectionalInput.x);

        if (chr.DirectionalInput.x < 0) chr.Spr.flipX = true;
        if (chr.DirectionalInput.x > 0) chr.Spr.flipX = false;
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        chr.Anim.animationSpeed = Mathf.Abs(chr.DirectionalInput.x);

        //if (Mathf.Abs(chr.DirectionalInput.x) < 0.5f && fAnimSlow != null)
        //    chr.Anim.ChangeAnimation(fAnimSlow);


        if (Mathf.Abs(chr.DirectionalInput.x) >= 0.7f)
            chr.Anim.ChangeAnimation(animation);


        chr.GetInputs();

        if (Mathf.Abs(chr.DirectionalInput.x) == 0f || Mathf.Sign(chr.DirectionalInput.x) != chr.csDirection)
        {
            //CState exit = (CS_Skid)chr.GetState(typeof(CS_Skid));
            //
            //if (exit != null)
            //{
            //    CS_Skid skid = (CS_Skid)exit;
            //    skid.direction = chr.csDirection;
            //}
            //else
            //{
            //    exit = (CS_Idle)chr.GetState(typeof(CS_Idle));
            //}
            //ChangeState(exit);

            ChangeState(SCS_Idle.InstanceP);

        }

        if (chr.HoldShield)
            ChangeState(SCS_Shield.Instance);

        if (chr.Jump)
            ChangeState(SCS_JumpSquat.Instance);


        if (chr.DirectionalInput.y < -0.5f)
            ChangeState(SCS_Crouch.Instance);


        if (chr.CheckForSpecialAttacks() == false)
        {
            if (chr.CheckForSoulAttacks() == false)
            {
                chr.CheckForTiltAttacks();
            }
        }

        chr.CS_CheckIfStillGrounded();
    }
}

public class SCS_Dash : StaticCState
{
    private static SCS_Walk instance;
    public static SCS_Walk Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SCS_Walk();
            }
            return instance;
        }
    }
}

public class SCS_JumpSquat : StaticCState
{
    private static SCS_JumpSquat instance;
    public static SCS_JumpSquat Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SCS_JumpSquat();
            }
            return instance;
        }
    }
}

public class SCS_Crouch : StaticCState
{
    private static SCS_Crouch instance;
    public static SCS_Crouch Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SCS_Crouch();
            }
            return instance;
        }
    }
}

public class SCS_Shield : StaticCState
{
    private static SCS_Shield instance;
    public static SCS_Shield Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SCS_Shield();
            }
            return instance;
        }
    }
}
