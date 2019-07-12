using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable_Vine : MonoBehaviour, ICanTakeDamage
{
    public float hitpoints = 30;
    public float maxHitpoints = 30;

    public bool shrunk;

    public float timeTillGrowS = 5;

    protected FrameAnimator fanim;
    protected BoxCollider2D col;

    protected int lastHitID;

    protected void Start()
    {
        hitpoints = maxHitpoints;

        fanim = GetComponent<FrameAnimator>();
        col = GetComponent<BoxCollider2D>();

        if (shrunk)
        {
            fanim.ChangeAnimation("down");
            col.enabled = false;
        }
        else
        {
            fanim.ChangeAnimation("idle");
            col.enabled = true;
        }

        fanim.AnimationOver += OnAnimationOver;
    }



    public void GetHit(Damage damage)
    {
        if (damage.hitID == lastHitID)
        {
            return;
        }

        lastHitID = damage.hitID;

        if (shrunk == false)
        {
            if (damage.StatusEffect != null)
            {
                if (damage.StatusEffect.effectType == EStatusEffectType.Burning)
                {
                    hitpoints = 0;
                }
            }

            hitpoints -= damage.damageNumber;

            if (hitpoints <= 0)
            {
                hitpoints = 0;

                Shrink();
            }
            else
            {
                fanim.ChangeAnimation("hit");
            }
        }
    }

    protected void Shrink()
    {
        fanim.ChangeAnimation("shrink");

        shrunk = true;
        col.enabled = false;

        StartCoroutine(CWaitForGrow());
    }

    protected void OnAnimationOver(FrameAnimation anim)
    {
        if (anim.name == "hit")
        {
            fanim.ChangeAnimation("idle");
        }

        if (anim.name == "shrink")
        {
            fanim.ChangeAnimation("down");
        }

        if (anim.name == "grow")
        {
            fanim.ChangeAnimation("idle");
            shrunk = false;
            col.enabled = true;

            hitpoints = maxHitpoints;
        }


    }

    protected IEnumerator CWaitForGrow()
    {
        yield return new WaitForSeconds(timeTillGrowS);

        fanim.ChangeAnimation("grow");

    }
}
