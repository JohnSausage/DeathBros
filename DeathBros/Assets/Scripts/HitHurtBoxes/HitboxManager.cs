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

                Damage damage = currentFrame.hitboxes[i].GetDamage(spr.flipX);
                damage.hitID = currentID;
                damage.Owner = Chr;


                if (hitObject != null)
                {
                    if (hitObject is Character)
                    {
                        Character hitChr = (Character)hitObject;

                        //if (!hitChr.ContainsHidID(currentFrame.hitboxes[i].ID))
                        //{
                        //
                        //    hitChr.AddHitIDToQueue(currentFrame.hitboxes[i].ID);
                        damage.HitPosition = (Chr.transform.position + hitChr.transform.position) / 2f;

                        if (Chr is Player)
                        {
                            Player player = (Player)Chr;


                            //if (!Chr.ContainsHidID(currentFrame.hitboxes[i].ID))
                            //{
                            //damage = currentFrame.hitboxes[i].GetDamage(spr.flipX).AddDamage(player.soulCharge);

                            damage.AddDamage(player.soulCharge);
                            //if (PlayerHitsEnenmy != null) PlayerHitsEnenmy((Enemy)hitObject, damage);
                            //}
                        }
                        else if (Chr is Enemy)
                        {
                            //damage = currentFrame.hitboxes[i].GetDamage(spr.flipX);
                            //hitChr.TakeDamage(currentFrame.hitboxes[i].GetDamage(spr.flipX));
                        }

                        hitChr.GetHit(damage);

                        //}


                        //if (EnemyHit != null) EnemyHit(hitChr.transform.position);

                    }
                    else if (hitObject is Item)
                    {
                        //damage = currentFrame.hitboxes[i].GetDamage(spr.flipX);
                        //damage.hitID = currentID;
                        Item hitItem = (Item)hitObject;

                        hitItem.Owner = Chr;

                        hitItem.GetHit(damage);
                    }
                }


                /*
                Character hitCharacter = hit[0].GetComponentInParent<Character>();

                if (hitCharacter != null)
                {
                    if (!hitCharacter.ContainsHidID(currentFrame.hitboxes[i].ID))
                    {
                        if (Chr is Player)
                        {
                            Player player = (Player)Chr;
                            Enemy ene = (Enemy)hitCharacter;
                            Damage damage = currentFrame.hitboxes[i].GetDamage(spr.flipX).AddDamage(player.soulCharge);

                            hitCharacter.TakeDamage(damage);

                            if (PlayerHitsEnenmy != null) PlayerHitsEnenmy(ene, damage);
                        }
                        else
                        {
                            hitCharacter.TakeDamage(currentFrame.hitboxes[i].GetDamage(spr.flipX));
                        }

                        hitCharacter.AddHitIDToQueue(currentFrame.hitboxes[i].ID);

                        if (EnemyHit != null) EnemyHit(hitCharacter.transform.position);
                    }
                }
                */

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
            damageType = damageType
        };
    }

    public Damage AddDamage(float damage)
    {
        Damage rDamage = this.Clone();
        rDamage.damageNumber += damage;

        return rDamage;
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

    public void GenerateID()
    {
        hitID = UnityEngine.Random.Range(0, 99999);
    }
}