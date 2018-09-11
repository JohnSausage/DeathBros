using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtboxManager : _MB
{
    public List<CircleCollider2D> hurtboxes;

    private Frame previousFrame;
    private SpriteRenderer spr;
    private bool flipped;
    public override void Init()
    {
        base.Init();

        hurtboxes = new List<CircleCollider2D>();
        spr = GetComponent<SpriteRenderer>();
    }

    public void SetHurtboxes(Frame frame)
    {
        bool changedFlip = false;

        if (spr != null)
        {
            if (flipped != spr.flipX)

            {
                flipped = spr.flipX;
                changedFlip = true;
            }
        }

        if (previousFrame != frame || changedFlip) //only change hurtboxes when frame has changed
        {
            ClearHurtboxes();

            for (int i = 0; i < frame.hurtBoxes.Count; i++)
            {
                AddHurtboxColllider(frame.hurtBoxes[i]);
            }
        }

        previousFrame = frame;
    }

    private void AddHurtboxColllider(Hurtbox hurtbox)
    {
        CircleCollider2D newCollider = gameObject.AddComponent<CircleCollider2D>();

        if (!flipped)
        {
            newCollider.offset = hurtbox.position;
        }
        else
        {
            newCollider.offset = new Vector2(-hurtbox.position.x, hurtbox.position.y);
        }

        newCollider.radius = hurtbox.radius;

        hurtboxes.Add(newCollider);
    }

    private void ClearHurtboxes()
    {
        for (int i = 0; i < hurtboxes.Count; i++)
        {
            Destroy(hurtboxes[i]);
        }

        hurtboxes.Clear();
    }

}

[System.Serializable]
public class Hurtbox
{
    public Vector2 position;
    public float radius = 0.5f;

    public Hurtbox Clone()
    {
        return new Hurtbox
        {
            radius = radius,
            position = position
        };
    }
}