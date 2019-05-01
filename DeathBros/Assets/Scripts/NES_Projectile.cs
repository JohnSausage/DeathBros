using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NES_Projectile : MonoBehaviour
{
    public Vector2 Velocity;// { get; set; }
    public float destroyAfterSeconds;// { get; set; }

    protected FrameAnimator fanim;
    protected SpriteRenderer spr;

    protected int counter;

    protected void Awake()
    {
        fanim = GetComponent<FrameAnimator>();
        spr = GetComponent<SpriteRenderer>();

        counter = 0;
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
}
