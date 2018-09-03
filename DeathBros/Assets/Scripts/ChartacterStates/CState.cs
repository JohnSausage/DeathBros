using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CState : IState
{
    public string animationName;

    protected Character chr;
    protected FrameAnimation animation;

    public virtual void Init(Character chr)
    {
        this.chr = chr;
        animation = chr.Anim.GetAnimation(animationName);
    }

    public virtual void Enter()
    {
        chr.Anim.ChangeAnimation(animation);
    }

    public virtual void Execute()
    {
    }

    public virtual void Exit()
    {
    }

    protected void ChangeState(CState newState)
    {
        chr.CSMachine.ChangeState(newState);
    }
}
