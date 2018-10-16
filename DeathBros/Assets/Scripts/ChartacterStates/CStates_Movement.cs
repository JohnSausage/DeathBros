using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CStates_Movement
{
    [Space]

    public CS_Idle idle;
    public CS_Walking walking;
    public CS_Jumpsquat jumpsquat;
    public CS_Jumping jumping;
    public CS_Landing landing;
    public CS_Hitstun hitstun;
    public CS_Hitfreeze hitfreeze;

    public virtual void Init(Character chr)
    {
        idle.Init(chr);
        walking.Init(chr);
        jumpsquat.Init(chr);
        jumping.Init(chr);
        landing.Init(chr);
        hitstun.Init(chr);
        hitfreeze.Init(chr);

        chr.CSMachine.ChangeState(idle);
    }
}

[System.Serializable]
public class CStates_AdvancedMovement : CStates_Movement
{
    [Space]

    public CS_DoubleJumpsquat doubleJumpsquat;
    public CS_Wallsliding wallsliding;
    public CS_WalljumpStart walljumpStart;
    public CS_Walljumping walljumping;
    public CS_Skid skid;
    public CS_Crouch crouch;

    public override void Init(Character chr)
    {
        base.Init(chr);

        doubleJumpsquat.Init(chr);
        wallsliding.Init(chr);
        walljumpStart.Init(chr);
        walljumping.Init(chr);
        skid.Init(chr);
        crouch.Init(chr);
    }
}

[System.Serializable]
public class CS_Idle : CState
{
    CS_Walking walking;
    CS_Jumpsquat jumpsquat;

    public override void Init(Character chr)
    {
        base.Init(chr);
        if (animationName == "") animationName = "idle";
    }

    public override void InitExitStates()
    {
        base.InitExitStates();

        walking = (CS_Walking)chr.GetState(typeof(CS_Walking));
        jumpsquat = (CS_Jumpsquat)chr.GetState(typeof(CS_Jumpsquat));
    }

    public override void Enter()
    {
        base.Enter();
        chr.jumpsUsed = 0;
    }

    public override void Execute()
    {
        base.Execute();

        chr.SetInputs();

        if (chr.DirectionalInput.y < 0f)
        {
            ChangeState(typeof(CS_Crouch));
        }

        if (Mathf.Abs(chr.DirectionalInput.x) != 0)
        {
            //ChangeState(chr.advancedMovementStates.walking);
            ChangeState(walking);
        }

        if (chr.Jump)
        {
            //ChangeState(chr.advancedMovementStates.jumpsquat);
            ChangeState(jumpsquat);
        }

        chr.CS_CheckIfStillGrounded();

        //if(damage != null)
        {
            //ChangeState(hitstun);
        }

        if (chr.CheckForSpecialAttacks() == false)
        {
            if (chr.CheckForSoulAttacks() == false)
            {
                chr.CheckForTiltAttacks();
            }
        }

        //if (!chr.CheckForSpecialAttacks()) { }
        //else if (!chr.CheckForSoulAttacks())
        //    chr.CheckForTiltAttacks();
        /*
        if (chr.Attack)
        {
            ChangeState(chr.GetState(typeof(CS_Attack)));
        }
        */
    }
}

[System.Serializable]
public class CS_Walking : CState
{
    [SerializeField]
    string animationSlow;
    [SerializeField]
    float direction = 0;

    protected FrameAnimation fAnimSlow;

    public override void Init(Character chr)
    {
        base.Init(chr);

        if (animationSlow != "")
        {
            fAnimSlow = chr.Anim.GetAnimation(animationSlow);
        }
    }
    public override void Enter()
    {
        base.Enter();

        direction = Mathf.Sign(chr.DirectionalInput.x);

        if (chr.DirectionalInput.x < 0) chr.Spr.flipX = true;
        if (chr.DirectionalInput.x > 0) chr.Spr.flipX = false;
    }

