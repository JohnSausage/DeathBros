using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ECardColor { Red, Blue, Green, White, Legendary }

[System.Serializable]
public class Card : MonoBehaviour
{
    public int level;

    public ECardColor connectColor;

    public CardEffectSO cardEffectSO;

    private void Start()
    {

    }
}

[System.Serializable]
public class CardData
{
    public int level;
    public ECardColor color;
    public ECardColor colorLeft;
    public ECardColor colorRight;
    public float triggerPosition;

    public CardEffect cardEffect;
}

[System.Serializable]
public class CardEffect
{
    public virtual void SetRandomValues(int level) { }
    public virtual void ModifyDamage(Damage damage) { }
    public virtual string GetEffectText() { return ""; }

    protected string TranslateAttackType(EAttackType attackType)
    {
        string text = "";

        switch (attackType)
        {
            case EAttackType.DTilt:
                {
                    text = "DT";
                    break;
                }
            case EAttackType.UTilt:
                {
                    text = "UT";
                    break;
                }
            case EAttackType.FTilt:
                {
                    text = "RT";
                    break;
                }
            case EAttackType.Jab1:
                {
                    text = "NT";
                    break;
                }
            case EAttackType.DAir:
                {
                    text = "DA";
                    break;
                }
            case EAttackType.UAir:
                {
                    text = "UA";
                    break;
                }
            case EAttackType.FAir:
                {
                    text = "RA";
                    break;
                }
            case EAttackType.NAir:
                {
                    text = "NA";
                    break;
                }
            case EAttackType.BAir:
                {
                    text = "LA";
                    break;
                }
            default: break;
        }

        return text;
    }
}

[System.Serializable]
public class CardEffect_SingleAttackStrength : CardEffect
{
    public EAttackType attackType;
    public float damageMultiplier;

    public override void SetRandomValues(int level)
    {
        base.SetRandomValues(level);

        attackType = InventoryManager.GetRandomEnum<EAttackType>();

        damageMultiplier = 1f + level * 0.1f;
    }

    public override void ModifyDamage(Damage damage)
    {
        base.ModifyDamage(damage);

        if (damage.attackType == attackType)
        {
            damage.damageNumber *= damageMultiplier;
        }
    }

    public override string GetEffectText()
    {
        string text = "";

        text = TranslateAttackType(attackType);

        return text;
    }
}

public enum EAttackClass { Tilts, Aerials, Strongs, Specials, Throws, None }

[System.Serializable]
public class CardEffect_AllAttackStrength : CardEffect
{
    public EAttackClass attackClass;
    public float damageMultiplier;

    public override void SetRandomValues(int level)
    {
        base.SetRandomValues(level);

        attackClass = InventoryManager.GetRandomEnum<EAttackClass>();

        damageMultiplier = 1f + level * 0.1f;
    }

    public override void ModifyDamage(Damage damage)
    {
        base.ModifyDamage(damage);

        if (Damage.CheckIfDamageApplies(damage.attackType, attackClass))
        {
            damage.damageNumber *= damageMultiplier;
        }
    }
}

public class CardEffect_StatMod : CardEffect
{
    public EAttackType attackType;

    public StatMod statMod;

    public override void SetRandomValues(int level)
    {
        base.SetRandomValues(level);

        attackType = InventoryManager.GetRandomEnum<EAttackType>();

        statMod = new StatMod(1f + level * 0.1f, false, true, 60 * level, "Movespeed");
    }

    public override void ModifyDamage(Damage damage)
    {
        base.ModifyDamage(damage);

        if (damage.attackType == attackType)
        {
            damage.ApplyStatMod = statMod;
        }
    }

    public override string GetEffectText()
    {
        string text = "";

        text = TranslateAttackType(attackType);

        return text;
    }
}
