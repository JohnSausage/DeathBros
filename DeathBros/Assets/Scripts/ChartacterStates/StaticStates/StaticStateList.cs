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

        chr.ACharacterTakesDasmage += TakeDamage;
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

        chr.ACharacterTakesDasmage -= TakeDamage;
    }

    /*
    protected void ChangeState(Character chr, SCState newState)
    {
        if (chr == null) return;

        if (newState.stateType == EStateType.Complex && chr.StatesSO.stateTypes != EStateType.Complex)
        {
            return;
        }


        chr.ChrSM.ChangeState(chr, newState);
    }
    */


    protected void TakeDamage(Character chr, Damage damage)
    {
        //if (damage.damageType == EDamageType.Grab)
        //{
        //    CS_GetGrabbed grabbed = (CS_GetGrabbed)chr.GetState(typeof(CS_GetGrabbed));
        //    grabbed.damage = damage;
        //    ChangeState(typeof(CS_GetGrabbed));
        //}
        //else
        if (chr.shielding)
        {
            chr.SCS_ChangeState(StaticStates.shieldHit);
        }
        else
        {
            chr.SCS_ChangeState(StaticStates.hitfreeze);
        }
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

        chr.SCS_CheckForIdleOptions();

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

        chr.SCS_CheckForWalkingOptions();

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
                chr.SCS_ChangeState(StaticStates.doubleJumpsquat);
            }
        }

        if (chr.Ctr.onWall)

            chr.SCS_ChangeState(StaticStates.wallsliding);
        
         
        if (chr.Shield)

            chr.SCS_ChangeState(StaticStates.airDodge);


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

        chr.Anim.ChangeAnimation(chr.StatesSO.crouch_anim);
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
            chr.SCS_ChangeState(StaticStates.idle);
        }

        if (chr.Jump)
        {
            chr.SCS_ChangeState(StaticStates.jumpsquat);
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
                chr.SCS_ChangeState(StaticStates.jumping);

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

        chr.Anim.ChangeAnimation(chr.StatesSO.doublejumpsquat_anim);
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);


        if (chr.Timer >= chr.StatesSO.doublejumpsquat_duration)
        {
            chr.SCS_ChangeState(StaticStates.jumping);
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
                chr.SCS_ChangeState(StaticStates.crouch);
            }
            else
            {
                chr.SCS_ChangeState(StaticStates.idle);
            }
        }

        chr.FrozenInputX *= 0.8f;
        chr.SetInputs(new Vector2(chr.FrozenInputX, 0));
    }

    public override void Exit(Character chr)
    {
        base.Exit(chr);

        chr.LandingLag = 0;
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

        chr.Anim.ChangeAnimation(chr.StatesSO.dash_anim);
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        chr.GetInputs();

        if (Mathf.Sign(chr.DirectionalInput.x) != chr.Direction && Mathf.Abs(chr.DirectionalInput.x) > 0.5f)
        {
            chr.SCS_ChangeState(StaticStates.idle);
        }


        if (chr.Timer >= chr.StatesSO.dash_duration)
        {
            if (chr.DirectionalInput.x == 0)
            {
                chr.SCS_ChangeState(StaticStates.idle);
            }
            else
            {
                chr.SCS_ChangeState(StaticStates.walking);
            }
        }

        if (chr.Timer <= 2)
            chr.SetInputs(new Vector2(chr.FrozenInputX * 0.05f, 0));
        else
            chr.SetInputs(new Vector2(chr.FrozenInputX * 1.2f, 0));

        if (chr.Jump)
        {
            chr.SCS_ChangeState(StaticStates.jumping);
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

        chr.Anim.ChangeAnimation(chr.StatesSO.skid_anim);
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

        if (chr.Timer >= chr.StatesSO.skid_duration)
        {
            chr.SCS_ChangeState(StaticStates.idle);

            chr.GetInputs();
        }
        else if (chr.IdleTimer >= chr.StatesSO.skid_idleOutDuration)
        {
            chr.SCS_ChangeState(StaticStates.idle);

            chr.GetInputs();
        }
        else if (chr.Jump)
        {
            chr.SCS_ChangeState(StaticStates.jumpsquat);
        }
    }
}

