using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/ComboCardData")]
public class ComboCardDataSO : ScriptableObject
{
    public string cardName;

    public virtual void ApplyEffect(Player player)
    {
        
    }

    public virtual void RemoveEffect(Player player)
    {

    }

    public virtual Damage ModifyDamage(Damage damage)
    {
        return damage;
    }
}
