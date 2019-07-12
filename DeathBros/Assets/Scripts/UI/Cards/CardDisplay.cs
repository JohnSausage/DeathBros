using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [SerializeField]
    protected CardDataSO cardDataSO;

    [Space]

    [SerializeField]
    protected Image cardBG;

    [SerializeField]
    protected Image cardPicture;

    [SerializeField]
    protected SpriteFontText title;

    [SerializeField]
    protected SpriteFontText damage;

    [SerializeField]
    protected SpriteFontText cost;

    [SerializeField]
    protected SpriteFontText details;

    void Start()
    {
        SetCardData(cardDataSO);
    }

    public void SetCardData(CardDataSO cardDataSO)
    {
        if(cardDataSO == null)
        {
            return;
        }

        cardBG.color = CardManager.GetColor(cardDataSO.cardColor);

        cardPicture.sprite = cardDataSO.picture;

        title.SetText(cardDataSO.title);
        damage.SetText(cardDataSO.damage.ToString() + CardManager.GetEffectIcon(cardDataSO.element));
        cost.SetText(cardDataSO.cost.ToString() + "\\4");
        details.SetText(cardDataSO.details);
    }
}