///-----------------------------------------------------------------
/// SCS_Wallsliding
///-----------------------------------------------------------------
public class SCS_Wallsliding : SCState
{
    public override void Execute(Character chr)
    {
        base.Execute(chr);

        if (chr.Ctr.velocity.y > 0)
        {
            chr.Anim.ChangeAnimation(chr.StatesSO.wallslidingUp_anim);
        }
        else
        {
            chr.Anim.ChangeAnimation(chr.StatesSO.wallslidingDown_anim);
        }

        chr.Spr.flipX = chr.Ctr.wallDirection == -1;

        chr.GetInputs();

        chr.SCS_CheckIfLanding();

        if (!chr.Ctr.onWall)
        {
            chr.SCS_ChangeState(StaticStates.jumping);
        }

        chr.SCS_CheckForAerials();

        if (chr.Jump)
        {
            //walljumpStart.walljumpDirection = -chr.Ctr.wallDirection;

            chr.SCS_ChangeState(StaticStates.walljumpStart);
        }
    }

    public override void Exit(Character chr)
    {
        base.Exit(chr);

        chr.Direction = Mathf.Sign(chr.FrozenInputX);
    }
}

///-----------------------------------------------------------------
/// SCS_WalljumpStart
///-----------------------------------------------------------------
public class SCS_WalljumpStart : SCState
{
    protected float jumpHeightReductionFactor = 0.75f;

    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.Spr.flipX = chr.Ctr.wallDirection == 1;
        chr.SetInputs(Vector2.zero);

        chr.Anim.ChangeAnimation(chr.StatesSO.walljumpstart_anim);
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        if (chr.Timer >= chr.StatesSO.walljumpstart_duration)
        {
            chr.SetInputs(new Vector2(-Mathf.Sign(chr.FrozenInputX), 0));
            chr.Ctr.jumpVelocity = chr.GetCurrentStatValue("JumpStrength") * jumpHeightReductionFactor;

            chr.SCS_ChangeState(StaticStates.walljumping);
        }
    }
}

///-----------------------------------------------------------------
/// SCS_Walljumping
///-----------------------------------------------------------------
public class SCS_Walljumping : SCState
{
    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.Anim.ChangeAnimation(chr.StatesSO.walljumping_anim);
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        chr.GetInputs();

        chr.CheckForAerialAttacks();

        chr.SetInputs(new Vector2(Mathf.Sign(chr.FrozenInputX), 0));

        chr.SCS_CheckIfLanding();

        if (chr.Ctr.onWall)
        {
            chr.SCS_ChangeState(StaticStates.wallsliding);
        }


        if (chr.Timer >= chr.StatesSO.walljumping_duration)
        {
            chr.SCS_ChangeState(StaticStates.jumping);
        }
    }
}

///-----------------------------------------------------------------
/// SCS_Hitstun
///-----------------------------------------------------------------
public class SCS_Hitstun : SCState
{

    //public int minDuration = 3;
    private float spawnCloudVelocity = 10;

    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.Ctr.inControl = false;

        chr.Direction = Mathf.Sign(chr.Ctr.velocity.x);

        EffectManager.SpawnEffect("Cloud1", chr.transform.position);
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        if (chr.Ctr.velocity.y > 0)
            chr.Anim.ChangeAnimation(chr.StatesSO.hitstunUp_anim);
        else
            chr.Anim.ChangeAnimation(chr.StatesSO.hitstunUp_anim);


        chr.GetInputs();

        if (chr.Timer <= 60)
        {
            if (chr.Ctr.velocity.magnitude * 60 >= spawnCloudVelocity)
            {
                if (chr.Timer <= 30)
                {
                    if (chr.Timer % 4 == 0) EffectManager.SpawnEffect("Cloud1", chr.transform.position);
                }

                if (chr.Timer % 10 == 0) EffectManager.SpawnEffect("Cloud1", chr.transform.position);


            }
        }

        if (chr.Timer > chr.HitStunDuration)
        {
            chr.Ctr.inControl = true;
            chr.SCS_ChangeState(StaticStates.jumping);

            chr.RaiseComboOverEvent();
        }

        if (chr.Ctr.collision)
        {
            chr.SCS_ChangeState(StaticStates.hitland);
        }
    }

    public override void Exit(Character chr)
    {
        base.Exit(chr);

        chr.Ctr.freeze = false;

        chr.HitStunDuration = 0;
    }
}

///-----------------------------------------------------------------
/// SCS_Hitfreeze
///-----------------------------------------------------------------
public class SCS_Hitfreeze : SCState
{

    [SerializeField]
    public int duration = 5;

    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.Ctr.inControl = false;
        chr.Ctr.freeze = true;

