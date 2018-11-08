using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private GameObject hitEffect1;
    private GameObject damageNumber;
    private GameObject cloudEffect;

    void Start()
    {
        hitEffect1 = (GameObject)Resources.Load("Effects/Hit1");
        damageNumber = (GameObject)Resources.Load("Effects/DamageNumber");
        cloudEffect = (GameObject)Resources.Load("Effects/Cloud1");

        //HitboxManager.EnemyHit += SpawnHitEffect;
        Character.TakesDamageAll += SpawnDamageNumber;
        Character.TakesDamageAll += SpawnHitEffect;
        Character.InKnockback += SpawnCloudEffect;
    }


    private void SpawnHitEffect(Damage damage, Vector2 position)
    {
        Instantiate(hitEffect1, damage.HitPosition, Quaternion.identity);

        //GameObject effect = Instantiate(hitEffect1, position, Quaternion.identity);
        //effect.GetComponent<Effect>().color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
    }

    private void SpawnDamageNumber(Damage damage, Vector2 position)
    {
        GameObject dmgNr = Instantiate(damageNumber, position + Vector2.up, Quaternion.identity);
        dmgNr.GetComponent<DamageNumber>().damageNumber = damage.damageNumber.ToString();

    }

    private void SpawnCloudEffect(Character chr)
    {
        Instantiate(cloudEffect, (Vector2)chr.transform.position + Vector2.down, Quaternion.identity);
    }
}
