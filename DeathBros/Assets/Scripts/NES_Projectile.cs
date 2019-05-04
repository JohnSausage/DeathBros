using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NES_Projectile : MonoBehaviour
{
    public string fly_anim;
    public string explode_anim;
    public Vector2 Velocity;// { get; set; }
    public float destroyAfterSeconds;// { get; set; }

    protected FrameAnimator fanim;
    protected SpriteRenderer spr;
    protected HitboxManager hibMn;
    protected CircleCollider2D col;
    public Character Owner { get; set; }


    protected int counter;

    protected void Awake()
    {
        fanim = GetComponent<FrameAnimator>();
        spr = GetComponent<SpriteRenderer>();
        hibMn = GetComponent<HitboxManager>();
        col = GetComponent<CircleCollider2D>();

        counter = 0;

        fanim.ChangeAnimation(fly_anim);
    }

    public void SetOwner(Character owner)
    {
        Owner = owner;

        hibMn.Chr = owner;

        hibMn.ACharacterHit += ExplodeOnHit;
    }

    protected void FixedUpdate()
    {
        counter++;

        transform.Translate(Velocity / 60);


        if (Velocity.x < 0)
        {
            spr.flipX = true;
        }
        else
        {
            spr.flipX = false;
        }


        if (counter > destroyAfterSeconds * 60)
        {
            Destroy(gameObject);
        }
    }

    protected void DestroyAfterAnimation(FrameAnimation anim)
    {
        if(anim.name == explode_anim)
        {
            Destroy(gameObject);
        }
    }

    protected void ExplodeOnHit(Character hitChr)
    {
        Velocity = Vector2.zero;
        fanim.ChangeAnimation(explode_anim);

        fanim.AnimationOver += DestroyAfterAnimation;
    }
}
