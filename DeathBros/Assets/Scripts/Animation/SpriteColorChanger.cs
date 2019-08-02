using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColorChanger : MonoBehaviour
{
    [SerializeField]
    protected Color color1;

    void Start()
    {

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
                //INSERT YOUR LOGIC HERE
                if (soruceTexture.GetPixel(x, y) == Color.green)
                {
                    //This line of code and if statement, turn Green pixels into Red pixels.
                    texture.SetPixel(x, y, color1);
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
