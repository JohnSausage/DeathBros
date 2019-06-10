
using UnityEngine;

[CreateAssetMenu(menuName = "AttackStates/CS_Grab")]
public class CS_AttackGrabSO : CS_AttackSO
{
    public override void InitState(Character chr)
    {
        base.InitState(chr);

        CS_GrabAttack grabAttack = new CS_GrabAttack();
        grabAttack.animationName = animationName;
        grabAttack.attackType = EAttackType.Grab;

        grabAttack.Init(chr);
    }
}