using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private GameObject hitEffect1;

    void Start()
    {
        hitEffect1 = (GameObject)Resources.Load("Effects/Hit1");

        HitboxManager.EnemyHit += SpawnHitEffect;
    }

    void Update()
    {

    }

    private void SpawnHitEffect(Vector2 position)
    {
        GameObject effect = Instantiate(hitEffect1, position, Quaternion.identity);

        //effect.GetComponent<Effect>().color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
    }
}
