using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackStates/CS_Strong")]
public class CS_AttackStrongSO : CS_AttackSO
{
    public string chargeAnimationName;

    public override void InitState(Character chr)
    {
        base.InitState(chr);

        CS_SoulAttack strongAttack = new CS_SoulAttack();
        strongAttack.animationName = animationName;
        strongAttack.attackType = attackType;
        strongAttack.chargeAnimationName = chargeAnimationName;

        strongAttack.Init(chr);
    }
}