using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardButton : MonoBehaviour
{
    public bool HasCard { get; set; }

    [SerializeField]
    private GameObject figure10, figure1, trigger;

    [SerializeField]
    private CardData cardData;

    [SerializeField]
    private CardListControl cardListControl;

    private Image cardBG;

    public void SetCard(CardData cardData)
    {
        this.cardData = cardData;


        cardBG = GetComponent<Image>();

        Texture2D oldTexture = cardBG.sprite.texture;

        if(cardData == null)
        {
            cardBG.sprite = InventoryManager.NoCardSprite;
            HasCard = false;
        }
        else
        {
            HasCard = true;
        }
        try
        {
            //Replace main Colors

            Color mainColor = Color.black;

            Texture2D newTexture = new Texture2D(oldTexture.width, oldTexture.height);
            newTexture.filterMode = FilterMode.Point;

            int y = 0;
            while (y < newTexture.height)
            {
                int x = 0;
                while (x < newTexture.width)
                {
                    if (oldTexture.GetPixel(x, y) == InventoryManager.replaceColorMain)
                    {
                        newTexture.SetPixel(x, y, InventoryManager.GetCardColorDark(cardData.color));
                    }
                    else if (oldTexture.GetPixel(x, y) == InventoryManager.replaceColorLeft)
                    {
                        newTexture.SetPixel(x, y, InventoryManager.GetCardColorBright(cardData.colorLeft));
                    }
                    else if (oldTexture.GetPixel(x, y) == InventoryManager.replaceColorRight)
                    {
                        newTexture.SetPixel(x, y, InventoryManager.GetCardColorBright(cardData.colorRight));
                    }
                    else
                    {
                        newTexture.SetPixel(x, y, oldTexture.GetPixel(x, y));
                    }
                    ++x;
                }
                ++y;
            }


            newTexture.Apply();

            cardBG.sprite = Sprite.Create(newTexture, cardBG.sprite.rect, Vector2.up);


            //ReplaceTriggerColor

            oldTexture = trigger.GetComponent<Image>().sprite.texture;


            newTexture = new Texture2D(oldTexture.width, oldTexture.height);
            newTexture.filterMode = FilterMode.Point;


            y = 0;
            while (y < newTexture.height)
            {
                int x = 0;
                while (x < newTexture.width)
                {
                    if (oldTexture.GetPixel(x, y) == InventoryManager.replaceColorMain)
                    {
                        newTexture.SetPixel(x, y, InventoryManager.GetCardColorBright(cardData.color));
                    }
                    else
                    {

                        newTexture.SetPixel(x, y, oldTexture.GetPixel(x, y));
                    }
                    ++x;
                }
                ++y;
            }

            newTexture.Apply();

            trigger.GetComponent<Image>().sprite = Sprite.Create(newTexture, trigger.GetComponent<Image>().sprite.rect, Vector2.up);


            //set number icons

            figure1.GetComponent<Image>().sprite = InventoryManager.GetNumber((cardData.level) % 10);
            figure10.GetComponent<Image>().sprite = InventoryManager.GetNumber((int)(cardData.level / 10));

            //set trigger position

            trigger.transform.localPosition = new Vector3((-28.5f + 57f * cardData.triggerPosition), trigger.transform.localPosition.y, 0);

            //set effect icons


        }
        catch { }
    }

    public void OnClick()
    {
        cardListControl.CardClicked(cardData);
    }
}
