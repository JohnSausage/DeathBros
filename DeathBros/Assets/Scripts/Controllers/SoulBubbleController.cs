using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulBubbleController : MonoBehaviour
{
    public Transform target;
    public float comboPower = 1;

    [Space]

    public Sprite sprite1;
    public Sprite sprite2;

    public int blinkTimeInFrames = 6;

    [Space]

    public float startVelocityMax = 5f;
    public float acceleration = 0.2f;

    public int moveStartVelocityTimer = 60;
    public int fadeOutTime = 60;

    private int moveTimer;
    private int blinkTimer = 0;
    private int fadeTimer = 0;

    protected SpriteRenderer spr;
    protected FrameAnimator fanim;

    protected Vector2 velocity;
    protected Vector2 startVelocity;

    protected void Start()
    {
        spr = GetComponentInChildren<SpriteRenderer>();
        spr.sprite = sprite1;

        fanim = GetComponent<FrameAnimator>();
        fanim.ChangeAnimation("blink");

        if (GameManager.Player.transform == null)
        {
            Debug.Log("Player as target ot found!");
        }
        else
        {
            target = GameManager.Player.transform;
        }

        RandomizeStartingVelocity();
    }

    protected void FixedUpdate()
    {
        Blink();
        MoveToTarget();
        //FadeOut();
        CheckForDestruction();
    }

    protected void Blink()
    {
        blinkTimer++;

        if (blinkTimer >= blinkTimeInFrames)
        {
            blinkTimer = 0;

            if (spr.sprite == sprite1)
            {
                spr.sprite = sprite2;
            }
            else
            {
                spr.sprite = sprite1;
            }
        }
    }

    protected void FadeOut()
    {
        if (fadeTimer < fadeOutTime)
        {
            fadeTimer++;
        }

        spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, (1 - fadeTimer / fadeOutTime));
    }

    protected void MoveToTarget()
    {
        if (target == null)
        {
            return;
        }

        moveTimer++;


        velocity *= 0.95f;

        //velocity = startVelocity * (1 - moveTimer / moveStartVelocityTimer) + targetVector * acceleration * (moveTimer / moveStartVelocityTimer);

        if (moveTimer >= 300)
        {
            fanim.ChangeAnimation("burst");
            fanim.AnimationOver += Burst;
        }
        //Vector2 targetVector = target.position - transform.position;

        //if (moveTimer < moveStartVelocityTimer)
        //{
        //    velocity = startVelocity * (1 - moveTimer / moveStartVelocityTimer) + targetVector * acceleration * (moveTimer / moveStartVelocityTimer);
        //}
        //else
        //{
        //    velocity += targetVector * acceleration;
        //}

        transform.Translate(velocity / 60f);

    }

    protected void CheckForDestruction()
    {
        float targetDistanceSqr = (target.position - transform.position).sqrMagnitude;

        if (moveTimer > 15 && targetDistanceSqr < 3f)
        {
            Vector2 targetVector = target.position - transform.position;
            velocity = targetVector * 3f;
        }

        if (moveTimer >= 15 && targetDistanceSqr < 0.5f)
        {
            AddComboPowerToPlayer();
            fanim.ChangeAnimation("burst");

            fanim.AnimationOver += Burst;

        }
    }

    protected void Burst(FrameAnimation burstAnimation)
    {
        if (burstAnimation == fanim.currentAnimation)
        {
            Destroy(gameObject);
        }
    }

    protected void AddComboPowerToPlayer()
    {
        if (target == null)
        {
            return;
        }

        Player player = target.GetComponent<Player>();

        if (player == null)
        {
            return;
        }

        player.ModifiyComboPower(comboPower);
    }

    protected void RandomizeStartingVelocity()
    {
        startVelocity = new Vector2(Random.Range(-startVelocityMax, startVelocityMax), Random.Range(-startVelocityMax, startVelocityMax));
        velocity = startVelocity;
    }
}
