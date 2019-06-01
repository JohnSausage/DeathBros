using System;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField]
    protected float comboMultiplier = 1;
    public float ComboMultiplier { get { return comboMultiplier; } }

    [SerializeField]
    protected GameObject projectile;

    public override void Init()
    {
        base.Init();


        Anim.SpawnProjectile += SpawnProjectile;
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

    public override void Dead()
    {
        base.Dead();

        Destroy(gameObject);
    }
}
