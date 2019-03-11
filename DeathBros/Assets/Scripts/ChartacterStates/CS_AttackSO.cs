using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackStates/CS_Attack")]
public class CS_AttackSO : ScriptableObject
{
    public string attackName;
    public EAttackType attackType;
    public string animationName;

    //Replaces current attack with this one
    public virtual void InitState(Character chr)
    {
        CS_Attack oldState = chr.GetAttackState(attackType);
        if (oldState != null) chr.cStates.Remove(oldState);

    }
}