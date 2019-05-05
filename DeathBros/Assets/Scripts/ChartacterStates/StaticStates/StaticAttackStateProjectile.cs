using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StaticAttackStates/Special_Projectile")]
public class StaticAttackStateProjectile : StaticAttackStateSpecial
{
    public NES_Projectile projectile;
    public Vector2 projectileVelocity;

    public override SCS_Attack CreateAttackState()
    {
        SCS_SpecialAttackProjectile projectileAttack = new SCS_SpecialAttackProjectile();
        projectileAttack.animationName = animationName;
        projectileAttack.aerialLimit = aerialLimit;

        projectileAttack.projectile = projectile;
        projectileAttack.projectileVelocity = projectileVelocity;
        projectileAttack.attackBuff = new AttackBuff();
        return projectileAttack;
    }

    public override SCS_SpecialAttack CreateAttackState(ESpecial type)
    {
        SCS_SpecialAttackProjectile projectileAttack = new SCS_SpecialAttackProjectile();

        projectileAttack.animationName = animationName;
        projectileAttack.aerialLimit = aerialLimit;
        projectileAttack.attackBuff = new AttackBuff();

        projectileAttack.projectile = projectile;
        projectileAttack.projectileVelocity = projectileVelocity;
        projectileAttack.type = type;
        projectileAttack.comboPowerCost = comboPowerCost;
        return projectileAttack;
    }
}

public class SCS_SpecialAttackProjectile : SCS_SpecialAttack
{
    public NES_Projectile projectile;
    public Vector2 projectileVelocity;

    public override void Enter(Character chr)
    {
        base.Enter(chr);

        chr.ASpawnProjectile += SpawnProjectile;
    }

    public override void Execute(Character chr)
    {
        base.Execute(chr);
    }

    public override void Exit(Character chr)
    {
        base.Exit(chr);

        chr.ASpawnProjectile -= SpawnProjectile;
    }

    protected void SpawnProjectile(Character chr, Vector2 position)
    {
        chr.SCS_SpawnProjetile(projectile, projectileVelocity);
    }
}