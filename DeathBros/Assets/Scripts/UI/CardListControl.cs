﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardListControl : MonoBehaviour
{

    [SerializeField]
    private GameObject cardButtonTemplate;

    [SerializeField]
    private GameObject[] comboCards;

    [SerializeField]
    private List<GameObject> cards;

    private void Start()
    {
        cards = new List<GameObject>();

        //GenerateCards();
    }

    public void LoadCards()
    {
        cards.Clear();

        for (int i = 0; i < InventoryManager.cards.Count; i++)
        {
            AddCard(InventoryManager.cards[i]);
        }

        for (int i = 0; i < comboCards.Length; i++)
        {
            comboCards[i].GetComponent<CardButton>().SetCard(InventoryManager.comboCards[i]);
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

    }
}