    public override void Execute()
    {
        base.Execute();

        chr.Anim.animationSpeed = Mathf.Abs(chr.DirectionalInput.x);

        if (Mathf.Abs(chr.DirectionalInput.x) < 0.5f && fAnimSlow != null)
        {
            chr.Anim.ChangeAnimation(fAnimSlow);
        }

        if (Mathf.Abs(chr.DirectionalInput.x) >= 0.7f)
        {
            chr.Anim.ChangeAnimation(animation);
        }

        chr.SetInputs();

        if (Mathf.Abs(chr.DirectionalInput.x) == 0f || Mathf.Sign(chr.DirectionalInput.x) != direction)
        {
            //chr.advancedMovementStates.skid.direction = direction;
            CState exit = (CS_Skid)chr.GetState(typeof(CS_Skid));

            if (exit != null)
            {
                CS_Skid skid = (CS_Skid)exit;
                skid.direction = direction;
            }
            else
            {
                exit = (CS_Idle)chr.GetState(typeof(CS_Idle));
            }
            //ChangeState(chr.advancedMovementStates.skid);
            ChangeState(exit);

        }

        if (chr.Jump)
        {
            //ChangeState(chr.advancedMovementStates.jumpsquat);
            ChangeState(typeof(CS_Jumpsquat));
        }

        if (chr.CheckForSpecialAttacks() == false)
        {
            if (chr.CheckForSoulAttacks() == false)
            {
                chr.CheckForTiltAttacks();
            }
        }


        if (chr.DirectionalInput.y < 0f)
        {
            ChangeState(typeof(CS_Crouch));
        }

        chr.CS_CheckIfStillGrounded();
    }
}

[System.Serializable]
public class CS_Skid : CState
{
    public float direction { get; set; }

    [SerializeField]
    int duration = 6;

    [SerializeField]
    int idleOutDuration = 3;
    private int timer;

    private int idleTimer;

    private bool changedDirection;

    public override void Enter()
    {
        base.Enter();
        timer = 0;
        idleTimer = 0;

        changedDirection = false;

        chr.Anim.animationSpeed = 0.5f;
    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        chr.SetInputs();


        if (!changedDirection)
        {
            if (Mathf.Sign(chr.DirectionalInput.x) != direction && chr.DirectionalInput.x != 0) //don't change direction if there is no input
            {
                chr.Spr.flipX = !chr.Spr.flipX;
                changedDirection = true;
            }
        }


        if (chr.DirectionalInput.x == 0)
        {
            idleTimer++;
        }

        chr.SetInputs(new Vector2(direction * 0.5f, 0));

        if (timer >= duration)
        {
            //ChangeState(chr.advancedMovementStates.idle);
            ChangeState(typeof(CS_Idle));

            chr.SetInputs();
        }
        else if (idleTimer >= idleOutDuration)
        {
            ChangeState(typeof(CS_Idle));

            chr.SetInputs();
        }
        else if (chr.Jump)
        {
            //ChangeState(chr.advancedMovementStates.jumpsquat);
            ChangeState(typeof(CS_Jumpsquat));
        }
    }
}

