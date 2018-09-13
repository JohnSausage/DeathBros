using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FrameAnimator))]
public class Character : _MB
{
    public Vector2 DirectionalInput { get; protected set; }
    public Vector2 StrongInputs;// { get; protected set; }

    public bool Jump { get; protected set; }
    public bool HoldJump { get; protected set; }

    [Space]

    public float jumpStrength = 20;
    public int jumps = 2;
    public int jumpsUsed = 0;

    [Space]

    public StateMachine CSMachine;// { get; protected set; }
    public FrameAnimator Anim { get; protected set; }
    public SpriteRenderer Spr { get; protected set; }
    public Controller2D Ctr { get; protected set; }

    public CStates_Movement movementStates;
    public CStates_AdvancedMovement advancedMovementStates;

    public bool IsFlipped
    {
        set
        {
            Spr.flipX = value;
        }
        get
        {
            return Spr.flipX;
        }
    }

    public override void Init()
    {
        base.Init();

        CSMachine = new StateMachine();
        Anim = GetComponent<FrameAnimator>();
        Anim.Init();

        Spr = GetComponent<SpriteRenderer>();
        Ctr = GetComponent<Controller2D>();

        movementStates.Init(this);
        advancedMovementStates.Init(this);
        CSMachine.ChangeState(advancedMovementStates.idle);
    }

    protected virtual void FixedUpdate()
    {
        StrongInputs = Vector2.zero;
        if(Mathf.Abs(Ctr.input.x - DirectionalInput.x) > 0.1f && Mathf.Abs(DirectionalInput.x) > 0.8f)
        {
            StrongInputs = new Vector2(DirectionalInput.x - Ctr.input.x, 0);
        }

        CSMachine.Update();

        Ctr.ManualFixedUpdate();

        Jump = false;
    }

    public void Spawn(Vector2 position)
    {

    }

    public virtual void ModHealth(float value)
    {

    }

    public virtual void Die()
    {

    }

    public virtual void SetInputs()
    {
        Ctr.input = DirectionalInput;
    }

    public virtual void SetInputs(Vector2 inputs)
    {
        DirectionalInput = inputs;
        SetInputs();
    }

    public virtual void SetInputs(float reduceControl)
    {
        DirectionalInput *= reduceControl;
        SetInputs();
    }

    /*
    public virtual void CS_CheckForJump()
    {
        if (jumpsUsed < jumps)
        {
            if (Ctr.grounded)
                CSMachine.ChangeState(advancedMovementStates.jumpsquat);
            else
                CSMachine.ChangeState(advancedMovementStates.doubleJumpsquat);
        }
    }

    public virtual void CS_StartJump()
    {
        Ctr.jumpVelocity = jumpStrength;
        CSMachine.ChangeState(advancedMovementStates.jumping);
    }
     */

    public virtual void CS_CheckLanding()
    {
        if (Ctr.grounded)
            CSMachine.ChangeState(advancedMovementStates.landing);
    }

    public virtual void CS_CheckIfStillGrounded()
    {
        if (!Ctr.grounded)
            CSMachine.ChangeState(advancedMovementStates.jumping);
    }

    public virtual void CS_SetIdle()
    {
        CSMachine.ChangeState(advancedMovementStates.idle);
    }
   
}
