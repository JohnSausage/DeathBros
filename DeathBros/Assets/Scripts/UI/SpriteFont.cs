using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SpriteFont")]
public class SpriteFont : ScriptableObject
{
    public List<SpriteFontCharacter> chars;

    public List<Sprite> icons;

    public List<Sprite> addSprites;

    public List<Sprite> Sprites(string text)
    {
        List<Sprite> sprites = new List<Sprite>();

        foreach (char character in text)
        {
            SpriteFontCharacter spriteFontCharacter = chars.Find(x => x.character == character);

            sprites.Add(spriteFontCharacter.sprite);
        }

        return sprites;
    }

    public Sprite GetIcon(char c)
    {
        int iconIndex = (int)char.GetNumericValue(c);

        if(iconIndex >= icons.Count)
        {
            return null;
        }

        return icons[iconIndex];
    }

    public Sprite GetSprite(char c)
    {
        SpriteFontCharacter sfc = chars.Find(x => x.character == c);

        if (sfc == null)
        {
            return null;
        }

        return sfc.sprite;
    }


    public int GetWidth(char c)
    {
        SpriteFontCharacter sfc = chars.Find(x => x.character == c);

        if(sfc == null)
        {
            return 0;
        }

        return sfc.width;
    }
}

[System.Serializable]
public class SpriteFontCharacter
{
    public char character;
    public Sprite sprite;
    public int width;

    public SpriteFontCharacter() { }

    public SpriteFontCharacter(char character)
    {
        this.character = character;
    }

    public SpriteFontCharacter(char character, Sprite sprite)
    {
        this.character = character;
        this.sprite = sprite;
    }
}