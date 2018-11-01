using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField]
    protected CStates_Movement movementStates;

    [SerializeField]
    protected CS_TiltAttack attackState;

    [SerializeField]
    protected GameObject projectile;

    public CS_Attack currentAttack { get { return attackState; } }
    //public EnemyAI AI { get; protected set; }

    public override void Init()
    {
        base.Init();

        //AI = GetComponent<EnemyAI>();

        movementStates.Init(this);
        attackState.Init(this);

        CStates_InitExitStates();

        Anim.SpawnProjectile += SpawnProjectile;
    }

    private void Update()
    {
        //DirectionalInput = new Vector2(1, 0);

        /*
        DirectionalInput = AI.TargetDirection;

        if(AI.InAttackRange)
        {
            Attack = true;
        }
        else
        {
            Attack = false;
        }
        */
    }

    public void SpawnProjectile(Vector2 position, Vector2 direction)
    {
        GameObject spawnProjectile = Instantiate(projectile, position, Quaternion.identity);

        Item item = spawnProjectile.GetComponent<Item>();
        if (item != null) item.Velocity = direction;
    }

    public override bool CheckForTiltAttacks()
    {
        if (Attack)
        {
            CSMachine.ChangeState(GetAttackState(EAttackType.Jab1));

            return true;
        }
        else return false;
    }

    public override void Dead()
    {
        base.Dead();

        Destroy(gameObject);
    }
}
