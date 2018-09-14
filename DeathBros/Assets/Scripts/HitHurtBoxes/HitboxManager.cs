using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxManager : _MB
{
    public LayerMask hitLayer;
    private Frame previousFrame;
    private SpriteRenderer spr;
    private bool flipped;
    public int currentID;

    public override void Init()
    {
        base.Init();

        spr = GetComponent<SpriteRenderer>();
    }

    public void DrawHitboxes(Frame currentFrame)
    {
        Collider2D[] hit = new Collider2D[5];
        Vector2 position = transform.position;

        for (int i = 0; i < currentFrame.hitboxes.Count; i++)
        {
            if (currentFrame.hitboxes[i].getNewID)
                GenerateID();

            currentFrame.hitboxes[i].ID = currentID;

            int number = Physics2D.OverlapCircleNonAlloc(position + currentFrame.hitboxes[i].position, currentFrame.hitboxes[i].radius, hit, hitLayer);

            if (number > 0)
            {
                Character enemy = hit[0].GetComponentInParent<Character>();

                if(enemy != null)
                {
                    enemy.TakeDamage(currentFrame.hitboxes[i].damage);
                }

            }
        }
    }

    public void GenerateID()
    {
        currentID = Random.Range(0, 99999);
    }
}

[System.Serializable]
public class Hitbox
{
    public Vector2 position;
    public float radius;
    public Damage damage;
    public int ID;
    public bool getNewID;

    public Hitbox Clone()
    {
        return new Hitbox
        {
            radius = radius,
            position = position,
            damage = damage
        };
    }
}

public enum EDamageType { Normal }

[System.Serializable]
public class Damage
{
    public float damageNumber;
    public Vector2 knockBackDirection;
    public float baseKnockback;
    public float KnockbackGrowth;
    public EDamageType damageType;
}