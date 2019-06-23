using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingCollider : MonoBehaviour
{
    public Damage damage;
    public ContactFilter2D filter;

    public Collider2D Col { get; protected set; }

    protected RaycastHit2D[] collisions;
    protected int numberOfCollisions;

    protected virtual void Start()
    {
        SetCollider();

        collisions = new RaycastHit2D[10];
        damage.position = (Vector2)transform.position + Col.offset;
        damage.GenerateID();
    }

    protected virtual void SetCollider()
    {
        Col = GetComponent<Collider2D>();
    }

    protected virtual void FixedUpdate()
    {
        numberOfCollisions = Col.Cast(Vector2.zero, filter, collisions);

        if (numberOfCollisions > 0)
        {
            for (int i = 0; i < numberOfCollisions; i++)
            {
                ICanTakeDamage hitObject = collisions[i].transform.GetComponentInParent<ICanTakeDamage>();
                if (hitObject != null)
                {
                    hitObject.GetHit(damage);
                    StartCoroutine(EGenerateNewHitID());
                }
            }
        }
    }

    private IEnumerator EGenerateNewHitID()
    {
        yield return new WaitForSeconds((damage.hitStunFrames + 3) / 60f);
        damage.GenerateID();
    }
}
