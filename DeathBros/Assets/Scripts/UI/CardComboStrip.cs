using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardComboStrip : MonoBehaviour
{
    public Image sliderImage;
    public Color sliderColor;
    public Color sliderColorBlink;

    [SerializeField]
    protected SpriteFontText[] comboBuffTexts;

    private int counter = 0;

    private CardEffectManager cardEffectMng;

    private void Awake()
    {
        cardEffectMng = FindObjectOfType<CardEffectManager>();

        if(cardEffectMng == null)
        {
            Debug.Log("CardEffectManager not found!");
            return;
        }

        cardEffectMng.AUpdateComboCard += UpdateComboCardText;
        cardEffectMng.AUpdateComboCardStatus += UpdateComboCardStatus;
    }

    private void Start()
    {
        ModifySlider(GameManager.Player.ComboPower);

        GameManager.Player.AChangeComboPower += ModifySlider;

        sliderImage.color = sliderColor;
    }

    private void FixedUpdate()
    {
        if (GameManager.Player.ComboPower >= 100f)
        {
            counter++;

            if (counter >= 8)
            {
                sliderImage.color = sliderColorBlink;
            }
            else
            {
                sliderImage.color = sliderColor;
            }
        }
        else
        {
            sliderImage.color = sliderColor;
        }

        if (counter > 10)
        {
            counter = 0;
        }
    }

    private void ModifySlider(float value)
    {
        sliderImage.fillAmount = value / 110f;
        //sliderImage.sizeDelta = new Vector2(value / 110 * 352, sliderImage.sizeDelta.y);
    }

    private void UpdateComboCardText(int index, ComboCardDataSO comboCardData)
    {
        comboBuffTexts[index].SetText(comboCardData.buffText);
    }

    private void UpdateComboCardStatus(int index, bool enabled)
    {
        Color newColor;
        if(enabled == true)
        {
            newColor = Color.white;
        }
        else
        {
            newColor = Color.black;
        }

        comboBuffTexts[index].SetColor(newColor);
    }
}
