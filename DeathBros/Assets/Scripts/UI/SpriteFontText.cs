using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SpriteFontText : MonoBehaviour
{
    [SerializeField]
    [TextArea(3,10)]
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

    public bool updateText;



    protected int textWidth;


    void Awake()
    {

    }

    void Update()
    {
        if (updateText == true)
        {
            updateText = false;

            Image[] images = GetComponentsInChildren<Image>();

            for (int i = 0; i < images.Length; i++)
            {
                DestroyImmediate(images[i].gameObject);
            }


            textWidth = 0;

            foreach (char c in text)
            {
                textWidth += spriteFont.GetWidth(c) * 2;
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

            foreach (char c in text)
            {
                GameObject newGO = new GameObject();
                newGO.name = spriteFont.name + "_" + c;
                newGO.transform.SetParent(transform);

                Image newImage = newGO.AddComponent<Image>();
                newImage.sprite = spriteFont.GetSprite(c);
                if(newImage.sprite == null)
                {
                    newImage.enabled = false;
                }
                newImage.SetNativeSize();
                newImage.color = color;

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

                positionX += spriteFont.GetWidth(c) * 2;

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
}
