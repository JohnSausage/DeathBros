using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///-----------------------------------------------------------------
/// ChrStateMachine
///-----------------------------------------------------------------
[System.Serializable]
public class ChrStateMachine
{
    public string currentState;

    public SCState CurrentState { get; protected set; }
    protected SCState previousState;

    public ChrStateMachine()
    {
        CurrentState = new SCState();
        previousState = new SCState();

        //StaticStates.Instance.Init();
    }

    public void Update(Character chr)
    {
        CurrentState.Execute(chr);

        currentState = CurrentState.ToString();
    }

    public void ChangeState(Character chr, SCState newState)
    {
        if (newState != null)
        {
            CurrentState.Exit(chr);
            previousState = CurrentState;
            CurrentState = newState;
            newState.Enter(chr);
        }
        else
        {
            Debug.Log("ChrStateMachine - New State not found!");
        }
    }

    public void GoToPreviousState(Character chr)
    {
        ChangeState(chr, previousState);
    }
}

///-----------------------------------------------------------------
/// SCState
///-----------------------------------------------------------------
public class SCState
{
    public virtual void Enter(Character chr)
    {
        if (chr == null) return;
        if (chr.Anim == null) return;

        chr.Timer = 0;
        chr.FrozenInputX = chr.DirectionalInput.x;
    }

    public virtual void Execute(Character chr)
    {
        if (chr == null) return;

        chr.Timer++;
    }

    public virtual void Exit(Character chr)
    {
        if (chr == null) return;
        if (chr.Anim == null) return;

        chr.Anim.animationSpeed = 1;
    }

    protected void ChangeState(Character chr, SCState newState)
    {
        if (chr != null)
            chr.ChrSM.ChangeState(chr, newState);
    }

    protected void TakeDamage(Damage damage, Character chr)
    {

    }
}

///-----------------------------------------------------------------
/// SCS_Idle
///-----------------------------------------------------------------
public class SCS_Idle : SCState
{
    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.jumpsUsed = 0;

        chr.Anim.animationSpeed = 1.5f;

        chr.Anim.ChangeAnimation(chr.StatesSO.idle_anim);
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        chr.GetInputs();

        if (chr.DirectionalInput.y < -0.5f)
            ChangeState(chr, StaticStates.crouch);

        if (Mathf.Abs(chr.DirectionalInput.x) != 0)
            ChangeState(chr, StaticStates.walking);

        if (chr.StrongInputs.x != 0)
            ChangeState(chr, StaticStates.dash);

        //if (chr.HoldShield)
        //    ChangeState(typeof(CS_Shield));

        if (chr.Jump)
            ChangeState(chr, StaticStates.jumpsquat);

        chr.SCS_CheckIfGrounded();

        chr.SCS_CheckForGroundAttacks();
    }
}

///-----------------------------------------------------------------
/// SCS_Walking
///-----------------------------------------------------------------
public class SCS_Walking : SCState
{
    public override void Enter(Character chr)
    {
        base.Enter(chr);

        if (chr.DirectionalInput.x < 0) chr.Spr.flipX = true;
        if (chr.DirectionalInput.x > 0) chr.Spr.flipX = false;
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        chr.Anim.animationSpeed = Mathf.Abs(chr.DirectionalInput.x);

        if (Mathf.Abs(chr.DirectionalInput.x) < 0.5f)
            chr.Anim.ChangeAnimation(chr.StatesSO.walking_anim);


        if (Mathf.Abs(chr.DirectionalInput.x) >= 0.7f)
            chr.Anim.ChangeAnimation(chr.StatesSO.running_anim);


        chr.GetInputs();

        if (Mathf.Abs(chr.DirectionalInput.x) == 0f || Mathf.Sign(chr.DirectionalInput.x) != chr.Direction)
        {
            ChangeState(chr, StaticStates.skid);

            //CState exit = (CS_Skid)chr.GetState(typeof(CS_Skid));
            //
            //if (exit != null)
            //{
            //    CS_Skid skid = (CS_Skid)exit;
            //    skid.direction = chr.Direction;
            //}
            //else
            //{
            //    exit = (CS_Idle)chr.GetState(typeof(CS_Idle));
            //}
            //ChangeState(exit);

        }

        //if (chr.HoldShield)
        //    ChangeState(typeof(CS_Shield));

       if (chr.Jump)
           ChangeState(chr, StaticStates.jumpsquat);
       
       
        if (chr.DirectionalInput.y < -0.5f)
            ChangeState(chr, StaticStates.crouch);

        chr.SCS_CheckForGroundAttacks();

        chr.SCS_CheckIfGrounded();

    }
}

