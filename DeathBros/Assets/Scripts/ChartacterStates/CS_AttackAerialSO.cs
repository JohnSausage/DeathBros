using UnityEngine;

[CreateAssetMenu(menuName = "AttackStates/CS_Aerial")]
public class CS_AttackAerialSO : CS_AttackSO
{
    public int landingLag;

    public override void InitState(Character chr)
    {
        base.InitState(chr);

        CS_Aerial aerialAttack = new CS_Aerial();
        aerialAttack.animationName = animationName;
        aerialAttack.attackType = attackType;
        aerialAttack.landingLag = landingLag;

        aerialAttack.Init(chr);
    }
}