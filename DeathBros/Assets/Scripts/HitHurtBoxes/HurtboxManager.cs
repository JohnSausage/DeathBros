using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtboxManager : _MB
{
    public List<CircleCollider2D> hurtboxes;

    private Frame previousFrame;

    public override void Init()
    {
        base.Init();

        hurtboxes = new List<CircleCollider2D>();
    }

    public void SetHurtboxes(Frame frame)
    {
        if (previousFrame != frame) //only change hurtboxes when frame has changed
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

        newCollider.offset = hurtbox.position;
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