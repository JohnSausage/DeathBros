using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardListControl : MonoBehaviour
{

    [SerializeField]
    private GameObject cardButtonTemplate;

    [SerializeField]
    private List<GameObject> cards;

    private void Start()
    {
        cards = new List<GameObject>();

        GenerateCards();
    }

    private void GenerateCards()
    {
        for (int i = 0; i < 20; i++)
        {
            GameObject card = Instantiate(cardButtonTemplate) as GameObject;
            card.SetActive(true);

            card.GetComponent<CardButton>().SetCard(InventoryManager.CreateRandomCard());

            card.transform.SetParent(cardButtonTemplate.transform.parent);
        }
    }


    public void CardClicked(CardData cardData)
    {

    }
}
