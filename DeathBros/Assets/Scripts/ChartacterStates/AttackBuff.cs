using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackBuff
{
    public float damageMulti = 1f;
    public float damageAdd = 0f;

    public EDamageEffect damageEffect = EDamageEffect.Normal;

    public AttackBuff()
    {
        damageMulti = 1f;
        damageAdd = 0f;
        damageEffect = EDamageEffect.Normal;
    }
}

public enum EDamageEffect { None, Normal, Fire, Water, Ice, Poison }