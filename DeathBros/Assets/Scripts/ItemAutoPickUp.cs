using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EItemAutoPickUpType { None, RandomCard }
public class ItemAutoPickUp : MonoBehaviour
{
    public EItemAutoPickUpType itemType;
    public int itemLevel = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (itemType == EItemAutoPickUpType.RandomCard)
            {
                InventoryManager.cards.Add(InventoryManager.CreateRandomCard());

                Destroy(gameObject);
            }
        }
    }
}
