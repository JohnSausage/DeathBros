using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class Buff
{
    public int ID { get; set; }

    public virtual void AddBuff(Character chr)
    {
        chr.Buffs.Add(this);
    }

    public virtual void RemoveBuff(Character chr)
    {
        chr.Buffs.Remove(this);
    }
}

[SerializeField]
public class BuffAddDamageToAttack : Buff
{
    public EAttackType attackType { get; protected set; }

    public float damagePercent { get; protected set; }

    public BuffAddDamageToAttack(EAttackType attackType, float damagePercent)
    {
        this.attackType = attackType;
        this.damagePercent = damagePercent;
    }
}