        chr.Anim.ChangeAnimation(chr.StatesSO.hitfreeze_anim);
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        if (chr.Timer > chr.HitFreezeDuration)
        {
            chr.SCS_ChangeState(StaticStates.hitstun);
        }
    }

    public override void Exit(Character chr)
    {
        base.Exit(chr);

        chr.Ctr.inControl = false;
        chr.Ctr.freeze = false;

        chr.Ctr.forceMovement = chr.currentKnockback;
    }
}


///-----------------------------------------------------------------
/// SCS_Shield
///-----------------------------------------------------------------
public class SCS_Shield : SCState
{
    public override void Enter(Character chr)
    {
        base.Enter(chr);
        chr.shielding = true;
        chr.Flash(Color.yellow, 5);

        chr.Anim.ChangeAnimation(chr.StatesSO.shield_anim);
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        chr.GetInputs();

        //if (chr.DirectionalInput.x != 0)
        if (chr.StrongInputs.x != 0)
            chr.SCS_ChangeState(StaticStates.roll);

        chr.SetInputs(Vector2.zero);

        if (!chr.HoldShield) chr.SCS_Idle();


        if (chr.Jump) chr.SCS_ChangeState(StaticStates.idle);

        chr.CheckForTiltAttacks();
    }

    public override void Exit(Character chr)
    {
        base.Exit(chr);
        chr.shielding = false;
    }
}

///-----------------------------------------------------------------
/// SCS_Roll
///-----------------------------------------------------------------
public class SCS_Roll : SCState
{
    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.SetInputs(new Vector2(chr.FrozenInputX, 0));
        chr.Anim.ChangeAnimation(chr.StatesSO.roll_anim);
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        chr.SetInputs(new Vector2(Mathf.Sign(chr.FrozenInputX) * 0.8f, 0));

        if (chr.Ctr.onLedge)
        {
            chr.SetInputs(Vector2.zero);
        }

        if (chr.Anim.animationOver)
        {
            chr.SCS_Idle();
        }
            
    }

    public override void Exit(Character chr)
    {
        base.Exit(chr);

        chr.Direction = -chr.FrozenInputX;
    }
}

///-----------------------------------------------------------------
/// SCS_HitLand
///-----------------------------------------------------------------
public class SCS_HitLand : SCState
{
    public override void Enter(Character chr)
    {
        base.Enter(chr);
        chr.Ctr.freeze = true;
        chr.Ctr.inControl = false;
        chr.CollisionReflectVector = chr.Ctr.collisionReflect;
        chr.Flash(Color.white, 5);

        chr.Anim.ChangeAnimation(chr.StatesSO.hitland_anim);
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        chr.GetInputs();

        chr.SCS_CheckForTech();

        if (chr.Timer > chr.StatesSO.hitland_duration)
        {
            if (chr.CollisionReflectVector.magnitude * 60 > 20f)
            {
                chr.Ctr.forceMovement = chr.CollisionReflectVector * 60 * 0.8f; //80% reduction

                chr.SCS_ChangeState(StaticStates.hitstun);
            }
            else
            {
                if (chr.Ctr.grounded)
                {
                    chr.RaiseComboOverEvent();

                    chr.SCS_ChangeState(StaticStates.hitlanded);
                    chr.Ctr.inControl = true;
                }
                else
                {
                    chr.Ctr.forceMovement = chr.CollisionReflectVector * 60 * 0.8f; //80% reduction

                    chr.SCS_ChangeState(StaticStates.hitstun);
                }
            }
        }
    }

    public override void Exit(Character chr)
    {
        base.Exit(chr);
        chr.Ctr.freeze = false;
    }
}
///-----------------------------------------------------------------
/// SCS_HitLanded
///-----------------------------------------------------------------
public class SCS_HitLanded : SCState
{
    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.Anim.ChangeAnimation(chr.StatesSO.hitlanded_anim);
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        chr.GetInputs();

        if (chr.Timer > chr.StatesSO.hitLanded_minDuration)
        {
            chr.SCS_GetUpAfterHitLanded();
        }

        chr.SetInputs(Vector2.zero);
    }
}

///-----------------------------------------------------------------
/// SCS_StandUp
///-----------------------------------------------------------------
public class SCS_StandUp : SCState
{
    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.Anim.ChangeAnimation(chr.StatesSO.standup_anim);
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        chr.SetInputs(Vector2.zero);

