using System;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour, ICanTakeDamage, ITrigger
{
    [SerializeField]
    protected bool isBurning;

    protected FrameAnimator fanim;

    public event Action ATriggered;

    protected void Start()
    {
        fanim = GetComponent<FrameAnimator>();

        SetTorch(isBurning);
    }


    public void GetHit(Damage damage)
    {
        if(damage.StatusEffect == null)
        {
            return;
        }

        StatusEffect statusEffect = damage.StatusEffect;

        if(statusEffect.effectType == EStatusEffectType.Burning)
        {
            SetTorch(true);
        }
        else if (statusEffect.effectType == EStatusEffectType.Wet)
        {
            SetTorch(false);
        }
    }

    public void SetTorch(bool isBurning)
    {
        this.isBurning = isBurning;

        if (this.isBurning)
        {
            fanim.ChangeAnimation("on");
        }
        else
        {
            fanim.ChangeAnimation("off");
        }

        if (ATriggered != null) ATriggered();
    }

    public bool TriggerOn()
    {
        return isBurning;
    }
}

public interface ITrigger
{
    bool TriggerOn();

    event Action ATriggered;
}