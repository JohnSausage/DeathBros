using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardComboStrip : MonoBehaviour
{
    public RectTransform sliderImage;
    public Color sliderColor;
    public Color sliderColorBlink;

    private Image image;
    private int counter = 0;

    private void Start()
    {
        image = sliderImage.GetComponent<Image>();
        image.color = sliderColor;

        ModifySlider(GameManager.Player.ComboPower);

        GameManager.Player.AChangeComboPower += ModifySlider;
    }

    private void FixedUpdate()
    {


        if (GameManager.Player.ComboPower >= 100f)
        {
            counter++;

            if (counter >= 8)
            {
                image.color = sliderColorBlink;
            }
            else
            {
                image.color = sliderColor;
            }
        }
        else
        {
            image.color = sliderColor;
        }

        if (counter > 10)
        {
            counter = 0;
        }
    }

    private void ModifySlider(float value)
    {
        sliderImage.sizeDelta = new Vector2(value / 110 * 352, sliderImage.sizeDelta.y);
    }
}
