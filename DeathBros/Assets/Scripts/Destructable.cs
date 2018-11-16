using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour, ICanTakeDamage
{
    [SerializeField]
    protected float hitPoints = 10;

    [SerializeField]
    protected bool takeOnlyDamageFromExplosions = false;

    protected Queue<int> hitIDs = new Queue<int>();

    public FrameAnimator anim { get; protected set; }

    private void Start()
    {
        anim = GetComponent<FrameAnimator>();

        anim.AnimationOver += Remove;
    }

    public void GetHit(Damage damage)
    {
        if (takeOnlyDamageFromExplosions)
        {
            if (damage.damageType != EDamageType.Explosion)
                return;
        }


        if (!hitIDs.Contains(damage.hitID))
        {
            hitIDs.Enqueue(damage.hitID);

            hitPoints -= damage.damageNumber;

            if (hitPoints <= 0)
            {
                Die();
            }
        }

        if (hitIDs.Count > 10) hitIDs.Dequeue();
    }

    protected void Die()
    {
        anim.ChangeAnimation("die");
    }


    protected void Remove(FrameAnimation die)
    {
        if (die == anim.GetAnimation("die"))
            Destroy(gameObject);
    }
}