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
    public CS_HitLand hitLand;
    public CS_Shield shield;

    public virtual void Init(Character chr)
    {
        idle.Init(chr);
        walking.Init(chr);
        jumpsquat.Init(chr);
        jumping.Init(chr);
        landing.Init(chr);
        hitstun.Init(chr);
        hitfreeze.Init(chr);
        hitLand.Init(chr);
        shield.Init(chr);

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
    public CS_Dash dash;
    public CS_Crouch crouch;
    public CS_Roll roll;

    public override void Init(Character chr)
    {
        base.Init(chr);

        doubleJumpsquat.Init(chr);
        wallsliding.Init(chr);
        walljumpStart.Init(chr);
        walljumping.Init(chr);
        skid.Init(chr);
        dash.Init(chr);
        crouch.Init(chr);
        roll.Init(chr);
    }
}

[System.Serializable]
public class CS_Idle : CState
{
    CS_Walking walking;
    CS_Jumpsquat jumpsquat;
    CS_Crouch crouch;

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
        crouch = (CS_Crouch)chr.GetState(typeof(CS_Crouch));
    }

    public override void Enter()
    {
        base.Enter();
        chr.jumpsUsed = 0;

        chr.Anim.animationSpeed = 1.5f;
    }

    public override void Execute()
    {
        base.Execute();

        chr.GetInputs();

        if (chr.DirectionalInput.y < -0.5f)
            ChangeState(crouch);

        if (Mathf.Abs(chr.DirectionalInput.x) != 0)
            ChangeState(walking);

        if (chr.StrongInputs.x != 0)
            ChangeState(typeof(CS_Dash));

        if (chr.HoldShield)
            ChangeState(typeof(CS_Shield));

        if (chr.Jump)
            ChangeState(jumpsquat);

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
            chr.Anim.ChangeAnimation(fAnimSlow);


        if (Mathf.Abs(chr.DirectionalInput.x) >= 0.7f)
            chr.Anim.ChangeAnimation(animation);


        chr.GetInputs();

        if (Mathf.Abs(chr.DirectionalInput.x) == 0f || Mathf.Sign(chr.DirectionalInput.x) != direction)
        {
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
            ChangeState(exit);

        }

        if (chr.HoldShield)
            ChangeState(typeof(CS_Shield));

        if (chr.Jump)
            ChangeState(typeof(CS_Jumpsquat));


        if (chr.DirectionalInput.y < -0.5f)
            ChangeState(typeof(CS_Crouch));


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

[System.Serializable]
public class CS_Dash : CState
{
    [SerializeField]
    private int duration = 10;
    private int timer = 0;

    private float dirX;

    public override void Enter()
    {
        base.Enter();

        timer = 0;
        dirX = Mathf.Sign(chr.DirectionalInput.x);

        chr.Direction = dirX;
    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        chr.GetInputs();

        if (Mathf.Sign(chr.DirectionalInput.x) != dirX && chr.DirectionalInput.x != 0)
        {
            ChangeState(typeof(CS_Dash));
        }

        if (timer >= duration)
        {
            if (chr.DirectionalInput.x == 0)
            {
                ChangeState(typeof(CS_Idle));
            }
            else
            {
                ChangeState(typeof(CS_Walking));
            }
        }

        chr.SetInputs(new Vector2(dirX, 0));

        if (chr.Jump)
        {
            ChangeState(typeof(CS_Jumpsquat));
        }
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

        chr.GetInputs();


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

            chr.GetInputs();
        }
        else if (idleTimer >= idleOutDuration)
        {
            ChangeState(typeof(CS_Idle));

            chr.GetInputs();
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

        chr.GetInputs();
        chr.SetInputs(new Vector2(0, chr.DirectionalInput.y));

        if (chr.StrongInputs.y < 0)
        {
            chr.Ctr.fallThroughPlatform = true;
        }

        if (chr.DirectionalInput.y >= -0.25f)
        {
            chr.CS_SetIdle();
        }

        if (chr.Jump)
        {
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

        //chr.ModInputs(0.5f);

        timer++;

        if (timer >= duration)
        {
            if (chr.TiltInput != Vector2.zero)
            {
                chr.CheckForAerialAttacks();
                chr.Ctr.jumpVelocity = chr.stats.jumpStrength.CurrentValue * 0.6f;
            }
            else
            {
                ChangeState(typeof(CS_Jumping));

                chr.GetInputs();
                if (chr.HoldJump || chr.DirectionalInput.y > 0.75f)
                {
                    chr.Ctr.jumpVelocity = chr.stats.jumpStrength.CurrentValue;
                }
                else
                {
                    chr.Ctr.jumpVelocity = chr.stats.jumpStrength.CurrentValue * 0.6f;
                }
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
        chr.GetInputs();

        timer++;

        if (timer >= duration)
        {
            chr.GetInputs();
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

    public override void Init(Character chr)
    {
        base.Init(chr);

        jumpRisingFA = chr.Anim.GetAnimation(jumpRisingAnimation);
    }

    public override void Execute()
    {
        base.Execute();

        if (chr.Ctr.velocity.y > 0)
            chr.Anim.ChangeAnimation(jumpRisingFA);
        else
            chr.Anim.ChangeAnimation(animation);


        chr.GetInputs();

        if (chr.Ctr.velocity.y < 0 && chr.DirectionalInput.y < -0.5f)
        {
            chr.Ctr.fastFall = true;
        }


        chr.CS_CheckLanding();

        if (chr.Jump)
        {

            if (chr.jumpsUsed < chr.stats.jumps.CurrentValue)
            {
                ChangeState(typeof(CS_DoubleJumpsquat));
            }
        }

        chr.CheckForAerialAttacks();

        if (chr.Ctr.onWall)
        {
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

    private float dirBeforeWallslide;

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
        base.Enter(); //Dont automatically change animation to slideDwon, check vel.y first!

        dirBeforeWallslide = chr.Direction;

        if (chr.Ctr.velocity.y > 0)
        {
            chr.Anim.ChangeAnimation(wallUpFA);
        }
        else
        {
            chr.Anim.ChangeAnimation(animation);
        }
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

        chr.GetInputs();

        chr.CS_CheckLanding();

        if (!chr.Ctr.onWall)
        {
            //ChangeState(chr.advancedMovementStates.jumping);
            ChangeState(jumping);
        }

        if (chr.Jump || (chr.StrongInputs.x != 0 && Mathf.Sign(chr.StrongInputs.x) != chr.Ctr.wallDirection))
        {
            if (chr.Ctr.wallDirection != Mathf.Sign(chr.DirectionalInput.x))
            {
                walljumpStart.walljumpDirection = -chr.Ctr.wallDirection;
                //ChangeState(chr.advancedMovementStates.walljumpStart);
                ChangeState(walljumpStart);
            }
            else if (chr.jumpsUsed < chr.stats.jumps.CurrentValue)
            {
                //ChangeState(chr.advancedMovementStates.doubleJumpsquat);
                ChangeState(typeof(CS_DoubleJumpsquat));
            }
        }
    }

    public override void Exit()
    {
        base.Exit();

        //jumping.AllowWallJump = true;
        chr.Direction = dirBeforeWallslide;
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
        //jumping.AllowWallJump = false;

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
    [SerializeField]
    string hitstunFallAnimation;
    FrameAnimation hitstunFallFA;

    //public int freezeStart = 15;
    //public int freezeEnd = 45;
    public int minDuration = 3;

    //public float knockbackX { get; set; }
    private int timer;


    public override void Init(Character chr)
    {
        base.Init(chr);

        hitstunFallFA = chr.Anim.GetAnimation(hitstunFallAnimation);
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

        if (chr.Ctr.velocity.y > 0)
            chr.Anim.ChangeAnimation(animation);
        else
            chr.Anim.ChangeAnimation(hitstunFallFA);


        chr.GetInputs();

        timer++;

        //chr.SetInputs(new Vector2(knockbackX, 0));
        /*
        if (timer == freezeStart)
        {
            chr.Ctr.freeze = true;
        }

        if (timer == freezeEnd)
        {
            chr.Ctr.freeze = false;
        }

    
        if (chr.Ctr.IsGrounded && timer > minDuration)
        {
            chr.Ctr.inControl = true;
            ChangeState(landing);
        }
        */

        if (chr.Ctr.collision)
        {
            ChangeState(typeof(CS_HitLand));
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

    private void SetDuration(Damage damage)
    {
        duration = (int)damage.damageNumber;
        duration = Mathf.Clamp(duration, 4, 10);
    }

    public override void Init(Character chr)
    {
        base.Init(chr);

        chr.TakesDamage += SetDuration;
    }
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

        if (timer > duration)
        {
            ChangeState(typeof(CS_Hitstun));
        }
    }

    public override void Exit()
    {
        base.Exit();

        chr.Ctr.inControl = false;
        chr.Ctr.freeze = false;

        chr.Ctr.forceMovement = chr.currentKnockback;
    }
}

[System.Serializable]
public class CS_Shield : CState
{
    public override void Enter()
    {
        base.Enter();
        chr.shielding = true;
    }

    public override void Execute()
    {
        base.Execute();

        chr.GetInputs();

        if (chr.DirectionalInput.x != 0)
            ChangeState(typeof(CS_Roll));

        chr.SetInputs(Vector2.zero);

        if (!chr.HoldShield) ChangeState(typeof(CS_Idle));


        if (chr.Jump) ChangeState(typeof(CS_Jumpsquat));
    }

    public override void Exit()
    {
        base.Exit();
        chr.shielding = false;
    }
}

[System.Serializable]
public class CS_Roll : CState
{
    private float dirX;

    public override void Enter()
    {
        base.Enter();

        dirX = 0;
        if (chr.DirectionalInput.x != 0) dirX = Mathf.Sign(chr.DirectionalInput.x);
    }

    public override void Execute()
    {
        base.Execute();

        chr.SetInputs(new Vector2(dirX, 0));

        if (chr.Anim.animationOver)
            ChangeState(typeof(CS_Idle));
    }

    public override void Exit()
    {
        base.Exit();

        chr.Direction = -dirX;
    }
}

[System.Serializable]
public class CS_HitLand : CState
{
    [SerializeField]
    private int duration = 10;
    private int timer;

    private Vector2 collisionReflect;

    public override void Enter()
    {
        base.Enter();
        timer = 0;
        chr.Ctr.freeze = true;
        chr.Ctr.inControl = false;
        collisionReflect = chr.Ctr.collisionReflect;

    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        chr.GetInputs();

        if (chr.Shield)
        {
            ChangeState(typeof(CS_Idle));
            chr.Ctr.inControl = true;
            collisionReflect = Vector2.zero;
        }

        if (timer > duration)
        {
            if (collisionReflect.magnitude > 0.3f)
            {
                chr.Ctr.forceMovement = collisionReflect * 60 * 0.8f; //80% reduction

                ChangeState(typeof(CS_Hitstun));
            }
            else
            {
                ChangeState(typeof(CS_Idle));
                chr.Ctr.inControl = true;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        chr.Ctr.freeze = false;

    }
}