using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardComboStrip : MonoBehaviour
{
    public Image sliderImage;
    public Color sliderColor;
    public Color sliderColorBlink;

    private int counter = 0;

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
}
