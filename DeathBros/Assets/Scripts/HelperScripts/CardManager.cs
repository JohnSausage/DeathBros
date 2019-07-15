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

    public List<CardDataSO> cardDataSOList;

    public List<CardDataSO> currentSkillsCardData { get; set; }

    void Start()
    {
        Object[] objects = Resources.LoadAll("CardData/SkillCards");

        foreach (Object obj in objects)
        {
            cardDataSOList.Add((CardDataSO)obj);
        }
        SortAscending sortAscending = new SortAscending();

        cardDataSOList.Sort(sortAscending);

        currentSkillsCardData = new List<CardDataSO>();

        if (GameManager.Instance.saveData.currentSkillIDs.Length < 4)
        {
            Debug.Log("Not enough current skills found!");
            return;
        }

        for (int i = 0; i < GameManager.Instance.saveData.currentSkillIDs.Length; i++)
        {
            currentSkillsCardData.Add(GetLoadedCardData(GameManager.Instance.saveData.currentSkillIDs[i]));
        }

        SetPlayerSkills();
    }

    public static void SaveSkillIDs()
    {
        for (int i = 0; i < Instance.currentSkillsCardData.Count; i++)
        {
            GameManager.Instance.saveData.currentSkillIDs[i] = Instance.currentSkillsCardData[i].cardID;
        }
    }

    public static void SetPlayerSkills()
    {
        Player player = FindObjectOfType<Player>();

        if (player == null)
        {
            Debug.Log("Player not found!");
            return;
        }

        if (Instance.currentSkillsCardData[0] != null)
        {
            player.SetSpecialAttack(Instance.currentSkillsCardData[0].attackStateSO, ESpecial.NEUTRAL);
        }

        if (Instance.currentSkillsCardData[1] != null)
        {
            player.SetSpecialAttack(Instance.currentSkillsCardData[1].attackStateSO, ESpecial.SIDE);
        }

        if (Instance.currentSkillsCardData[2] != null)
        {
            player.SetSpecialAttack(Instance.currentSkillsCardData[2].attackStateSO, ESpecial.UP);
        }

        if (Instance.currentSkillsCardData[3] != null)
        {
            player.SetSpecialAttack(Instance.currentSkillsCardData[3].attackStateSO, ESpecial.DOWN);
        }
    }

    public static CardDataSO GetLoadedCardData(int cardID)
    {
        CardDataSO retVal = null;

        retVal = Instance.cardDataSOList.Find(x => x.cardID == cardID);

        return retVal;
    }

    public static Color GetColor(ECardColor cardColor)
    {
        Color retVal = Color.white;

        switch (cardColor)
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

        switch (effectType)
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

class SortAscending : IComparer<CardDataSO>
{
    public int Compare(CardDataSO x, CardDataSO y)
    {
        if (x.cardID == 0 || y.cardID == 0)
        {
            return 0;
        }

        // CompareTo() method 
        return x.cardID.CompareTo(y.cardID);
    }
}