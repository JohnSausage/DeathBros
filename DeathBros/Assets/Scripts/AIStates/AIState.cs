using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIState : IState
{
    protected RatAI ai;

    public AIState(RatAI ai)
    {
        this.ai = ai;
    }

    public virtual void Enter()
    {
    }

    public virtual void Execute()
    {
    }

    public virtual void Exit()
    {
    }
}

[System.Serializable]
public class AI_Follow : AIState
{
    public AI_Follow(RatAI ai) : base(ai)
    {
    }

    public override void Execute()
    {
        base.Execute();

        ai.enemy.SetInputs(ai.TargetDirection);
    }
}

[System.Serializable]
public class AI_Flee : AIState
{
    public AI_Flee(RatAI ai) : base(ai)
    {
    }

    public override void Execute()
    {
        base.Execute();

        ai.enemy.SetInputs(-ai.TargetDirection);
    }
}

[System.Serializable]
public class AI_Attack : AIState
{
    public AI_Attack(RatAI ai) : base(ai)
    {
    }

    public override void Execute()
    {
        base.Execute();

        ai.enemy.SetInputs(Vector2.zero);

        ai.enemy.SetAttack();
    }
}