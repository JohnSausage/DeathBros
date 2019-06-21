using UnityEngine;

[CreateAssetMenu(menuName = "StatusEffects/Wet")]
public class StatusEffect_Wet : StatusEffect
{
    public float weightIncrease = 0.5f;
    public float gravityIncrease = 0.5f;
    public float jumpStrengthFactor = 0.75f;
    public float speedFactor = 0.75f;

    void Awake()
    {
        effectType = EStatusEffectType.Wet;
        colorEffect = new Color((100f / 255f), (100f / 255f), (160f / 255f));
    }

    public override void ApplyEffect(Character chr)
    {
        base.ApplyEffect(chr);

        StatMod wetWeight = new StatMod((1f + weightIncrease), false, true, (int)(durationS * 60), "Weight");
        wetWeight.ApplyToCharacter(chr);

        StatMod wetGravity = new StatMod((1f + gravityIncrease), false, true, (int)(durationS * 60), "Gravity");
        wetGravity.ApplyToCharacter(chr);

        StatMod wetJumpStrength = new StatMod(jumpStrengthFactor, false, true, (int)(durationS * 60), "JumpStrength");
        wetJumpStrength.ApplyToCharacter(chr);

        StatMod wetMovespeed = new StatMod(speedFactor, false, true, (int)(durationS * 60), "Movespeed");
        wetMovespeed.ApplyToCharacter(chr);

        StatMod wetAirspeed = new StatMod(speedFactor, false, true, (int)(durationS * 60), "Airspeed");
        wetAirspeed.ApplyToCharacter(chr);
    }
}