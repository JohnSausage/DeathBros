using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FrameAnimator))]
public class Character : _MB
{
    public Vector2 Movement { get; protected set; }

    public FrameAnimator Anim { get; protected set; }
    public StateMachine CSMachine;// { get; protected set; }

    public CStates_Movement movementStates;

    public override void Init()
    {
        base.Init();

        CSMachine = new StateMachine();
        Anim = GetComponent<FrameAnimator>();

        movementStates.Init(this);
        CSMachine.ChangeState(movementStates.idle);
    }

    protected void FixedUpdate()
    {
        CSMachine.Update();
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
}
