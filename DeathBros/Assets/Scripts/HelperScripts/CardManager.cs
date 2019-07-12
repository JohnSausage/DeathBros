using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField]
    protected Color cardColorRed;

    [SerializeField]
    protected Color cardColorBlue;

    [SerializeField]
    protected Color cardColorGreen;

    #region Singelton
    public static CardManager Instance { get; protected set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public List<CardDataSO> cardData;

    void Start()
    {
        Object[] objects = Resources.LoadAll("CardData/SkillCards");

        foreach (Object obj in objects)
        {
            cardData.Add((CardDataSO)obj);
        }
    }

    public static Color GetColor(ECardColor cardColor)
    {
        Color retVal = Color.white;

        switch(cardColor)
        {
            case ECardColor.Red:
                {
                    retVal = Instance.cardColorRed;
                    break;
                }

            case ECardColor.Blue:
                {
                    retVal = Instance.cardColorBlue;
                    break;
                }

            case ECardColor.Green:
                {
                    retVal = Instance.cardColorGreen;
                    break;
                }

            default:
                {
                    break;
                }
        }

        return retVal;
    }

    public static string GetEffectIcon(EStatusEffectType effectType)
    {
        string retVal = "";

        switch(effectType)
        {
            case EStatusEffectType.Burning:
                {
                    retVal = "\\1";
                    break;
                }

            case EStatusEffectType.Wet:
                {
                    retVal = "\\0";
                    break;
                }

            default:
                {
                    break;
                }
        }

        return retVal;
    }
}

[System.Serializable]
public class CardData
{
    public string title;

}