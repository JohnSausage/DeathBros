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
    public CS_Turnaround turnaround;

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
        turnaround.Init(chr);
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
    [SerializeField] float direction = 0;

    public override void Enter()
    {
        base.Enter();

        direction = Mathf.Sign(chr.DirectionalInput.x);
    }

    public override void Execute()
    {
        base.Execute();


        chr.SetInputs();

        if (chr.DirectionalInput.x < 0) chr.IsFlipped = true;
        if (chr.DirectionalInput.x > 0) chr.IsFlipped = false;


        if (Mathf.Abs(chr.DirectionalInput.x) == 0f || Mathf.Sign(chr.DirectionalInput.x) != direction)
        {
            chr.movementStates.turnaround.direction = direction;
            ChangeState(chr.movementStates.turnaround);
        }

        if (chr.Jump)
        {
            ChangeState(chr.movementStates.jumpsquat);
        }

        chr.CS_CheckIfStillGrounded();
    }
}

[System.Serializable]
public class CS_Turnaround : CState
{
    public float direction { get; set; }

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

        timer++;

        chr.SetInputs();

        if (chr.DirectionalInput.x < 0) chr.IsFlipped = true;
        if (chr.DirectionalInput.x > 0) chr.IsFlipped = false;

        chr.SetInputs(new Vector2(direction, 0));

        if (chr.Jump)
        {
            ChangeState(chr.movementStates.jumpsquat);
        }

        if (timer >= duration)
        {
            ChangeState(chr.movementStates.idle);
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
    public override void Execute()
    {
        base.Execute();

        chr.SetInputs();

        chr.CS_CheckLanding();

        if (chr.Jump)
        {
            chr.CS_CheckForJump();
        }

        if (chr.Ctr.onWall)
            ChangeState(chr.movementStates.wallsliding);
    }
}

[System.Serializable]
public class CS_Wallsliding : CState
{
    public override void Enter()
    {
        base.Enter();

        chr.Spr.flipX = chr.Ctr.wallDirection == 1;
    }

    public override void Execute()
    {
        base.Execute();

        chr.SetInputs(0.1f);

        chr.CS_CheckLanding();

        if (!chr.Ctr.onWall)
            ChangeState(chr.movementStates.jumping);

        if (chr.Jump)
        {
            chr.movementStates.walljumpStart.walljumpDirection = -chr.Ctr.wallDirection;
            ChangeState(chr.movementStates.walljumpStart);
        }
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
    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        chr.SetInputs(new Vector2(walljumpDirection, 0));

        chr.CS_CheckLanding();

        if (chr.Ctr.onWall)
            ChangeState(chr.movementStates.wallsliding);

        if (timer >= duration)
            ChangeState(chr.movementStates.jumping);
    }
}