using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIState : IState
{
    public string stateName;

    protected EnemyAI ai;

    [SerializeField]
    protected float randomEnterChance = 50;
    public float RandomEnterChance { get { return randomEnterChance; } }

    [SerializeField]
    protected AIStateTransition transition;

    public void Init(EnemyAI ai)
    {
        this.ai = ai;

        ai.AIStates.Add(this);

        if (randomEnterChance > 0)
            ai.RandomStates.Add(this);
    }

    public virtual void Enter()
    {

    }

    public virtual void Execute()
    {
        if(transition.ConditionMet())
        {
            ChangeState(transition.exitStateName);
        }
    }

    public virtual void Exit()
    {
    }

    protected void ChangeState(AIState newState)
    {
        if (newState != null)
        {
            ai.aiMachine.ChangeState(newState);
        }
    }

    protected void ChangeState(string stateName)
    {
        ChangeState(ai.AIStates.Find(x => x.stateName == stateName));
    }
}

public enum ETransitonType { Timer, AnimationOver, PlayerDistance, GetHit, HitPlayer}
[System.Serializable]
public class AIStateTransition
{
    public string exitStateName;

    public ETransitonType transitionType;


    public bool ConditionMet()
    {
        switch (transitionType)
        {
            case ETransitonType.Timer:
                {
                    return true; 
                }
        }

        return false;
    }
}

[System.Serializable]
public class AI_Walk : AIState
{
    [SerializeField]
    private int duration = 60;
    private int timer = 0;

    [SerializeField]
    private int randomPlusMinusDuration = 30;
    private int randomizedDuration;

    private float dirX;

    public override void Enter()
    {
        base.Enter();
        timer = 0;
        randomizedDuration = duration + Random.Range(-randomPlusMinusDuration, randomPlusMinusDuration);

        dirX = Mathf.Sign(Random.Range(-1f, 1f));
    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        if (ai.chr.Ctr.onLedge)
            dirX = -dirX;

        ai.chr.SetInputs(new Vector2(dirX, 0));

        if (timer > randomizedDuration)
        {
            ai.aiMachine.ChangeState(ai.GetRandomState());
        }

        if (ai.InAggroRange)
        {
            ChangeState(ai.aiFollowPlayer);
        }
    }
}

[System.Serializable]
public class AI_Follow : AIState
{
    [SerializeField]
    private int duration = 60;
    private int timer = 0;

    [SerializeField]
    private int randomPlusMinusDuration = 30;
    private int randomizedDuration;

    private float dirX;

    public override void Enter()
    {
        base.Enter();
        timer = 0;
        randomizedDuration = duration + Random.Range(-randomPlusMinusDuration, randomPlusMinusDuration);

        dirX = Mathf.Sign(ai.TargetDirection.x);
    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        if (ai.chr.Ctr.onLedge)
            dirX = -dirX;

        ai.chr.SetInputs(new Vector2(dirX, 0));

        if (timer > randomizedDuration)
        {
            ai.aiMachine.ChangeState(ai.GetRandomState());
        }

        if (ai.InAttackRange)
        {
            ChangeState(ai.GetAttackState());
        }
    }
}




[System.Serializable]
public class AI_Flee : AIState
{
    [SerializeField]
    private int duration = 60;
    private int timer = 0;

    [SerializeField]
    private int randomPlusMinusDuration = 30;
    private int randomizedDuration;

    private float dirX;

    public override void Enter()
    {
        base.Enter();
        timer = 0;
        randomizedDuration = duration + Random.Range(-randomPlusMinusDuration, randomPlusMinusDuration);

        dirX = -Mathf.Sign(ai.TargetDirection.x);
    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        if (ai.chr.Ctr.onLedge)
            dirX = -dirX;

        ai.chr.SetInputs(new Vector2(dirX, 0));

        if (timer > randomizedDuration)
        {
            ai.aiMachine.ChangeState(ai.GetRandomState());
        }
    }
}

[System.Serializable]
public class AI_Attack : AIState
{
    protected int duration = 30;
    protected int timer = 0;


    public override void Enter()
    {
        base.Enter();

        timer = 0;

        ai.chr.SetInputs(ai.TargetDirection);
    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        ai.chr.SetInputs(ai.TargetDirection);

        ai.chr.SetAttack(true);

        if (timer > duration)
        {
        }
    }

    public override void Exit()
    {
        base.Exit();

        ai.chr.SetAttack(false);
    }
}