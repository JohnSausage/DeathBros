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

    public override void Init()
    {
        base.Init();

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

            int number = Physics2D.OverlapCircleNonAlloc(position + new Vector2(currentFrame.hitboxes[i].position.x * dirX,
                currentFrame.hitboxes[i].position.y),
                currentFrame.hitboxes[i].radius, hit, hitLayer);

            if (number > 0)
            {
                ICanTakeDamage hitObject = (ICanTakeDamage)hit[0].GetComponentInParent(typeof(ICanTakeDamage));

                Damage damage = currentFrame.hitboxes[i].GetDamage(spr.flipX).Clone();
                damage.hitID = currentID;
                damage.Owner = Chr;

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

public enum EDamageType { Normal, SweetSpot, SourSpot, LateHit }

[System.Serializable]
public class Damage
{
    public float damageNumber;
    public Vector2 knockBackDirection;
    public float baseKnockback;
    public float knockbackGrowth;
    public EDamageType damageType;
    public Color editorColor;

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
            editorColor = editorColor
        };
    }

    public void AddDamage(float damage)
    {
        damageNumber += damage;
    }

    /*
    public Damage AddDamage(float damage)
    {
        Damage rDamage = this.Clone();
        rDamage.damageNumber += damage;

        return rDamage;
    }
    */
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

    public void GenerateID()
    {
        hitID = UnityEngine.Random.Range(0, 99999);
    }
}