        if (chr.Anim.animationOver)
        {
            chr.SCS_Idle();
        }
    }
}

///-----------------------------------------------------------------
/// SCS_Dead
///-----------------------------------------------------------------
public class SCS_Dead : SCState
{
    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.Anim.ChangeAnimation(chr.StatesSO.dead_anim);
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        chr.SetInputs(Vector2.zero);

        chr.Dead();
    }
}

///-----------------------------------------------------------------
/// SCS_Die
///-----------------------------------------------------------------
public class SCS_Die : SCState
{
    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.Anim.ChangeAnimation(chr.StatesSO.die_anim);
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        if (chr.Anim.animationOver)
            chr.SCS_ChangeState(StaticStates.dead);
    }
}

///-----------------------------------------------------------------
/// SCS_ThrowItem
///-----------------------------------------------------------------
public class SCS_ThrowItem : SCState
{
    public override void Enter(Character chr)
    {
        base.Enter(chr);

        if (!chr.Ctr.IsGrounded)
        {
            chr.Anim.ChangeAnimation(chr.StatesSO.throwitemaerial_anim);
        }
        else
        {
            chr.Anim.ChangeAnimation(chr.StatesSO.throwitem_anim);
        }
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        chr.SetInputs(Vector2.zero);

        if (chr.Anim.animationOver)
        {
            chr.GetInputs();

            if (chr.Ctr.IsGrounded)
            {
                chr.SCS_Idle();
            }
            else
            {
                chr.SCS_ChangeState(StaticStates.jumping);
            }
        }
    }

    public override void Exit(Character chr)
    {
        base.Exit(chr);

        if (chr is Player)
        {
            Player player = (Player)chr;

            player.ThrowItem(chr.DirectionalInput);
        }
    }
}

///-----------------------------------------------------------------
/// SCS_AirDodge
///-----------------------------------------------------------------
public class SCS_AirDodge : SCState
{
    protected int duration = 25;

    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.Flash(EffectManager.ColorDefend, duration);

        chr.GetInputs();
        chr.AirDodgeVector = chr.DirectionalInput;
        chr.AirDodgeVector = new Vector2(chr.AirDodgeVector.x, chr.AirDodgeVector.y * 0.3f); //otherwise too high up

        chr.Anim.ChangeAnimation(chr.StatesSO.airdodge_anim);
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        chr.GetInputs();

        if (chr.Timer <= 3) chr.Ctr.forceMovement = -chr.AirDodgeVector * 3f;
        else if (chr.Timer <= 8) chr.Ctr.forceMovement = chr.AirDodgeVector * 15f;

        if (chr.Timer > duration)
        {
            chr.SCS_ChangeState(StaticStates.jumping);
        }

        chr.CS_CheckLanding();
    }
}

///-----------------------------------------------------------------
/// SCS_ShieldHit
///-----------------------------------------------------------------
public class SCS_ShieldHit : SCState
{
    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.shielding = true;
        chr.Anim.ChangeAnimation(chr.StatesSO.shieldhit_anim);

    }
    public override void Execute(Character chr)
    {
        base.Execute(chr);

        chr.CS_CheckIfStillGrounded();

        if (chr.Anim.animationOver)
        {
            chr.SCS_Idle();
        }
    }

    public override void Exit(Character chr)
    {
        base.Exit(chr);

        chr.shielding = false;
    }
}

///-----------------------------------------------------------------
/// SCS_Grab
///-----------------------------------------------------------------
public class SCS_Grab : SCState
{
    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.Anim.ChangeAnimation(chr.StatesSO.grab_anim);
    }
    public override void Execute(Character chr)
    {
        base.Execute(chr);


        chr.SetInputs(Vector2.zero);

        if (chr.Timer >= 10)
        {
            if (chr.CheckForThrowAttacks())
            {
                chr.GetInputs();
            }
        }
    }
}

///-----------------------------------------------------------------
/// SCS_GetGrabbed 
///-----------------------------------------------------------------
public class SCS_GetGrabbed : SCState
{
    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.Ctr.inControl = false;
        chr.Anim.ChangeAnimation(chr.StatesSO.getGrabbed_anim);

    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);

        chr.Ctr.forceMovement = ((chr.currentDamage.position - chr.Position)) * 20f + Vector2.up;
    }

    public override void Exit(Character chr)
    {
        base.Exit(chr);

        chr.Ctr.inControl = true;
    }
}
