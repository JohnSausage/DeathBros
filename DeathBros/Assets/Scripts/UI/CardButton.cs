//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;

//public class CardButton : MonoBehaviour, ISelectHandler
//{
//    public SpriteFont cardEffectFont;
//    public GameObject spriteFontCharacterPrefab;
//    private List<GameObject> spriteFontCharacters;

//    public bool HasCard { get; set; }

//    [SerializeField]
//    private GameObject figure10, figure1, trigger;

//    [SerializeField]
//    public CardData cardData;


//    [SerializeField]
//    private CardListControl cardListControl;

//    [SerializeField]
//    private CardPanel cardPanel;

//    private Image cardBG;

//    private void Awake()
//    {
//        cardBG = GetComponent<Image>();

//        spriteFontCharacters = new List<GameObject>();
//    }

//    public void OnSelect(BaseEventData eventData)
//    {
//        if (HasCard)
//            cardPanel.SelectCard(cardData);

//        cardListControl.ZoomToCard(gameObject);
//    }

//    public void SetCard(CardData cardData)
//    {
//        this.cardData = cardData;

//        cardBG = GetComponent<Image>();

//        cardBG.sprite = InventoryManager.NoCardSprite;
//        cardBG.sprite = InventoryManager.NoCardSprite;
//        HasCard = false;

//        figure1.SetActive(false);
//        figure10.SetActive(false);
//        trigger.SetActive(false);

//        for (int j = 0; j < spriteFontCharacters.Count; j++)
//        {
//            Destroy(spriteFontCharacters[j].gameObject);
//        }

//        spriteFontCharacters.Clear();


//        if (cardData == null) return;
//        if (cardData.level == 0) return;


//        cardBG.sprite = InventoryManager.BlankCardSprite;
//        HasCard = true;

//        figure1.SetActive(true);
//        figure10.SetActive(true);
//        trigger.SetActive(true);



//        try
//        {
//            Texture2D oldTexture = cardBG.sprite.texture;
//            //Replace main Colors

//            Color mainColor = Color.black;

//            Texture2D newTexture = new Texture2D(oldTexture.width, oldTexture.height);
//            newTexture.filterMode = FilterMode.Point;

//            int y = 0;
//            while (y < newTexture.height)
//            {
//                int x = 0;
//                while (x < newTexture.width)
//                {
//                    if (oldTexture.GetPixel(x, y) == InventoryManager.replaceColorMain)
//                    {
//                        newTexture.SetPixel(x, y, InventoryManager.GetCardColorDark(cardData.color));
//                    }
//                    else if (oldTexture.GetPixel(x, y) == InventoryManager.replaceColorLeft)
//                    {
//                        newTexture.SetPixel(x, y, InventoryManager.GetCardColorBright(cardData.colorLeft));
//                    }
//                    else if (oldTexture.GetPixel(x, y) == InventoryManager.replaceColorRight)
//                    {
//                        newTexture.SetPixel(x, y, InventoryManager.GetCardColorBright(cardData.colorRight));
//                    }
//                    else
//                    {
//                        newTexture.SetPixel(x, y, oldTexture.GetPixel(x, y));
//                    }
//                    ++x;
//                }
//                ++y;
//            }


//            newTexture.Apply();

//            cardBG.sprite = Sprite.Create(newTexture, cardBG.sprite.rect, Vector2.up);


//            //ReplaceTriggerColor

//            oldTexture = trigger.GetComponent<Image>().sprite.texture;


//            newTexture = new Texture2D(oldTexture.width, oldTexture.height);
//            newTexture.filterMode = FilterMode.Point;


//            y = 0;
//            while (y < newTexture.height)
//            {
//                int x = 0;
//                while (x < newTexture.width)
//                {
//                    if (oldTexture.GetPixel(x, y) == InventoryManager.replaceColorMain)
//                    {
//                        newTexture.SetPixel(x, y, InventoryManager.GetCardColorBright(cardData.color));
//                    }
//                    else
//                    {

//                        newTexture.SetPixel(x, y, oldTexture.GetPixel(x, y));
//                    }
//                    ++x;
//                }
//                ++y;
//            }

//            newTexture.Apply();

//            trigger.GetComponent<Image>().sprite = Sprite.Create(newTexture, trigger.GetComponent<Image>().sprite.rect, Vector2.up);


//            //set number icons

//            figure1.GetComponent<Image>().sprite = InventoryManager.GetNumber((cardData.level) % 10);
//            figure10.GetComponent<Image>().sprite = InventoryManager.GetNumber((int)(cardData.level / 10));

//            //set trigger position

//            trigger.transform.localPosition = new Vector3((-28.5f + 57f * cardData.triggerPosition), trigger.transform.localPosition.y, 0);

//            //set effect icons



//            string text = cardData.cardEffect.GetEffectText();

//            List<Sprite> sprites = cardEffectFont.Sprites(text);

//            int i = 0;
//            foreach (Sprite sprite in sprites)
//            {
//                GameObject spriteChar = Instantiate(spriteFontCharacterPrefab, transform);
//                spriteChar.GetComponent<Image>().sprite = sprite;
//                spriteChar.transform.localPosition = new Vector3(spriteChar.transform.localPosition.x + i * 20, spriteChar.transform.localPosition.y, 0);
//                i++;

//                spriteFontCharacters.Add(spriteChar);
//            }
//        }
//        catch { }
//    }
//}
