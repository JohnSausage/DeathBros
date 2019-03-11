using UnityEngine;

[CreateAssetMenu(menuName = "AttackStates/CS_Jab")]
public class CS_AttackJabSO : CS_AttackSO
{
    public override void InitState(Character chr)
    {
        base.InitState(chr);

        CS_JabAttack jabAttack = new CS_JabAttack();
        jabAttack.animationName = animationName;
        jabAttack.attackType = attackType;

        jabAttack.Init(chr);
    }
}