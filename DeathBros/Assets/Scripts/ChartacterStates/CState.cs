using System;

[System.Serializable]
public class CState : IState
{
    public string animationName = "idle";

    protected Character chr;
    protected FrameAnimation animation;

    public virtual void Init(Character chr)
    {
        this.chr = chr;
        animation = chr.Anim.GetAnimation(animationName);

        chr.cStates.Add(this);
    }

    public virtual void InitExitStates()
    {

    }

    public virtual void Enter()
    {
        //start animation
        chr.Anim.ChangeAnimation(animation);
    }

    public virtual void Execute()
    {
    }

    public virtual void Exit()
    {
        //reset animation speed
        chr.Anim.animationSpeed = 1;
    }

    
    protected void ChangeState(CState newState)
    {
        chr.CSMachine.ChangeState(newState);
    }
    
    protected void ChangeState(Type type)
    {
        CState newState = chr.GetState(type);

        if(newState != null)
        {
            chr.CSMachine.ChangeState(newState);
        }
    }
}
