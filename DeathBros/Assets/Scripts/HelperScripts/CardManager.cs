using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
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


    }
}

[System.Serializable]
public class CardData
{
    public string title;

}