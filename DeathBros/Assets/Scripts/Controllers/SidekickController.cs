using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidekickController : MonoBehaviour
{
    [SerializeField]
    protected float movespeed = 4;

    [SerializeField]
    protected LayerMask collisionMask;



    public Vector2 playerOffset { get; protected set; }
    public FrameAnimator fanim { get; protected set; }
    public SpriteRenderer spr { get; protected set; }

    protected Player player;

    protected const float smoothSpeed = 0.02f;
    protected const float noiseSpeed = 0.02f;
    protected const float noiseStrength = 200f;

    protected const float collsisionCheckRange = 0.75f;
    protected const float collisionCheckDistance = 5f;


    protected void Start()
    {
        fanim = GetComponent<FrameAnimator>();
        spr = GetComponent<SpriteRenderer>();

        player = GameManager.Player;

        playerOffset = new Vector2(-1, 2);
    }

    protected void FixedUpdate()
    {
        MoveToPlayer();
    }

    protected void MoveToPlayer()
    {
        Vector2 newPosition = Vector2.zero;

        Vector2 targetPosition = player.Position + new Vector2(playerOffset.x * player.Direction, playerOffset.y);
        float targetDistance = ((Vector2)transform.position - targetPosition).magnitude;

        if (targetDistance > collisionCheckDistance)
        {
            Talk("Hey, wait for me!", 0.9f);
        }



        RaycastHit2D collisionSearch = Physics2D.CircleCast(transform.position, collsisionCheckRange, Vector2.zero, 0, collisionMask);


        if ((collisionSearch == true) && (targetDistance < collisionCheckDistance))
        {
            Vector2 antiCollisionVector = Vector2.zero;


            antiCollisionVector.y = collisionSearch.normal.y;

            if (collisionSearch.normal.x != 0)
            {
                antiCollisionVector.x = collisionSearch.normal.x;
            }

            newPosition = Vector3.Lerp(transform.position, (Vector2)transform.position + antiCollisionVector, smoothSpeed);
        }
        else
        {
            Vector2 perlinNoise;
            perlinNoise.x = (Mathf.PerlinNoise(Time.time * noiseSpeed * 60, 0) - 0.5f) * noiseStrength / 60;
            perlinNoise.y = (Mathf.PerlinNoise(0, Time.time * noiseSpeed * 60) - 0.5f) * noiseStrength / 60;

            targetPosition += perlinNoise;

            Vector2 targetVector = targetPosition - (Vector2)transform.position;

            newPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        }


        transform.position = newPosition;
        spr.flipX = (player.transform.position.x < transform.position.x);
    }

    protected void Talk(string text, float durationS = 1f)
    {
        DialogueManager.QueueDialogue(text);
        StartCoroutine(TalkAnimationCoroutine(durationS));
    }

    protected IEnumerator TalkAnimationCoroutine(float durationS)
    {
        fanim.ChangeAnimation("talking");

        yield return new WaitForSeconds(durationS);

        fanim.ChangeAnimation("idle");
    }
}
