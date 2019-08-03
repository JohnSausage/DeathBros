using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDropper : MonoBehaviour
{
    [SerializeField]
    protected LootSO loot;

    private void Start()
    {
        Enemy enemy = GetComponent<Enemy>();

        if(enemy == null)
        {
            return;
        }

        enemy.AIsDying += DropLootOnDeath;
    }

    private void DropLootOnDeath(Vector2 position)
    {
        loot.SpawnLoot(position);
    }
}
