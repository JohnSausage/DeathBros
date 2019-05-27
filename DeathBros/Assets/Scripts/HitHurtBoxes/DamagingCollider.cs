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
    protected int colNr;

    protected virtual void Start()
    {
        SetCollider();

        collisions = new RaycastHit2D[10];
        damage.position = transform.position;
        damage.GenerateID();
    }

    protected virtual void SetCollider()
    {
        Col = GetComponent<Collider2D>();
    }

    protected virtual void FixedUpdate()
    {
        colNr = Col.Cast(Vector2.zero, filter, collisions);

        if (colNr > 0)
        {
            for (int i = 0; i < colNr; i++)
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
        yield return new WaitForSeconds(0.5f);
        damage.GenerateID();
    }
}
