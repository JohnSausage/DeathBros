using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemController2D))]
public class Item : _MB, ICanTakeDamage, ICanBePickedUp
{
    public Vector2 Velocity { get { return ctr.velocity; } set { ctr.SetVelocity = value; } }
    public Character Owner { get; set; }
    public bool IsSimulated { set { ctr.IsSimulated = value; } get { return ctr.IsSimulated; } }

    [SerializeField]
    protected float weight = 20;

    [SerializeField]
    protected bool destroyIfGrounded = false;

    [SerializeField]
    protected bool destroyOnCollision = false;

    [SerializeField]
    protected bool destroyOnCharacterHit = false;

    [SerializeField]
    protected int destroyAfterTime = 0;
    private int timer = 0;

    [Space]

    [SerializeField]
    protected Damage damage;

    public void GenerateID()
    {
        damage.GenerateID();
    }

    [SerializeField]
    protected float damagingSpeed = 10;

    [SerializeField]
    protected ContactFilter2D filter;

    protected ItemController2D ctr;
    protected Collider2D col;
    protected RaycastHit2D[] collisions;

    protected Queue<int> hitIDs = new Queue<int>();

    protected void Awake()
    {
        ctr = GetComponent<ItemController2D>();
        col = GetComponentInChildren<Collider2D>();
        collisions = new RaycastHit2D[2];

        FrameAnimator anim = GetComponent<FrameAnimator>();
        if (anim != null) anim.AnimationOver += Remove;
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
        if (destroyAfterTime != 0)
            timer++;

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

                        if (destroyIfGrounded) Destroy(gameObject);
                    }
                }
            }
        }

        if (destroyIfGrounded)
        {
            if (ctr.grounded)
            {
                FrameAnimator anim = GetComponent<FrameAnimator>();

                if (anim != null)
                {
                    anim.ChangeAnimation("die");
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        if (destroyOnCollision)
        {
            if (ctr.collision)
            {
                FrameAnimator anim = GetComponent<FrameAnimator>();

                if (anim != null)
                {
                    anim.ChangeAnimation("die");
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        if (destroyOnCharacterHit)
        {
            if (ctr.characterHit)
            {
                FrameAnimator anim = GetComponent<FrameAnimator>();

                if (anim != null)
                {
                    anim.ChangeAnimation("die");
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        if (timer > destroyAfterTime)
        {
            FrameAnimator anim = GetComponent<FrameAnimator>();

            if (anim != null)
            {
                anim.ChangeAnimation("die");
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    protected void Remove(FrameAnimation animation)
    {
        FrameAnimator anim = GetComponent<FrameAnimator>();

        if (anim != null)
        {
            if (animation == anim.GetAnimation("die"))
            {
                Destroy(gameObject);
            }
        }
    }
}

public interface ICanBePickedUp
{

}