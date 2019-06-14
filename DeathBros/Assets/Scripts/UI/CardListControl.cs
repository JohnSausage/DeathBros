using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardListControl : MonoBehaviour
{
    //[SerializeField]
    //private int NrOfColumnsForZooming = 6;

    [SerializeField]
    private GameObject cardButtonTemplate;

    [SerializeField]
    private List<GameObject> cards;
    List<GameObject> Cards { get { return cards; } }

    public CardData clickedCard;

    private ScrollRect scrollRect;

    private RectTransform selectedTransform;

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    private void Update()
    {
        if (selectedTransform)
            scrollRect.content.localPosition = scrollRect.GetSnapToPositionToBringChildIntoView(selectedTransform);
        else
            scrollRect.content.localPosition = Vector3.zero;
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

    public void ZoomToCard(GameObject cardGO)
    {
        if (cards.Contains(cardGO))
            selectedTransform = cardGO.GetComponent<RectTransform>();
    }
}


public static class ScrollRectExtensions
{
    public static Vector2 GetSnapToPositionToBringChildIntoView(this ScrollRect instance, RectTransform child)
    {
        Canvas.ForceUpdateCanvases();
        Vector2 viewportLocalPosition = instance.viewport.localPosition;
        Vector2 childLocalPosition = child.localPosition;
        Vector2 result = new Vector2(
            0/* - (viewportLocalPosition.x + childLocalPosition.x)*/,
            0 - (viewportLocalPosition.y + childLocalPosition.y)
        );
        return result;
    }
}
