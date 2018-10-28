using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemController2D))]
public class Item : _MB, ICanTakeDamage
{
    public Vector2 Velocity { get { return ctr.velocity; } }
    public Character Owner { get; set; }

    [SerializeField]
    protected float weight = 20;

    [Space]

    [SerializeField]
    protected Damage damage;

    [SerializeField]
    protected float damagingSpeed = 10;

    [SerializeField]
    protected ContactFilter2D filter;

    protected ItemController2D ctr;
    protected Collider2D col;
    protected RaycastHit2D[] collisions;

    protected Queue<int> hitIDs = new Queue<int>();

    protected void Start()
    {
        ctr = GetComponent<ItemController2D>();
        col = GetComponentInChildren<Collider2D>();
        collisions = new RaycastHit2D[2];
    }

    public void GetHit(Damage damage)
    {
        if (!hitIDs.Contains(damage.hitID))
        {
            hitIDs.Enqueue(damage.hitID);

            ctr.SetVelocity = damage.Knockback(weight);
        }

        if (hitIDs.Count > 10) hitIDs.Dequeue();
    }

    protected void FixedUpdate()
    {
        if (Velocity.magnitude * 60 >= damagingSpeed)
        {
            int colNr = col.Cast(Vector2.zero, filter, collisions);

            if (colNr > 0)
            {
                for (int i = 0; i < collisions.Length; i++)
                {

                    Character chr = collisions[0].transform.GetComponentInParent<Character>();

                    if (chr != Owner)
                    {
                        damage.knockBackDirection = Velocity;
                        chr.GetHit(damage);
                    }
                }
            }
        }
    }
}
