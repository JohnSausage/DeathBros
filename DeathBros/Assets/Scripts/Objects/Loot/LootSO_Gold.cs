using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Loot/Gold")]
public class LootSO_Gold : LootSO
{

    public int goldAmountMin;

    public int goldAmountMax;


    public GameObject goldPrefab;

    public override void SpawnLoot(Vector2 spawnPosition)
    {
        int goldAmount = Random.Range(goldAmountMin, goldAmountMax + 1);

        for (int i = 0; i < goldAmount; i++)
        {
            GameObject goldCoin = Instantiate(goldPrefab, spawnPosition, Quaternion.identity);
            IAutoPickup autoPickup = goldCoin.GetComponent<IAutoPickup>();

            autoPickup.Spawn(Vector2.zero);
        }
    }
}
