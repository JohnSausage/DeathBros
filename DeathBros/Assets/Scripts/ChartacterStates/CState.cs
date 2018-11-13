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
    }

    protected bool ChangeState(EAttackType attackType)
    {
        CS_Attack newState = chr.GetAttackState(attackType);

        return (ChangeState(newState));
    }

    protected void TakeDamage(Damage damage)
    {
        this.damage = damage;

        if (chr.shielding)
        {
            ChangeState(typeof(CS_ShieldHit));
        }
        else
        {
            ChangeState(typeof(CS_Hitfreeze));
        }
    }
}
