using UnityEngine;

[CreateAssetMenu(menuName = "StatusEffects/Burning")]
public class StatusEffect_Burning : StatusEffect
{
    public float speedFactor = 1.25f;
    public int tickTimeF = 30;

    public Damage burnDamage;

    void Awake()
    {
        effectType = EStatusEffectType.Burning;
        colorEffect = new Color((160f / 255f), (100f / 255f), (100f / 255f));
        effectAnimationName = "burning";
    }

    public override void ApplyEffect(Character chr)
    {
        base.ApplyEffect(chr);

        StatMod wetMovespeed = new StatMod(speedFactor, false, true, (int)(durationS * 60), "Movespeed");
        wetMovespeed.ApplyToCharacter(chr);

        StatMod wetAirspeed = new StatMod(speedFactor, false, true, (int)(durationS * 60), "Airspeed");
        wetAirspeed.ApplyToCharacter(chr);
    }

    public override void ManualUpdate(StatusEffectManager effectManager)
    {
        base.ManualUpdate(effectManager);

        if(effectManager.currentEffectTimer == 0)
        {
            return;
        }

        if(effectManager.currentEffectTimer%tickTimeF == 0)
        {

            effectManager.Chr.TakeDamage(burnDamage.Clone());
        }
    }


}