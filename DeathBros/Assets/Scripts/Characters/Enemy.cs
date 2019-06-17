using System;
using UnityEngine;

public class Enemy : Character, IActivatedByCamera
{
    public StaticAttackStateSO normalAttack;
    protected SCS_Attack normalAttack_SCS;

    [SerializeField]
    protected float comboMultiplier = 1;
    public float ComboMultiplier { get { return comboMultiplier; } }

    public bool isActive { get; set; }

    [SerializeField]
    protected GameObject projectile;

    public int ComboHitCounter { get; protected set; }

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

        isActive = false;
    }

    protected override void FixedUpdate()
    {
        if (isActive)
        {
            base.FixedUpdate();

            if (GameManager.MainCamera.ActivateionBounds.Contains(transform.position) == false)
            {
                isActive = false; ;
            }

            if(HitStunDuration <= 0)
            {
                ComboHitCounter = 0;
            }
        }
        else
        {
            if (GameManager.MainCamera.ActivateionBounds.Contains(transform.position) == true)
            {
                isActive = true;
            }
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


    public override void SCS_Dead()
    {
        base.SCS_Dead();

        Destroy(gameObject);
    }

    protected override void OnTakeDamage()
    {
        base.OnTakeDamage();

        if(HitStunDuration >= 0)
        {
            ComboHitCounter++;
        }

        EffectManager.SpawnSoulBubbles(Mathf.RoundToInt(currentDamage.damageNumber / 8 * ComboHitCounter), transform.position);
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

public interface IActivatedByCamera
{
    bool isActive { get; set; }
}