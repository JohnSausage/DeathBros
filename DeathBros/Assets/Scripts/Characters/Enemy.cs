using System;
using UnityEngine;

public class Enemy : Character, IActivatedByCamera
{
    public StaticAttackStateSO normalAttack;
    public StaticAttackStateSO specialAttack;

    protected SCS_Attack normalAttack_SCS;
    protected SCS_Attack specialAttack_SCS;

    //[SerializeField]
    //protected float comboMultiplier = 1;
    //public float ComboMultiplier { get { return comboMultiplier; } }

    public bool IsActive { get; set; }

    //[SerializeField]
    //protected GameObject projectile;

    public int ComboHitCounter { get; protected set; }
    public float ComboDamageCounter { get; protected set; }

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

        if (specialAttack == null)
        {

        }
        else
        {
            specialAttack_SCS = specialAttack.CreateAttackState();
        }

        IsActive = false;
    }

    protected override void FixedUpdate()
    {
        if (IsActive)
        {
            base.FixedUpdate();

            if (CameraController.Instance.ActivationBounds.Contains(transform.position) == false)
            {
                IsActive = false; ;
            }

            if(HitStunDuration <= 0)
            {
                ComboHitCounter = 0;
                ComboDamageCounter = 0;
            }

            HoldShield = false;
            Shield = false;
            Attack = false;
            Special = false;
        }
        else
        {
            if (CameraController.Instance.ActivationBounds.Contains(transform.position) == true)
            {
                IsActive = true;
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

    protected override void OnTakeDamage(Damage damage)
    {
        base.OnTakeDamage(damage);

        if(HitStunDuration >= 0)
        {
            if (damage.damageNumber >= 5)
            {
                ComboHitCounter++;
            }

            ComboDamageCounter += damage.damageNumber;
        }

        //EffectManager.SpawnSoulBubbles(Mathf.RoundToInt(currentDamage.damageNumber / 8 * ComboHitCounter), transform.position);
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
                SCS_ChangeState(normalAttack_SCS);
            }
        }

        if (Special)
        {
            if (specialAttack == null)
            {

            }
            else
            {
                SCS_ChangeState(specialAttack_SCS);
            }
        }
    }

    public override void SCS_CheckForAerials()
    {
        base.SCS_CheckForAerials();

        if (Special)
        {
            if (specialAttack == null)
            {

            }
            else
            {
                SCS_ChangeState(specialAttack_SCS);
            }
        }
    }
}

public interface IActivatedByCamera
{
    bool IsActive { get; set; }
}