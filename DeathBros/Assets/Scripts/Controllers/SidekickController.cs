using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidekickController : MonoBehaviour, IDialogueStarter
{
    [SerializeField]
    protected float movespeed = 4;

    [SerializeField]
    protected LayerMask collisionMask;

    [SerializeField]
    protected Dialogue waitDialogue;

    public Vector2 playerOffset { get; protected set; }
    public FrameAnimator fanim { get; protected set; }
    public SpriteRenderer spr { get; protected set; }

    protected Player player { get{return GameManager.Player; } }

    protected const float smoothSpeed = 0.02f;
    protected const float noiseSpeed = 0.02f;
    protected const float noiseStrength = 200f;

    protected const float collsisionCheckRange = 0.75f;
    protected const float collisionCheckDistance = 5f;

    protected bool talking = false;
    protected int talkTimerF = 0;
    protected int talkTime = 30;

    protected string[] talkBits = { "SideKick_Talk1"," SideKick_Talk2"," SideKick_Talk3", "SideKick_Talk4" };
    protected string laugh = "SideKick_Laugh";

    protected int textNumber = 0;

    protected float waitForSecondsToWhineAgain = 30;
    protected float whineTimer = 0;

    protected void Start()
    {
        fanim = GetComponent<FrameAnimator>();
        spr = GetComponent<SpriteRenderer>();


        playerOffset = new Vector2(-1, 2);

        SpawnCloseToPlayer();

        player.APlayerRespawn += SpawnCloseToPlayer;
    }

    protected void FixedUpdate()
    {
        whineTimer += 1f / 60f;

        MoveToPlayer();

        if(talking == true)
        {
            talkTimerF++;

            if(talkTimerF > talkTime)
            {
                talkTimerF = 0;

                AudioManager.PlaySound("SideKick_Talk1");
            }
        }
    }

    protected void SpawnCloseToPlayer()
    {
        transform.position = player.Position + Vector2.up;
    }

    protected void MoveToPlayer()
    {
        Vector2 newPosition = Vector2.zero;

        Vector2 targetPosition = player.Position + new Vector2(playerOffset.x * player.Direction, playerOffset.y);
        float targetDistance = ((Vector2)transform.position - targetPosition).magnitude;

        if ((talking == false) && (targetDistance > collisionCheckDistance + 1))
        {
            if (whineTimer >= waitForSecondsToWhineAgain)
            {
                WhineForWait("", 0.9f);
            }
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

            //Vector2 targetVector = targetPosition - (Vector2)transform.position;

            newPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        }


        transform.position = newPosition;
        spr.flipX = (player.transform.position.x < transform.position.x);
    }

    protected void WhineForWait(string text, float durationS = 1f)
    {
        whineTimer = 0;

        textNumber = 0;

        foreach (char c in text)
        {
            textNumber += c;
        }

        DialogueManager.DisplayComment(this, waitDialogue);
    }


    public FrameAnimator GetFanim()
    {
        return fanim;
    }

    public string GetEmotionAnim(EDialogueEmotion dialogueAction)
    {
        string retVal = "";
        switch(dialogueAction)
        {
            case EDialogueEmotion.Talking:
                {
                    retVal = "talking";
                    break;
                }

            default:
                {
                    break;
                }
        }

        return retVal;
    }

    public void StartConversation()
    {
        talking = true;
        fanim.ChangeAnimation("talking");
    }

    public void EndConversation()
    {
        fanim.ChangeAnimation("idle");
        talking = false;
    }

}
