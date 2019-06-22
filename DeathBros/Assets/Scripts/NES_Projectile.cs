using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NES_Projectile : MonoBehaviour
{
    public string fly_anim;
    public string explode_anim;
    public bool chasePlayer = false;
    public Vector2 Velocity;// { get; set; }
    public float gravity;
    public float destroyAfterSeconds;// { get; set; }
    public bool destroyOnCollision;
    public LayerMask collisionMask;



    protected FrameAnimator fanim;
    protected SpriteRenderer spr;
    protected HitboxManager hibMn;
    protected CircleCollider2D col;
    public Character Owner { get; set; }
    public Character Target { get; protected set; }

    protected int counter;

    protected void Awake()
    {
        fanim = GetComponent<FrameAnimator>();
        spr = GetComponent<SpriteRenderer>();
        hibMn = GetComponent<HitboxManager>();
        col = GetComponent<CircleCollider2D>();

        counter = 0;

        fanim.Init();
        fanim.ChangeAnimation(fly_anim);

        if (chasePlayer == true)
        {
            Target = GameManager.Player;
        }
    }

    public void SetOwner(Character owner)
    {
        Owner = owner;

        hibMn.Chr = owner;

        hibMn.ACharacterHit += Explode;
    }

    protected void FixedUpdate()
    {
        counter++;

        if (Target != null)
        {
            if (chasePlayer == true)
            {
                Vector2 targetVector = Target.Position - (Vector2)transform.position;

                Velocity = targetVector.normalized;
            }
        }

        Velocity.y += (gravity);

        transform.Translate(Velocity / 60);


        if (Velocity.x < 0)
        {
            spr.flipX = true;
        }
        else if (Velocity.x > 0)
        {
            spr.flipX = false;
        }


        if (counter > destroyAfterSeconds * 60)
        {
            Explode();
        }

        if (destroyOnCollision == true)
        {
            RaycastHit2D collisionCheck = Physics2D.CircleCast((Vector2)transform.position + col.offset, col.radius, Vector2.zero, 0, collisionMask);

            if (collisionCheck == true)
            {
                Explode();
            }
        }
    }

    protected void DestroyAfterAnimation(FrameAnimation anim)
    {
        if (anim.name == explode_anim)
        {
            Destroy(gameObject);
        }
    }


    protected void Explode(Character hitChr = null, Damage damage = null)
    {
        Velocity = Vector2.zero;
        fanim.ChangeAnimation(explode_anim);

        fanim.AnimationOver += DestroyAfterAnimation;
    }
}
