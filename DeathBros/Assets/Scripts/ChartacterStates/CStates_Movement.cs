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

    public void Init(Character chr)
    {
        idle.Init(chr);
        walking.Init(chr);
        jumpsquat.Init(chr);
        jumping.Init(chr);
        doubleJumpsquat.Init(chr);
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

        if (Mathf.Abs(chr.Movement.x) >= 0.1f)
        {
            ChangeState(chr.movementStates.walking);
        }

        if (chr.Jump)
        {
            ChangeState(chr.movementStates.jumpsquat);
        }

        if (!chr.Ctr.grounded)
        {
            ChangeState(chr.movementStates.jumping);
        }
    }
}

[System.Serializable]
public class CS_Walking : CState
{
    public override void Execute()
    {
        base.Execute();

        if (chr.Movement.x < 0) chr.IsFlipped = true;
        if (chr.Movement.x > 0) chr.IsFlipped = false;

        if (Mathf.Abs(chr.Movement.x) <= 0.1f)
        {
            ChangeState(chr.movementStates.idle);
        }

        if (chr.Jump)
        {
            ChangeState(chr.movementStates.jumpsquat);
        }

        if (!chr.Ctr.grounded)
        {
            ChangeState(chr.movementStates.jumping);
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

        timer++;

        if (timer >= duration)
        {
            ChangeState(chr.movementStates.jumping);
        }
    }

    public override void Exit()
    {
        base.Exit();

        chr.Ctr.jumpVelocity = chr.jumpStrength;
    }
}

[System.Serializable]
public class CS_DoubleJumpsquat : CState
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

        timer++;

        if (timer >= duration)
        {
            ChangeState(chr.movementStates.jumping);
        }
    }

    public override void Exit()
    {
        base.Exit();

        chr.Ctr.jumpVelocity = chr.jumpStrength;
    }

}
[System.Serializable]
public class CS_Jumping : CState
{
    public override void Enter()
    {
        base.Enter();

        chr.jumpsUsed++;
    }

    public override void Execute()
    {
        base.Execute();

        if (chr.Ctr.grounded)
        {
            ChangeState(chr.movementStates.idle);
        }

        if (chr.Jump && chr.jumpsUsed < chr.jumps)
        {
            ChangeState(chr.movementStates.doubleJumpsquat);
        }
    }
}