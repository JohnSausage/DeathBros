using UnityEngine;

public enum EStatusEffectType { None, Wet, Burning }

public class StatusEffect : ScriptableObject
{
    public EStatusEffectType effectType;
    public float durationS;

    public Color colorEffect;

    public virtual void ApplyEffect(Character chr)
    {
        chr.Spr.color = colorEffect;
    }

    public virtual void RemoveEffect(Character chr)
    {
        chr.Spr.color = Color.white;
    }
}