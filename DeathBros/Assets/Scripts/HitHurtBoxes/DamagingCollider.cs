using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingCollider : MonoBehaviour
{
    public Damage damage;
    public ContactFilter2D filter;

    public int tickDelay = 10;
    private int timer = 0;

    public Collider2D Col { get; protected set; }

    private RaycastHit2D[] collisions;
    private int colNr;

    void Start()
    {
        Col = GetComponent<Collider2D>();
        collisions = new RaycastHit2D[1];
    }

    private void FixedUpdate()
    {
        timer++;

        if (timer > tickDelay)
        {
            timer = 0;
            colNr = Col.Cast(Vector2.zero, filter, collisions);

            if (colNr > 0)
            {
                Character chr = collisions[0].transform.GetComponentInParent<Character>();

                chr.GetHit(damage);
            }
        }
    }
}
