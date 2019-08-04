using UnityEngine;

[CreateAssetMenu(menuName = "Cards/ComboCard: Status effect")]
public class ComboCard_DamageEffect : ComboCardDataSO
{
    public EAttackType attackType;

    public StatusEffect effect;

    public override Damage ModifyDamage(Damage damage)
    {
        Damage returnDamage = damage;

        if (returnDamage.attackType == attackType)
        {
            returnDamage.StatusEffect = effect;
        }

        return returnDamage;
    }
}