[System.Serializable]
public class CS_Crouch : CState
{
    public override void Execute()
    {
        base.Execute();

        chr.SetInputs();
        chr.SetInputs(new Vector2(0, chr.DirectionalInput.y));

        if (chr.DirectionalInput.y >= -0.1f)
        {
            chr.CS_SetIdle();
        }

        if (chr.Jump)
        {
            //ChangeState(chr.advancedMovementStates.jumpsquat);
            ChangeState(typeof(CS_Jumpsquat));
        }

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

[System.Serializable]
public class CS_Jumpsquat : CState
{
    [SerializeField]
    int duration;
    private int timer;

    public override void Enter()
    {
        base.Enter();
        timer = 0;
    }

    public override void Execute()
    {
        base.Execute();

        chr.SetInputs(0.5f);

        timer++;

        if (timer >= duration)
        {
            //ChangeState(chr.advancedMovementStates.jumping);
            ChangeState(typeof(CS_Jumping));

            if (chr.HoldJump)
            {
                chr.Ctr.jumpVelocity = chr.stats.jumpStrength.CurrentValue;
            }
            else
            {
                chr.Ctr.jumpVelocity = chr.stats.jumpStrength.CurrentValue * 0.75f;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        chr.jumpsUsed++;
    }
}

[System.Serializable]
public class CS_DoubleJumpsquat : CState
{
    [SerializeField]
    int duration = 3;
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

        if (timer >= duration)
        {
            //ChangeState(chr.advancedMovementStates.jumping);
            ChangeState(typeof(CS_Jumping));
            chr.Ctr.jumpVelocity = chr.stats.jumpStrength.CurrentValue;
        }
    }

    public override void Exit()
    {
        base.Exit();
        chr.jumpsUsed++;

        chr.Ctr.fastFall = false;
    }
}

[System.Serializable]
public class CS_Landing : CState
{
    [SerializeField]
    int duration = 3;
    private int timer;

    public override void Enter()
    {
        base.Enter();

        timer = 0;
    }

    public override void Execute()
    {
        base.Execute();

        //chr.SetInputs(0.2f);
        chr.SetInputs();

        timer++;

        if (timer >= duration)
        {
            chr.SetInputs();
            if (chr.DirectionalInput.y < 0)
            {
                ChangeState(typeof(CS_Crouch));
            }
            else
            {
                chr.CS_SetIdle();
            }
        }
    }
}

[System.Serializable]
public class CS_Jumping : CState
{
    [SerializeField]
    string jumpRisingAnimation;
    FrameAnimation jumpRisingFA;

    [SerializeField]
    int allowWallJumpAfterWallSlidingDuration = 5;
    int allowWallJumpAfterWallSlidingTimer;

    public override void Init(Character chr)
    {
        base.Init(chr);

        jumpRisingFA = chr.Anim.GetAnimation(jumpRisingAnimation);
    }

    public bool AllowWallJump
    {
        set
        {
            if (value == true)
            {
                allowWallJumpAfterWallSlidingTimer = allowWallJumpAfterWallSlidingDuration;
            }
            if (value == false)
            {
                allowWallJumpAfterWallSlidingTimer = 0;
            }
        }
        get
        {
            return allowWallJumpAfterWallSlidingTimer > 0;
        }
    }

    public override void Execute()
    {
        base.Execute();

        if (chr.Ctr.velocity.y < 0 && chr.StrongInputs.y < 0)
        {
            chr.Ctr.fastFall = true;
        }

        if (chr.Ctr.velocity.y > 0)
            chr.Anim.ChangeAnimation(jumpRisingFA);
        else
            chr.Anim.ChangeAnimation(animation);

        allowWallJumpAfterWallSlidingTimer--;

        chr.SetInputs();

        chr.CS_CheckLanding();

        if (chr.Jump)
        {
            if (AllowWallJump)
            {
                //ChangeState(chr.advancedMovementStates.walljumpStart);
                ChangeState(typeof(CS_WalljumpStart));
            }
            else if (chr.jumpsUsed < chr.stats.jumps.CurrentValue)
            {
                //ChangeState(chr.advancedMovementStates.doubleJumpsquat);
                ChangeState(typeof(CS_DoubleJumpsquat));
            }
        }

        chr.CheckForAerialAttacks();

        if (chr.Ctr.onWall)
        {
            //ChangeState(chr.advancedMovementStates.wallsliding);
            ChangeState(typeof(CS_Wallsliding));
        }
    }
}

[System.Serializable]
public class CS_Wallsliding : CState
{
    [SerializeField]
    string wallUpAnimation;

    private FrameAnimation wallUpFA;
    private CS_WalljumpStart walljumpStart;
    private CS_Jumping jumping;

    public override void Init(Character chr)
    {
        base.Init(chr);

        wallUpFA = chr.Anim.GetAnimation(wallUpAnimation);

        if (chr.Ctr.velocity.y > 0)
        {
            chr.Anim.ChangeAnimation(wallUpFA);
        }
        else
        {
            chr.Anim.ChangeAnimation(animation);
        }
    }

    public override void InitExitStates()
    {
        base.InitExitStates();

        walljumpStart = (CS_WalljumpStart)chr.GetState(typeof(CS_WalljumpStart));
        jumping = (CS_Jumping)chr.GetState(typeof(CS_Jumping));
    }

    public override void Enter()
    {
        //base.Enter(); //Dont automatically change animation to slideDwon, check vel.y first!
    }

    public override void Execute()
    {
        base.Execute();

        if (chr.Ctr.velocity.y > 0)
        {
            chr.Anim.ChangeAnimation(wallUpFA);
        }
        else
        {
            chr.Anim.ChangeAnimation(animation);
        }

        chr.Spr.flipX = chr.Ctr.wallDirection == -1;

        chr.SetInputs();

        chr.CS_CheckLanding();

        if (!chr.Ctr.onWall)
        {
            //ChangeState(chr.advancedMovementStates.jumping);
            ChangeState(typeof(CS_Jumping));
        }

        if (chr.Jump)
        {
            walljumpStart.walljumpDirection = -chr.Ctr.wallDirection;
            //ChangeState(chr.advancedMovementStates.walljumpStart);
            ChangeState(typeof(CS_WalljumpStart));
        }
    }

    public override void Exit()
    {
        base.Exit();

        jumping.AllowWallJump = true;
    }
}

[System.Serializable]
public class CS_WalljumpStart : CState
{
    public int walljumpDirection { get; set; }

    [SerializeField]
    float jumpHeightReductionFactor = 0.75f;
    [SerializeField]
    int duration = 3;

    private CS_Walljumping walljumping;
    private int timer;

    public override void InitExitStates()
    {
        base.InitExitStates();

        walljumping = (CS_Walljumping)chr.GetState(typeof(CS_Walljumping));
    }
    public override void Enter()
    {
        base.Enter();
        chr.Spr.flipX = chr.Ctr.wallDirection == 1;
        timer = 0;
        chr.SetInputs(Vector2.zero);
    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        if (timer >= duration)
        {
            chr.SetInputs(new Vector2(walljumpDirection, 0));
            chr.Ctr.jumpVelocity = chr.stats.jumpStrength.CurrentValue * jumpHeightReductionFactor;

            walljumping.walljumpDirection = walljumpDirection;
            //ChangeState(chr.advancedMovementStates.walljumping);
            ChangeState(walljumping);
        }
    }
}

[System.Serializable]
public class CS_Walljumping : CState
{
    public int walljumpDirection { get; set; }

    [SerializeField]
    int duration = 20;
    private int timer;

    private CS_Jumping jumping;

    public override void InitExitStates()
    {
        base.InitExitStates();

        jumping = (CS_Jumping)chr.GetState(typeof(CS_Jumping));
    }

    public override void Enter()
    {
        base.Enter();
        timer = 0;
        jumping.AllowWallJump = false;

    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        chr.SetInputs(new Vector2(walljumpDirection, 0));

        chr.CS_CheckLanding();

        if (chr.Ctr.onWall)
        {
            //ChangeState(chr.advancedMovementStates.wallsliding);
            ChangeState(typeof(CS_Wallsliding));
        }

        /*
        if (chr.Jump && chr.jumpsUsed < chr.jumps)
        {
            ChangeState(chr.movementStates.doubleJumpsquat);
        }
        */

        if (timer >= duration)
        {
            //ChangeState(chr.advancedMovementStates.jumping);
            ChangeState(jumping);
        }
    }
}

[System.Serializable]
public class CS_Hitstun : CState
{
    public float knockbackX { get; set; }
    private CS_Landing landing;
    public int minDuration = 3;
    private int timer;

    public override void InitExitStates()
    {
        base.InitExitStates();

        landing = (CS_Landing)chr.GetState(typeof(CS_Landing));
    }

    public override void Enter()
    {
        base.Enter();

        timer = 0;
        chr.Ctr.inControl = false;
    }

    public override void Execute()
    {
        base.Execute();

        chr.SetInputs();

        timer++;

        //chr.SetInputs(new Vector2(knockbackX, 0));

        if(timer == 15)
        {
            chr.Ctr.freeze = true;
        }

        if(timer == 45)
        {
            chr.Ctr.freeze = false;
        }

        if (chr.Ctr.IsGrounded && timer > minDuration)
        {
            chr.Ctr.inControl = true;
            ChangeState(landing);
        }
    }

    public override void Exit()
    {
        base.Exit();

        chr.Ctr.freeze = false;
    }
}

[System.Serializable]
public class CS_Hitfreeze : CState
{
    [SerializeField]
    private int duration = 5;
    private int timer;

    public override void Enter()
    {
        base.Enter();

        chr.Ctr.inControl = false;
        chr.Ctr.freeze = true;


        timer = 0;
    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        if(timer > duration)
        {
            ChangeState(typeof(CS_Hitstun));
        }
    }

    public override void Exit()
    {
        base.Exit();

        chr.Ctr.inControl = true;
        chr.Ctr.freeze = false;

        chr.Ctr.forceMovement = chr.currentKnockback;
    }
}