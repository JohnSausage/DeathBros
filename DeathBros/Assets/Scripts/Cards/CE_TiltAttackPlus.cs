using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/TiltAttackPlus")]
public class CE_TiltAttackPlus : CardEffectSO
{
    public EAttackType attackType;
    public float damagePercent;

    public string attackTypeText;
    private int ID;

    private void Awake()
    {
        ID = Random.Range(0, 99999);    
    }

    public override void AcitivateEffect(Character chr)
    {
        base.AcitivateEffect(chr);

        BuffAddDamageToAttack buffAddDamageToAttack = new BuffAddDamageToAttack(attackType, damagePercent);
        buffAddDamageToAttack.ID = ID;

        chr.Buffs.Add(buffAddDamageToAttack);
    }

    public override void DeactivateEffect(Character chr)
    {
        base.DeactivateEffect(chr);

        Buff buff = chr.Buffs.Find(x => x.ID == ID);
        chr.Buffs.Remove(buff);
    }
}
