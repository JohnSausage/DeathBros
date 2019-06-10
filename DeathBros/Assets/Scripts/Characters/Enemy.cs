using System;
using UnityEngine;

public class Enemy : Character
{
    public StaticAttackStateSO normalAttack;
    protected SCS_Attack normalAttack_SCS;

    [SerializeField]
    protected float comboMultiplier = 1;
    public float ComboMultiplier { get { return comboMultiplier; } }

    [SerializeField]
    protected GameObject projectile;

    public override void Init()
    {
        base.Init();

        Anim.SpawnProjectile += SpawnProjectile;

        if (normalAttack == null)
        {

        }
        else
        {
            normalAttack_SCS = normalAttack.CreateAttackState();
        }
    }

    public void SpawnProjectile(Vector2 position, Vector2 direction)
    {
        //if (projectile != null)
        //{
        //    GameObject spawnProjectile = Instantiate(projectile, position, Quaternion.identity);

        //    Item item = spawnProjectile.GetComponent<Item>();
        //    if (item != null)
        //    {
        //        Vector2 velocity = direction;
        //        velocity.x *= Direction;

        //        item.Velocity = velocity;
        //        item.Owner = this;
        //        item.GenerateID();
        //    }

        //    Projectile proj = spawnProjectile.GetComponent<Projectile>();
        //    if (proj != null)
        //    {
        //        Vector2 velocity = direction;
        //        velocity.x *= Direction;

        //        proj.Velocity = velocity;
        //        proj.Owner = this;
        //        proj.GenerateID();
        //    }
        //}
    }

    public override void Die()
    {
        base.Die();

        SCS_ChangeState(StaticStates.die);
    }

    public override void SCS_Dead()
    {
        base.SCS_Dead();

        Destroy(gameObject);
    }

    protected override void OnTakeDamage()
    {
        base.OnTakeDamage();

        EffectManager.SpawnSoulBubbles(Mathf.RoundToInt(currentDamage.damageNumber / 2), transform.position);
    }

    public override void SCS_CheckForGroundAttacks()
    {
        base.SCS_CheckForGroundAttacks();

        if (Attack)
        {
            if (normalAttack == null)
            {

            }
            else
            {
                ChrSM.ChangeState(this, normalAttack_SCS);
            }
        }
    }
}
