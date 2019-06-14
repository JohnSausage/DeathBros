using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPanel : MonoBehaviour
{

    public RectTransform sliderImage;

    protected float healthPercent;
    protected float sizeY;

    void Start()
    {
        sizeY = sliderImage.sizeDelta.y;
    }

    void FixedUpdate()
    {
        healthPercent = GameManager.Player.HealthPercent;

        sliderImage.sizeDelta = new Vector2(sliderImage.sizeDelta.x, sizeY * healthPercent);
    }
}
