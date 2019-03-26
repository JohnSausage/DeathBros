using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardListControl : MonoBehaviour
{

    [SerializeField]
    private GameObject cardButtonTemplate;



    [SerializeField]
    private List<GameObject> cards;
    public List<GameObject> Cards { get { return cards; } }

    public CardData clickedCard;

    private void Start()
    {
        cards = new List<GameObject>();

        //GenerateCards();
    }

    public void LoadCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Destroy(cards[i].gameObject);
        }
        cards.Clear();
        


        for (int i = 0; i < InventoryManager.cards.Count; i++)
        {
            AddCard(InventoryManager.cards[i]);
        }

    }

    private void GenerateCards()
    {
        for (int i = 0; i < 20; i++)
        {
            GameObject card = Instantiate(cardButtonTemplate) as GameObject;
            card.SetActive(true);

            card.GetComponent<CardButton>().SetCard(InventoryManager.CreateRandomCard());

            card.transform.SetParent(cardButtonTemplate.transform.parent);

            cards.Add(card);
        }
    }

    public void AddCard(CardData cardData)
    {
        GameObject card = Instantiate(cardButtonTemplate) as GameObject;
        card.SetActive(true);

        card.GetComponent<CardButton>().SetCard(cardData);

        card.transform.SetParent(cardButtonTemplate.transform.parent);

        cards.Add(card);
    }

    public void CardClicked(CardData cardData)
    {
        clickedCard = cardData;
    }
}
