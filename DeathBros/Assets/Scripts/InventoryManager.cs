using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    private Sprite noCardSprite;
    public static Sprite NoCardSprite { get { return instance.noCardSprite; } }

    [SerializeField]
    private Sprite blankCardSprite;
    public static Sprite BlankCardSprite { get { return instance.blankCardSprite; } }

    public static Color replaceColorMain = new Color((200f / 255f), (50f / 255f), (200f / 255f), 1f);
    public static Color replaceColorLeft = new Color((50f / 255f), (200f / 255f), (200f / 255f), 1f);
    public static Color replaceColorRight = new Color((200f / 255f), (200f / 255f), (50f / 255f), 1f);

    public static Color RedDark = new Color((100f / 255f), (30f / 255f), (30f / 255f), 1f);
    public static Color RedBright = new Color((240f / 255f), (70f / 255f), (0f / 255f), 1f);

    public static Color BlueDark = new Color((30f / 255f), (30f / 255f), (100f / 255f), 1f);
    public static Color BlueBright = new Color((30f / 255f), (90f / 255f), (240f / 255f), 1f);

    public static Color GreenDark = new Color((30f / 255f), (80f / 255f), (60f / 255f), 1f);
    public static Color GreenBright = new Color((70f / 255f), (180f / 255f), (70f / 255f), 1f);

    public List<Sprite> tarot_icons;

    #region Singelton
    private static InventoryManager instance;
    public static InventoryManager Instance { get { return instance; } }
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    public static List<CardData> cards;

    public static CardData[] comboCards;

    void Start()
    {
        cards = new List<CardData>();
        comboCards = new CardData[5];
    }

    void Update()
    {

    }



    static T GetRandomEnum<T>()
    {
        System.Array A = System.Enum.GetValues(typeof(T));
        T V = (T)A.GetValue(UnityEngine.Random.Range(0, A.Length));
        return V;
    }


    public static CardData CreateRandomCard()
    {
        CardData cardData = new CardData();

        cardData.color = GetRandomEnum<ECardColor>();
        cardData.colorLeft = ECardColor.Red;
        cardData.colorRight = ECardColor.Green;

        cardData.level = Random.Range(1, 21);

        cardData.triggerPosition = Random.Range(0f, 1f);

        cardData.cardEffect = new CardEffect();

        return cardData;
    }

    public static Sprite GetNumber(int number)
    {
        return (instance.tarot_icons[number]);
    }

    private Color RGB(int R, int G, int B)
    {
        return new Color((R / 255f), (G / 255f), (B / 255f), 1f);
    }

    public static Color GetCardColorDark(ECardColor color)
    {
        Color darkColor = Color.black;

        switch (color)
        {
            case ECardColor.Red:
                {
                    darkColor = RedDark;
                    break;
                }

            case ECardColor.Blue:
                {
                    darkColor = BlueDark;
                    break;
                }

            case ECardColor.Green:
                {
                    darkColor = GreenDark;
                    break;
                }

            default: break;
        }

        return darkColor;
    }

    public static Color GetCardColorBright(ECardColor color)
    {
        Color brightColor = Color.black;

        switch (color)
        {
            case ECardColor.Red:
                {
                    brightColor = RedBright;
                    break;
                }

            case ECardColor.Blue:
                {
                    brightColor = BlueBright;
                    break;
                }

            case ECardColor.Green:
                {
                    brightColor = GreenBright;
                    break;
                }

            default: break;
        }

        return brightColor;
    }
}