///-----------------------------------------------------------------
/// SCS_Jumping
///-----------------------------------------------------------------
public class SCS_Jumping : SCState
{

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        if (chr.canChangeDirctionInAir)
            chr.Direction = Mathf.Sign(chr.DirectionalInput.x);

        if (chr.Ctr.velocity.y > 0)
            chr.Anim.ChangeAnimation(chr.StatesSO.jumpUp_anim);
        else
            chr.Anim.ChangeAnimation(chr.StatesSO.jumpDown_anim);


        chr.GetInputs();

        if (chr.Ctr.velocity.y < 0 && chr.StrongInputs.y == -1)
        {
            chr.Ctr.fastFall = true;
        }

        if (chr.DirectionalInput.y <= -0.5f)
        {
            chr.Ctr.fallThroughPlatform = true;
        }
        else
        {
            chr.Ctr.fallThroughPlatform = false;
        }

        chr.SCS_CheckIfLanding();

        if (chr.Jump)
        {

            if (chr.jumpsUsed < chr.GetCurrentStatValue("Jumps"))
            {
                ChangeState(chr, StaticStates.doubleJumpsquat);
            }
        }

        //f (chr.Ctr.onWall)
        //
        //   ChangeState(typeof(CS_Wallsliding));
        //
        //
        //f (chr.Shield)
        //
        //   ChangeState(typeof(CS_AirDodge));
        //

        chr.SCS_CheckForAerials();
    }

}

///-----------------------------------------------------------------
/// SCS_Crouch
///-----------------------------------------------------------------
public class SCS_Crouch : SCState
{
    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.Anim.ChangeAnimation(chr.PlayerStatesSO.crouch_anim);
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        chr.GetInputs();
        chr.SetInputs(new Vector2(0, chr.DirectionalInput.y));

        if (chr.StrongInputs.y < 0)
        {
            chr.Ctr.fallThroughPlatform = true;
            chr.ClearStrongInputs();
        }

        if (chr.DirectionalInput.y >= -0.25f)
        {
            ChangeState(chr, StaticStates.idle);
        }

        if (chr.Jump)
        {
            ChangeState(chr, StaticStates.jumpsquat);
        }

        chr.SCS_CheckIfGrounded();

        chr.SCS_CheckForGroundAttacks();

    }
}

///-----------------------------------------------------------------
/// SCS_Jumpsquat
///-----------------------------------------------------------------
public class SCS_Jumpsquat : SCState
{
    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.FrozenInputX = chr.DirectionalInput.x * 0.75f;

        chr.Anim.ChangeAnimation(chr.StatesSO.jumpsquat_anim);
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);


        chr.SetInputs(new Vector2(chr.FrozenInputX, 0));


        //if (chr.Attack && chr.StrongInputs.y > 0)
        //{
        //    ChangeState(EAttackType.USoul);
        //}

        if (chr.Timer >= chr.StatesSO.jumpsquat_duration)
        {
            if (chr.TiltInput != Vector2.zero)
            {
                chr.SCS_CheckForAerials();
                chr.Ctr.jumpVelocity = chr.GetCurrentStatValue("JumpStrength") * 0.75f;
            }
            else
            {
                ChangeState(chr, StaticStates.jumping);

                chr.GetInputs();

                if (chr.HoldJump || chr.DirectionalInput.y > 0.75f)
                {
                    chr.Ctr.jumpVelocity = chr.GetCurrentStatValue("JumpStrength");
                }
                else
                {
                    chr.Ctr.jumpVelocity = chr.GetCurrentStatValue("JumpStrength") * 0.75f;
                }
            }
        }
    }

    public override void Exit(Character chr)
    {
        base.Exit(chr);

        chr.jumpsUsed++;
    }
}

