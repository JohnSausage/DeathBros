using System;
using UnityEngine;

public class HitboxManager : _MB
{
    public LayerMask hitLayer;
    private Frame previousFrame;
    private SpriteRenderer spr;
    private bool flipped;
    public int currentID;

    public Character chr { get; protected set; }

    public event Action<Character> EnemyHit;

    public override void Init()
    {
        base.Init();

        spr = GetComponent<SpriteRenderer>();
        chr = GetComponent<Character>();
    }

    public void DrawHitboxes(Frame currentFrame)
    {
        float dirX = 1;
        if (spr.flipX == true) dirX = -1;

        Collider2D[] hit = new Collider2D[5];
        Vector2 position = transform.position;

        if (previousFrame != currentFrame && currentFrame.newHitID)
            GenerateID();

        for (int i = 0; i < currentFrame.hitboxes.Count; i++)
        {
            currentFrame.hitboxes[i].ID = currentID;

            int number = Physics2D.OverlapCircleNonAlloc(position + new Vector2(currentFrame.hitboxes[i].position.x * dirX, 
                currentFrame.hitboxes[i].position.y), 
                currentFrame.hitboxes[i].radius, hit, hitLayer);

            if (number > 0)
            {
                Character enemy = hit[0].GetComponentInParent<Character>();

                if (enemy != null)
                {
                    if (!enemy.ContainsHidID(currentFrame.hitboxes[i].ID))
                    {
                        if (chr is Player)
                        {
                            Player player = (Player)chr;
                            enemy.TakeDamage(currentFrame.hitboxes[i].GetDamage(spr.flipX).AddDamage(player.soulCharge));
                        }
                        else
                        {
                            enemy.TakeDamage(currentFrame.hitboxes[i].GetDamage(spr.flipX));
                        }

                        enemy.AddHitIDToQueue(currentFrame.hitboxes[i].ID);

                        if (EnemyHit != null) EnemyHit(enemy);
                    }
                }
            }
        }

        previousFrame = currentFrame;
    }

    public void GenerateID()
    {
        currentID = UnityEngine.Random.Range(0, 99999);
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

    public Damage GetDamage(bool flipX)
    {
        Damage rDamage = damage.Clone();
        rDamage.knockBackDirection.x *= flipX ? -1 : 1;

        return rDamage;
    }
}

public enum EDamageType { Normal }

[System.Serializable]
public class Damage
{
    public float damageNumber;
    public Vector2 knockBackDirection;
    public float baseKnockback;
    public float knockbackGrowth;
    public EDamageType damageType;

    public Damage Clone()
    {
        return new Damage
        {
            damageNumber = damageNumber,
            knockBackDirection = knockBackDirection,
            baseKnockback = baseKnockback,
            knockbackGrowth = knockbackGrowth,
            damageType = damageType
        };
    }

    public Damage AddDamage(float damage)
    {
        Damage rDamage = this.Clone();
        rDamage.damageNumber += damage;

        return rDamage;
    }
}