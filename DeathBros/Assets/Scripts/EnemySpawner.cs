using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    protected GameObject enemyGO;

    protected FrameAnimator anim;

    protected bool enemySpawned;

    protected Collider2D spawnTriggerCollider;

    void Awake()
    {
        spawnTriggerCollider = GetComponent<Collider2D>();
        anim = GetComponent<FrameAnimator>();
        anim.Init();

        anim.ChangeAnimation("beforeSpawn");

        anim.AnimationOver += Remove;
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

        anim.ChangeAnimation("spawn");
    }

    private void Remove(FrameAnimation animation)
    {
        if (animation == anim.GetAnimation("spawn"))
        {
            anim.ChangeAnimation("afterSpawn");

            Instantiate(enemyGO, transform.position, Quaternion.identity);
        }
    }
}
