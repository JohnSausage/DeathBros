using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColorChanger : MonoBehaviour
{
    [SerializeField]
    protected Color color1;

    [SerializeField]
    protected Color color2;

    [Space]

    [SerializeField]
    protected int randomR = 0;

    [SerializeField]
    protected int randomB = 0;

    [SerializeField]
    protected int randomG = 0;

    public Color Color1 { set { color1 = value; } }


    protected static Color repColor1 = Color.green;
    protected static Color repColor2 = new Color(0f, 130f / 255f, 0f, 1f);

    private void Awake()
    {
        //Randomize colors
        int r1 = (int)(color1.r * 255) + Random.Range(-randomR, randomR + 1);
        int g1 = (int)(color1.g * 255) + Random.Range(-randomG, randomG + 1);
        int b1 = (int)(color1.b * 255) + Random.Range(-randomB, randomB + 1);

        int r2 = (int)(color2.r * 255) + Random.Range(-randomR, randomR + 1);
        int g2 = (int)(color2.g * 255) + Random.Range(-randomG, randomG + 1);
        int b2 = (int)(color2.b * 255) + Random.Range(-randomB, randomB + 1);

        r1 = Mathf.Clamp(r1, 0, 255);
        g1 = Mathf.Clamp(g1, 0, 255);
        b1 = Mathf.Clamp(b1, 0, 255);

        r2 = Mathf.Clamp(r2, 0, 255);
        g2 = Mathf.Clamp(g2, 0, 255);
        b2 = Mathf.Clamp(b2, 0, 255);

        color1 = new Color(r1 / 255f, g1 / 255f, b1 / 255f);
        color2 = new Color(r2 / 255f, g2 / 255f, b2 / 255f);
    }


    public Texture2D GetColoredSprite(Texture2D soruceTexture)
    {
        //Create a new Texture2D, which will be the copy.
        Texture2D texture = new Texture2D(soruceTexture.width, soruceTexture.height);

        //Choose your filtermode and wrapmode here.
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        int y = 0;

        while (y < texture.height)
        {
            int x = 0;
            while (x < texture.width)
            {
                if (soruceTexture.GetPixel(x, y) == repColor1)
                {
                    texture.SetPixel(x, y, color1);
                }
                else if(soruceTexture.GetPixel(x, y) == repColor2)
                {
                    texture.SetPixel(x, y, color2);
                }

                else
                {
                    //This line of code is REQUIRED. Do NOT delete it. This is what copies the image as it was, without any change.
                    texture.SetPixel(x, y, soruceTexture.GetPixel(x, y));
                }
                ++x;
            }
            ++y;
        }

        texture.name = soruceTexture.name + "_colored";

        //This finalizes it. If you want to edit it still, do it before you finish with .Apply(). Do NOT expect to edit the image after you have applied. It did NOT work for me to edit it after this function.
        texture.Apply();

        //Return the variable, so you have it to assign to a permanent variable and so you can use it.
        return texture;
    }
}
