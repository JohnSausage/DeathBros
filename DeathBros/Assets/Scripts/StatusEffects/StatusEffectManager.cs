using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    public StatusEffect currentStatusEffect;

    public Character Chr { get; protected set; }
    protected FrameAnimator fanim;
    protected SpriteRenderer spr;

    public int currentEffectTimer { get; protected set; }

    protected void Start()
    {
        Chr = GetComponent<Character>();

        if(Chr == null)
        {
            Chr = GetComponentInParent<Character>();
        }

        spr = GetComponent<SpriteRenderer>();

        fanim = GetComponent<FrameAnimator>();

        if(fanim != null)
        {
            fanim.stopAnimation = true;
        }

        Chr.AGetsHit += CheckForStatusEffect;
    }

    protected void FixedUpdate()
    {
        if (currentStatusEffect == null)
        {
            return;
        }

        currentStatusEffect.ManualUpdate(this);

        currentEffectTimer++;

        if (currentEffectTimer >= currentStatusEffect.durationS * 60)
        {
            currentStatusEffect.RemoveEffect(Chr);

            currentStatusEffect = null;

            if(fanim != null)
            {
                fanim.stopAnimation = true;
            }
        }
    }

    protected void CheckForStatusEffect(Damage damage)
    {
        if (damage.StatusEffect == null)
        {
            return;
        }


        ApplyStatusEffect(damage.StatusEffect);

    }

    protected void ApplyStatusEffect(StatusEffect statusEffect)
    {
        currentStatusEffect = statusEffect;
        currentEffectTimer = 0;

        statusEffect.ApplyEffect(Chr);

        if(fanim != null)
        {
            if(statusEffect.effectAnimationName != "")
            {
                fanim.stopAnimation = false;
                fanim.ChangeAnimation(statusEffect.effectAnimationName, false);
            }
        }
    }
}
