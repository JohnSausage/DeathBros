using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    protected GameObject enemyGO;

    [SerializeField]
    protected Sprite spawnedSprite;

    protected bool enemySpawned;

    protected Collider2D spawnTriggerCollider;

    void Awake()
    {
        spawnTriggerCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!enemySpawned)
        {
            if (col.tag == "Player")
            {
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        enemySpawned = true;
        Instantiate(enemyGO, transform.position, Quaternion.identity);

        GetComponent<SpriteRenderer>().sprite = spawnedSprite;
    }
}
