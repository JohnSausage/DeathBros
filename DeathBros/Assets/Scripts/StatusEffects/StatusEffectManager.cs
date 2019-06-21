using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    public StatusEffect currentStatusEffect;

    protected Character chr;
    protected int currentEffectTimer = 0;

    protected void Start()
    {
        chr = GetComponent<Character>();

        chr.AGetsHit += CheckForStatusEffect;
    }

    protected void FixedUpdate()
    {
        if (currentStatusEffect == null)
        {
            return;
        }

        currentEffectTimer++;

        if (currentEffectTimer >= currentStatusEffect.durationS * 60)
        {
            currentStatusEffect.RemoveEffect(chr);

            currentStatusEffect = null;
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

        statusEffect.ApplyEffect(chr);
    }
}
