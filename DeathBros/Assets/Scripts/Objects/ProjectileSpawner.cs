using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    public NES_Projectile projectile;

    [Space]

    public Vector2 spawnOffset;
    public float spawnIntervalS = 5f;
    public float spawnTimeOffsetS = 0;

    protected int timer = 0;

    protected void Start()
    {
        timer = -(int)(spawnTimeOffsetS * 60);
    }

    protected void FixedUpdate()
    {
        timer++;

        if (timer >= spawnIntervalS * 60f)
        {
            timer = 0;

            SpawnProjectile();
        }
    }

    protected void SpawnProjectile()
    {
        Instantiate(projectile.gameObject, (Vector2)transform.position + spawnOffset, Quaternion.identity, transform);
    }
}
