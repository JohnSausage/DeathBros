using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SpriteFontText : MonoBehaviour
{
    public string text;
    public int spaceLeft;

    [Space]

    public SpriteFont spriteFont;

    [Space]

    public bool updateText;


    void Awake()
    {

    }

    void Update()
    {
        if(updateText == true)
        {
            updateText = false;

            Image[] images = GetComponentsInChildren<Image>();

            for (int i = 0; i < images.Length; i++)
            {
                DestroyImmediate(images[i].gameObject);
            }

            int positionX = spaceLeft * 2;

            foreach(char c in text)
            {
                GameObject newGO = new GameObject();
                newGO.name = spriteFont.name + "_" + c;
                newGO.transform.SetParent(transform);

                Image newImage = newGO.AddComponent<Image>();
                newImage.sprite = spriteFont.GetSprite(c);
                newImage.SetNativeSize();

                newGO.transform.localPosition = new Vector3(positionX, 0, 0);
                newGO.transform.localScale = new Vector3(1, 1, 1);
                positionX += spriteFont.GetWidth(c) * 2;

                RectTransform rectTransform = newImage.GetComponent<RectTransform>();

                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                rectTransform.pivot = new Vector2(0, 1);
            }        
        }
    }
}
