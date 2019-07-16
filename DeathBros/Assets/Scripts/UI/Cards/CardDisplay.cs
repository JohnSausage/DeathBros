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

    protected bool selected;
    protected Vector2 originalLocalPosition;
    protected Vector2 offsetOnSelect;

    private void Awake()
    {
        originalLocalPosition = transform.localPosition;
        offsetOnSelect = Vector2.zero;
    }

    void Start()
    {
        SetCardData(cardDataSO);
    }

    public void SetCardData(CardDataSO cardDataSO)
    {
        if (cardDataSO == null)
        {
            return;
        }

        this.cardDataSO = cardDataSO;

        cardBG.color = CardManager.GetColor(cardDataSO.cardColor);

        cardPicture.sprite = cardDataSO.picture;

        title.SetText(cardDataSO.title);
        damage.SetText(cardDataSO.damage.ToString() + CardManager.GetEffectIcon(cardDataSO.element));
        cost.SetText(cardDataSO.cost.ToString() + "\\4");
        details.SetText(cardDataSO.details);
    }

    public void MoveToPositionOnSelect()
    {
        if (selected == true)
        {
            return;
        }

        selected = true;
        originalLocalPosition = transform.localPosition;
        //transform.localPosition += (Vector3)offsetOnSelect;
        StartCoroutine(CMoveToOffset());
    }

    public void Deselect()
    {
        if (selected == false)
        {
            return;
        }

        selected = false;
        transform.localPosition = originalLocalPosition;
    }

    public void SetOffsetOnSelect(Vector2 offset)
    {
        offsetOnSelect = offset;
    }

    protected IEnumerator CMoveToOffset()
    {
        for (int i = 0; i < 10; i++)
        {
            transform.Translate(offsetOnSelect / 10f);
            yield return null;
        }
    }
}
