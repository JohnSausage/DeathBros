using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Chest : MonoBehaviour, ICanInteract
{
    [SerializeField]
    protected LootSO loot;

    [SerializeField]
    protected bool isOpen;

    [SerializeField]
    protected List<Sprite> sprites;




    protected SpriteRenderer spr;

    private void Start()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    public void StartInteraction(Character chr)
    {
        if(isOpen)
        {
            return;
        }

        StartCoroutine(COpenChest());
    }

    protected IEnumerator COpenChest()
    {
        if(sprites.Count >= 3)
        {
            yield return new WaitForSeconds(5f / 60f);
            spr.sprite = sprites[1];

            yield return new WaitForSeconds(5f / 60f);
            spr.sprite = sprites[2];
        }

        SpawnItems();

        isOpen = true;

        yield return null;
    }

    protected void SpawnItems()
    {
        if(loot == null)
        {
            return;
        }

        loot.SpawnLoot(transform.position);
    }
}
