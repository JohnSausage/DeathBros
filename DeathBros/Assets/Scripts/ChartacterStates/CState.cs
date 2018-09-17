using System;

[System.Serializable]
public class CState : IState
{
    public string animationName = "idle";

    protected Character chr;
    protected FrameAnimation animation;
    protected Damage damage;
    //protected CS_Hitstun hitstun;

    public virtual void Init(Character chr)
    {
        this.chr = chr;
        animation = chr.Anim.GetAnimation(animationName);

        chr.cStates.Add(this);
    }

    public virtual void InitExitStates()
    {
        //hitstun = (CS_Hitstun)chr.GetState(typeof(CS_Hitstun));
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
        damage = null;
    }


    protected void ChangeState(CState newState)
    {
        if (newState != null)
            chr.CSMachine.ChangeState(newState);
    }

    protected void ChangeState(Type type)
    {
        CState newState = chr.GetState(type);

        if (newState != null)
        {
            chr.CSMachine.ChangeState(newState);
        }
    }

    public void TakeDamage(Damage damage)
    {
        this.damage = damage;

        CS_Hitstun hitstun = (CS_Hitstun)chr.GetState(typeof(CS_Hitstun));
        hitstun.knockbackX = damage.knockBackDirection.normalized.x;
        ChangeState(chr.GetState(typeof(CS_Hitstun)));
    }
}
