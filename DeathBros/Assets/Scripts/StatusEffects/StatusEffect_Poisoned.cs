using UnityEngine;

[CreateAssetMenu(menuName = "StatusEffects/Poisoned")]
public class StatusEffect_Poisoned : StatusEffect
{
    public int tickTimeF = 30;

    public Damage poisonDamage;

    void Awake()
    {
        effectType = EStatusEffectType.Poisoned;
        colorEffect = Color.green;
        effectAnimationName = "poisoned";
    }

    public override void ApplyEffect(Character chr)
    {
        base.ApplyEffect(chr);
    }

    public override void ManualUpdate(StatusEffectManager effectManager)
    {
        base.ManualUpdate(effectManager);

        if (effectManager.currentEffectTimer == 0)
        {
            return;
        }

        if (effectManager.currentEffectTimer % tickTimeF == 0)
        {

            effectManager.Chr.TakeDamage(poisonDamage.Clone());
        }
    }

}
