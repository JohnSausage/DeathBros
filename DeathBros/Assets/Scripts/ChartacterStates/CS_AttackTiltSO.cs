using UnityEngine;

[CreateAssetMenu(menuName = "AttackStates/CS_Tilt")]
public class CS_AttackTiltSO : CS_AttackSO
{
    public override void InitState(Character chr)
    {
        base.InitState(chr);

        CS_TiltAttack tiltAttack = new CS_TiltAttack();
        tiltAttack.animationName = animationName;
        tiltAttack.attackType = attackType;

        tiltAttack.Init(chr);
    }
}