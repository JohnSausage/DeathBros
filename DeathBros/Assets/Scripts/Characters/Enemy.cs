using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField]
    protected float comboMultiplier = 1;
    public float ComboMultiplier { get { return comboMultiplier; } }

    [SerializeField]
    protected CStates_Movement movementStates;

    [SerializeField]
    protected CS_TiltAttack normalAttack;

    [SerializeField]
    protected CS_Aerial aerialAttack;

    [SerializeField]
    protected CS_TiltAttack specialAttack;

    [SerializeField]
    protected GameObject projectile;

    public CS_Attack currentAttack { get { return normalAttack; } }

    public override void Init()
    {
        base.Init();

        movementStates.Init(this);
        normalAttack.Init(this);

        CStates_InitExitStates();

        Anim.SpawnProjectile += SpawnProjectile;
    }


    public void SpawnProjectile(Vector2 position, Vector2 direction)
    {
        if (projectile != null)
        {
            GameObject spawnProjectile = Instantiate(projectile, position, Quaternion.identity);

            Item item = spawnProjectile.GetComponent<Item>();
            if (item != null)
            {
                Vector2 velocity = direction;
                velocity.x *= Direction;

                item.Velocity = velocity;
                item.Owner = this;
                item.GenerateID();
            }
        }
    }

    public override bool CheckForTiltAttacks()
    {
        if (Attack)
        {
            CSMachine.ChangeState(GetAttackState(EAttackType.Jab1));

            return true;
        }
        return false;
    }

    public override bool CheckForSpecialAttacks()
    {
        if (Special)
        {
            Direction = DirectionalInput.x;

            CSMachine.ChangeState(GetAttackState(EAttackType.NSpec));
            return true;
        }
        return false;
    }

    public override bool CheckForAerialAttacks()
    {
        if (Attack)
        {
            CSMachine.ChangeState(GetAttackState(EAttackType.NAir));

            return true;
        }
        return false;
    }

    public override void Dead()
    {
        base.Dead();

        Destroy(gameObject);
    }
}
