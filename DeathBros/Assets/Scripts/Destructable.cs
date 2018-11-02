using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour, ICanTakeDamage
{
    [SerializeField]
    protected float hitPoints = 10;

    protected Queue<int> hitIDs = new Queue<int>();

    public void GetHit(Damage damage)
    {
        if (!hitIDs.Contains(damage.hitID))
        {
            hitIDs.Enqueue(damage.hitID);

            hitPoints -= damage.damageNumber;

            if(hitPoints <= 0)
            {
                Destroy(gameObject);
            }
        }

        if (hitIDs.Count > 10) hitIDs.Dequeue();
    }
}