///-----------------------------------------------------------------
/// SCS_DoubleJumpsquat
///-----------------------------------------------------------------

public class SCS_DoubleJumpsquat : SCState
{
    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.Anim.ChangeAnimation(chr.PlayerStatesSO.doublejumpsquat_anim);
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);


        if (chr.Timer >= chr.PlayerStatesSO.doublejumpsquat_duration)
        {
            ChangeState(chr, StaticStates.jumping);
            chr.Ctr.jumpVelocity = chr.GetCurrentStatValue("JumpStrength");
        }
    }

    public override void Exit(Character chr)
    {
        base.Exit(chr);

        chr.jumpsUsed++;

        chr.Ctr.fastFall = false;
    }
}

///-----------------------------------------------------------------
/// SCS_Landing
///-----------------------------------------------------------------
public class SCS_Landing : SCState
{
    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.Anim.ChangeAnimation(chr.StatesSO.landing_anim);
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        chr.GetInputs();

        if (chr.Timer >= (chr.StatesSO.landing_duration + chr.LandingLag))
        {
            chr.LandingLag = 0; //clear additional landing lag when landing

            if (chr.DirectionalInput.y < 0)
            {
                ChangeState(chr, StaticStates.crouch);
            }
            else
            {
                ChangeState(chr, StaticStates.idle);
            }
        }

        chr.FrozenInputX *= 0.8f;
        chr.SetInputs(new Vector2(chr.FrozenInputX, 0));
    }
}

///-----------------------------------------------------------------
/// SCS_Dash
///-----------------------------------------------------------------
public class SCS_Dash : SCState
{
    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.Direction = Mathf.Sign(chr.FrozenInputX);
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        chr.GetInputs();

        if (Mathf.Sign(chr.DirectionalInput.x) != chr.Direction && Mathf.Abs(chr.DirectionalInput.x) > 0.5f)
        {
            ChangeState(chr, StaticStates.idle);
        }


        if (chr.Timer >= chr.PlayerStatesSO.dash_duration)
        {
            if (chr.DirectionalInput.x == 0)
            {
                ChangeState(chr, StaticStates.idle);
            }
            else
            {
                ChangeState(chr, StaticStates.walking);
            }
        }

        if (chr.Timer <= 2)
            chr.SetInputs(new Vector2(chr.FrozenInputX * 0.05f, 0));
        else
            chr.SetInputs(new Vector2(chr.FrozenInputX * 1.2f, 0));

        if (chr.Jump)
        {
            ChangeState(chr, StaticStates.jumping);
        }

        if (chr.Timer <= 5)
        {
            if (chr.Attack)
            {
                //ChangeState(EAttackType.FSoul);
            }
        }

        chr.SCS_CheckForGroundAttacks();
    }
}

///-----------------------------------------------------------------
/// SCS_Skid
///-----------------------------------------------------------------
public class SCS_Skid : SCState
{
    public override void Enter(Character chr)
    {
        //base.Enter(chr);

        chr.Anim.animationSpeed = 0.5f;
        chr.ChangedDirection = false;

        chr.IdleTimer = 0;
        chr.Timer = 0;
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        chr.GetInputs();


        if (!chr.ChangedDirection)
        {
            if (Mathf.Sign(chr.DirectionalInput.x) != Mathf.Sign(chr.FrozenInputX) && chr.DirectionalInput.x != 0) //don't change direction if there is no input
            {
                chr.Spr.flipX = !chr.Spr.flipX;
                chr.ChangedDirection = true;
            }
        }


        if (chr.DirectionalInput.x == 0)
        {
            chr.IdleTimer++;
        }

        chr.SetInputs(new Vector2(chr.FrozenInputX * 0.9f, 0));

        if (chr.Timer >= chr.PlayerStatesSO.skid_duration)
        {
            ChangeState(chr, StaticStates.idle);

            chr.GetInputs();
        }
        else if (chr.IdleTimer >= chr.PlayerStatesSO.skid_idleOutDuration)
        {
            ChangeState(chr, StaticStates.idle);

            chr.GetInputs();
        }
        else if (chr.Jump)
        {
            ChangeState(chr, StaticStates.jumpsquat);
        }
    }
}