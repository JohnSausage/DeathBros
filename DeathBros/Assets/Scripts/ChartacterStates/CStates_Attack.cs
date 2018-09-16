using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CStates_Attack
{
    public CS_TestAttack testAttack;

    public virtual void Init(Character chr)
    {
        testAttack.Init(chr);
    }
}

public enum EAttackType { Jab, FTilt, DTilt, UTilt, DashAtk, NAir, FAir, DAir, UAir, BAir}

[System.Serializable]
public class CS_TestAttack : CState
{
    public EAttackType attackType;

    public override void Execute()
    {
        base.Execute();

        if(chr.Anim.animationOver)
        {
            chr.CS_SetIdle();
        }
    }
}

