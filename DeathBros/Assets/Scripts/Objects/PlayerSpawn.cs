using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour, ICanInteract
{
    [SerializeField]
    protected int spawnerID;

    public int SpawnerID { get { return spawnerID; } }

    public bool Activated;// { get; protected set; }

    [Space]

    [SerializeField]
    protected Sprite sp_activated, sp_deactivated;

    protected SpriteRenderer spr;


    public static event Action AOnActivate;



    private void Start()
    {
        spr = GetComponent<SpriteRenderer>();

        AOnActivate += DeactivateSpawn;
    }

    private void OnDestroy()
    {
        AOnActivate -= DeactivateSpawn;
    }

    public void ActivateSpawn()
    {
        //Deactivate every spawn when one gets activated
        if (AOnActivate != null)
        {
            AOnActivate();
        }

        Activated = true;
        spr.sprite = sp_activated;

        GameManager.Instance.saveData.spawnX = transform.position.x;
        GameManager.Instance.saveData.spawnY = transform.position.y;
    }

    protected void DeactivateSpawn()
    {
        spr.sprite = sp_deactivated;
        Activated = false;
    }

    public void StartInteraction(Character chr)
    {
        ActivateSpawn();
    }
}
