using System;
using UnityEngine;

public class HitboxManager : _MB
{
    public LayerMask hitLayer;
    private Frame previousFrame;
    private SpriteRenderer spr;
    private bool flipped;
    public int currentID;

    public Character Chr { get; protected set; }

    //public static event Action<Vector2> EnemyHit;
    //public static event Action<Enemy, Damage> PlayerHitsEnenmy;

    protected override void Awake()
    {
        base.Awake();

        spr = GetComponent<SpriteRenderer>();
        Chr = GetComponent<Character>();
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

            Vector2 hitBoxPosition = position + new Vector2(currentFrame.hitboxes[i].position.x * dirX,
                currentFrame.hitboxes[i].position.y);

            int number = Physics2D.OverlapCircleNonAlloc(hitBoxPosition, currentFrame.hitboxes[i].radius, hit, hitLayer);

            if (number > 0)
            {
                for (int j = 0; j < number; j++)
                {
                    ICanTakeDamage hitObject = (ICanTakeDamage)hit[j].GetComponentInParent(typeof(ICanTakeDamage));

                    Damage damage = currentFrame.hitboxes[i].GetDamage(spr.flipX).Clone();
                    damage.hitID = currentID;
                    damage.Owner = Chr;
                    damage.position = hitBoxPosition;

                    if (Chr is Player)
                    {
                        Player player = (Player)Chr;

                        damage.AddDamage(player.soulCharge);
                    }

                    if (hitObject != null)
                    {
                        if (hitObject is Character)
                        {
                            Character hitChr = (Character)hitObject;

                            damage.HitPosition = (Chr.transform.position + hitChr.transform.position) / 2f;


                            //hitChr.GetHit(damage);

                        }
                        else if (hitObject is Item)
                        {
                            Item hitItem = (Item)hitObject;

                            hitItem.Owner = Chr;

                            //hitItem.GetHit(damage);
                        }

                        hitObject.GetHit(damage);
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

public enum EDamageType { Normal, SweetSpot, SourSpot, LateHit, Explosion }

[System.Serializable]
public class Damage
{
    public float damageNumber;
    public Vector2 knockBackDirection;
    public float baseKnockback;
    public float knockbackGrowth;
    public EDamageType damageType;
    public Color editorColor;
    public Vector2 positionalInfluence;
    public Vector2 position;

    public int hitID { get; set; }
    public Character Owner { get; set; }
    public Vector2 HitPosition { get; set; }

    public Damage Clone()
    {
        return new Damage
        {
            damageNumber = damageNumber,
            knockBackDirection = knockBackDirection,
            baseKnockback = baseKnockback,
            knockbackGrowth = knockbackGrowth,
            damageType = damageType,
            editorColor = editorColor,
            positionalInfluence = positionalInfluence,
            position = position
        };
    }

    public void AddDamage(float damage)
    {
        damageNumber += damage;
    }

    public Vector2 Knockback(float weight)
    {
        return Knockback(weight, 1);

    }

    public Vector2 Knockback(float weight, float percentHealth)
    {
        Vector2 knockback;

        knockback = knockBackDirection.normalized * (baseKnockback + knockbackGrowth * (1 - percentHealth));
        knockback *= (0.5f + (200 - weight) / 200);

        return knockback;
    }

    public Vector2 Knockback(Vector2 hitPosition, float weight, float percentHealth)
    {
        Vector2 knockback = knockBackDirection.normalized;

        if (position != Vector2.zero)
        {
            Vector2 hitDirection = (hitPosition - position).normalized;
            hitDirection.x *= positionalInfluence.x;
            hitDirection.y *= positionalInfluence.y;

            knockback += hitDirection;

            knockback.Normalize();
        }


        knockback *= ((baseKnockback + knockbackGrowth * (1 - percentHealth)));
        knockback *= (0.5f + (200 - weight) / 200);

        return knockback;
    }

    public void GenerateID()
    {
        hitID = UnityEngine.Random.Range(0, 99999);
    }
}