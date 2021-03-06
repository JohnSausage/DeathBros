﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SpriteFontText : MonoBehaviour
{
    [SerializeField]
    [TextArea(3, 10)]
    protected string text;

    public Color color = Color.black;

    [Space]

    public SpriteFont spriteFont;

    [Space]

    public int spacingX;
    public int spacingY = 8;
    public int lineWidth = 256;

    public bool alignLeft;
    public bool alignCenter;
    public bool alignRight;
    public bool alignMiddle;

    [Space]

    public bool enableIconColor = false;

    [Space]
    public bool updateText;



    protected int textWidth;

    protected const int iconWidth = 6;

    private void Awake()
    {

    }

    private void Update()
    {
        if (text == null)
        {
            return;
        }

        if (updateText == true)
        {
            updateText = false;

            Image[] images = GetComponentsInChildren<Image>();

            for (int i = 0; i < images.Length; i++)
            {
                DestroyImmediate(images[i].gameObject);
            }


            textWidth = 0;

            bool skipIconWidth = false;

            foreach (char c in text)
            {
                if (skipIconWidth == true)
                {
                    skipIconWidth = false;
                }
                else
                {
                    textWidth += spriteFont.GetWidth(c) * 2;
                }

                if (c == 92)
                {
                    textWidth += iconWidth * 2;
                    skipIconWidth = true;
                }
            }


            int positionX = 0;

            if (alignRight)
            {
                positionX = -spacingX * 2;
            }
            if (alignLeft)
            {
                positionX = spacingX * 2;
            }
            if (alignCenter)
            {
                positionX = (int)(-textWidth / 2f);
            }

            int lineNumber = 0;

            bool iconMode = false;

            foreach (char c in text)
            {
                if (c == 92)
                {
                    iconMode = true;
                    continue;
                }

                GameObject newGO = new GameObject();
                newGO.name = spriteFont.name + "_" + c;
                newGO.transform.SetParent(transform);
                Image newImage = newGO.AddComponent<Image>();
                int letterWidth = 0;

                if (iconMode)
                {
                    newImage.sprite = spriteFont.GetIcon(c);
                    iconMode = false;
                    letterWidth = iconWidth;

                    if(enableIconColor == true)
                    {
                        newImage.color = color;
                    }
                }
                else
                {
                    newImage.sprite = spriteFont.GetSprite(c);
                    newImage.color = color;
                    letterWidth = spriteFont.GetWidth(c);
                }


                if (newImage.sprite == null)
                {
                    newImage.enabled = false;
                }

                newImage.SetNativeSize();


                newGO.transform.localScale = new Vector3(1, 1, 1);

                RectTransform rectTransform = newImage.GetComponent<RectTransform>();

                if (alignRight)
                {
                    newGO.transform.localPosition = new Vector3(positionX - textWidth, (lineNumber * -spacingY) - 1, 0);
                    rectTransform.anchorMin = new Vector2(1, 0.5f);
                    rectTransform.anchorMax = new Vector2(1, 0.5f);
                }
                if (alignLeft)
                {
                    newGO.transform.localPosition = new Vector3(positionX, (lineNumber * -spacingY) - 1, 0);
                    rectTransform.anchorMin = new Vector2(0, 0.5f);
                    rectTransform.anchorMax = new Vector2(0, 0.5f);
                }
                if (alignCenter)
                {
                    newGO.transform.localPosition = new Vector3(positionX, (lineNumber * -spacingY) - 1, 0);
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                }

                positionX += letterWidth * 2;

                if (positionX >= lineWidth || c == 10)
                {
                    lineNumber++;

                    positionX = 0;

                    if (alignRight)
                    {
                        positionX = -spacingX * 2;
                    }
                    if (alignLeft)
                    {
                        positionX = spacingX * 2;
                    }
                    if (alignCenter)
                    {
                        positionX = (int)(-textWidth / 2f);
                    }
                }


                rectTransform.pivot = new Vector2(0, 0.5f);
            }
        }

        if (alignRight)
        {

        }
    }

    public void SetText(string text)
    {
        this.text = text;
        updateText = true;
    }

    public void SetColor(Color color)
    {
        this.color = color;
        updateText = true;
    }
}
