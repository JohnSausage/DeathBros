using System;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_EnemiesDead : MonoBehaviour, ITrigger
{
    [SerializeField]
    protected List<Enemy> enemies;

    protected bool allDead;

    public event Action ATriggered;


    private void Start()
    {
        //Register all enemy dead events
        foreach (Enemy enemy in enemies)
        {
            enemy.AIsDead += CheckIfEnemiesDead;
        }
    }

    /// <summary>
    /// Sets allDead to true when all registered enemies are dead
    /// Executed on every enemy dead event
    /// </summary>
    protected void CheckIfEnemiesDead()
    {
        bool allDeadCheck = true;

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == null)
            {
                continue;
            }
            else if(enemies[i].dead == true)
            {
                continue;
            }

            allDeadCheck = false;
        }

        allDead = allDeadCheck;

        if(allDead == true)
        {
            if (ATriggered != null) ATriggered();
        }
    }

    public bool TriggerOn()
    {
        return allDead;
    }
}
