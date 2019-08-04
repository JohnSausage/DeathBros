using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/ComboCard: AttackDamage")]
public class ComboCard_AttackDamageBuff : ComboCardDataSO
{
    public EAttackType attackType;

    public int addToDamage = 5;

    public override Damage ModifyDamage(Damage damage)
    {
        Damage returnDamage = damage;

        if (returnDamage.attackType == attackType)
        {
            returnDamage.AddDamage(addToDamage);
        }

        return returnDamage;
    }
}
