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
    public CS_DoubleJumpsquat doubleJumpsquat;
    public CS_Landing landing;
    public CS_Wallsliding wallsliding;
    public CS_WalljumpStart walljumpStart;
    public CS_Walljumping walljumping;
    public CS_Skid skid;

    public void Init(Character chr)
    {
        idle.Init(chr);
        walking.Init(chr);
        jumpsquat.Init(chr);
        jumping.Init(chr);
        doubleJumpsquat.Init(chr);
        landing.Init(chr);
        wallsliding.Init(chr);
        walljumpStart.Init(chr);
        walljumping.Init(chr);
        skid.Init(chr);
    }
}

[System.Serializable]
public class CS_Idle : CState
{
    public override void Enter()
    {
        base.Enter();
        chr.jumpsUsed = 0;
    }
    public override void Execute()
    {
        base.Execute();

        chr.SetInputs();

        if (Mathf.Abs(chr.DirectionalInput.x) != 0)
        {
            ChangeState(chr.movementStates.walking);
        }

        if (chr.Jump)
        {
            ChangeState(chr.movementStates.jumpsquat);
        }

        chr.CS_CheckIfStillGrounded();
    }
}

[System.Serializable]
public class CS_Walking : CState
{
    [SerializeField] string animationSlow;
    [SerializeField] float direction = 0;

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

        if (chr.DirectionalInput.x < 0) chr.IsFlipped = true;
        if (chr.DirectionalInput.x > 0) chr.IsFlipped = false;


        if (Mathf.Abs(chr.DirectionalInput.x) == 0f || Mathf.Sign(chr.DirectionalInput.x) != direction)
        {
            chr.movementStates.skid.direction = direction;
            ChangeState(chr.movementStates.skid);
        }

        if (chr.Jump)
        {
            ChangeState(chr.movementStates.jumpsquat);
        }

        chr.CS_CheckIfStillGrounded();
    }
}

[System.Serializable]
public class CS_Skid : CState
{
    public float direction { get; set; }

    [SerializeField] int duration;
    private int timer;

    private bool changedDirection;

    public override void Enter()
    {
        base.Enter();
        timer = 0;

        changedDirection = false;

        chr.Anim.animationSpeed = 0.5f;
    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        chr.SetInputs();

        /*
        if (chr.DirectionalInput.x < 0) chr.IsFlipped = true;
        if (chr.DirectionalInput.x > 0) chr.IsFlipped = false;
        */

        if (!changedDirection)
        {
            if (Mathf.Sign(chr.DirectionalInput.x) != direction && chr.DirectionalInput.x != 0) //don't change direction if there is no input
            {
                chr.IsFlipped = !chr.IsFlipped;
                changedDirection = true;
            }
        }

        chr.SetInputs(new Vector2(direction * 0.2f, 0));

        if (timer >= duration)
        {
            ChangeState(chr.movementStates.idle);
        }

        if (chr.Jump)
        {
            ChangeState(chr.movementStates.jumpsquat);
        }
    }
}
[System.Serializable]
public class CS_Jumpsquat : CState
{
    [SerializeField] int duration;
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
            ChangeState(chr.movementStates.jumping);

            if (chr.HoldJump)
            {
                chr.Ctr.jumpVelocity = chr.jumpStrength;
            }
            else
            {
                chr.Ctr.jumpVelocity = chr.jumpStrength * 0.75f;
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
    [SerializeField] int duration = 3;
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
            ChangeState(chr.movementStates.jumping);
            chr.Ctr.jumpVelocity = chr.jumpStrength;
        }
    }

    public override void Exit()
    {
        base.Exit();
        chr.jumpsUsed++;
    }
}

[System.Serializable]
public class CS_Landing : CState
{
    [SerializeField] int duration = 3;
    private int timer;

    public override void Enter()
    {
        base.Enter();

        timer = 0;
    }

    public override void Execute()
    {
        base.Execute();

        chr.SetInputs(0.2f);

        timer++;

        if (timer >= duration)
        {
            chr.CS_SetIdle();
        }
    }
}

[System.Serializable]
public class CS_Jumping : CState
{
    [SerializeField] string jumpRisingAnimation;
    FrameAnimation jumpRisingFA;

    [SerializeField] int allowWallJumpAfterWallSlidingDuration = 5;
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
                ChangeState(chr.movementStates.walljumpStart);
            }
            else
            {
                chr.CS_CheckForJump();
            }
        }

        if (chr.Ctr.onWall)
            ChangeState(chr.movementStates.wallsliding);
    }
}

[System.Serializable]
public class CS_Wallsliding : CState
{
    [SerializeField] string wallUpAnimation;
    private FrameAnimation wallUpFA;

    public override void Init(Character chr)
    {
        base.Init(chr);

        wallUpFA = chr.Anim.GetAnimation(wallUpAnimation);
    }

    public override void Enter()
    {
        base.Enter();
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

        if (!chr.Ctr.oldOnWall)
        {
            ChangeState(chr.movementStates.jumping);
        }

        if (chr.Jump)
        {
            chr.movementStates.walljumpStart.walljumpDirection = -chr.Ctr.wallDirection;
            ChangeState(chr.movementStates.walljumpStart);
        }
    }

    public override void Exit()
    {
        base.Exit();

        chr.movementStates.jumping.AllowWallJump = true;
    }
}

[System.Serializable]
public class CS_WalljumpStart : CState
{
    public int walljumpDirection { get; set; }

    [SerializeField] float jumpHeightReductionFactor = 0.75f;
    [SerializeField] int duration = 3;
    private int timer;

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
            chr.Ctr.jumpVelocity = chr.jumpStrength * jumpHeightReductionFactor;

            chr.movementStates.walljumping.walljumpDirection = walljumpDirection;
            ChangeState(chr.movementStates.walljumping);
        }
    }
}

[System.Serializable]
public class CS_Walljumping : CState
{
    public int walljumpDirection { get; set; }

    [SerializeField] int duration = 20;
    private int timer;

    public override void Enter()
    {
        base.Enter();
        timer = 0;
        chr.movementStates.jumping.AllowWallJump = false;

    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        chr.SetInputs(new Vector2(walljumpDirection, 0));

        chr.CS_CheckLanding();

        if (chr.Ctr.onWall)
            ChangeState(chr.movementStates.wallsliding);

        /*
        if (chr.Jump && chr.jumpsUsed < chr.jumps)
        {
            ChangeState(chr.movementStates.doubleJumpsquat);
        }
        */

        if (timer >= duration)
            ChangeState(chr.movementStates.jumping);
    }
}