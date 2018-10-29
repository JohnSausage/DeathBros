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

        chr.TakesDamage += TakeDamage;
    }

    public virtual void Execute()
    {
    }

    public virtual void Exit()
    {
        chr.TakesDamage -= TakeDamage;
        //reset animation speed
        chr.Anim.animationSpeed = 1;
        damage = null;
    }


    protected bool ChangeState(CState newState)
    {
        if (newState != null)
        {
            chr.CSMachine.ChangeState(newState);
            return true;
        }
        return false;
    }

    protected bool ChangeState(Type type)
    {
        CState newState = chr.GetState(type);

        return (ChangeState(newState));
        //if (newState != null)
        //{
        //    chr.CSMachine.ChangeState(newState);
        //}
    }

    protected void TakeDamage(Damage damage)
    {
        this.damage = damage;

        //CS_Hitfreeze cS_Hitfreeze = (CS_Hitfreeze)chr.GetState(typeof(CS_Hitfreeze));
        //hitstun.knockbackX = damage.knockBackDirection.normalized.x;

        ChangeState(chr.GetState(typeof(CS_Hitfreeze)));

    }
}
