using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SpriteFont")]
public class SpriteFont : ScriptableObject
{
    public List<SpriteFontCharacter> chars;

    public List<Sprite> Sprites(string text)
    {
        List<Sprite> sprites = new List<Sprite>();

        foreach (char character in text)
        {
            SpriteFontCharacter spriteFontCharacter = chars.Find(x => x.cahracter == character);

            sprites.Add(spriteFontCharacter.sprite);
        }

        return sprites;
    }
}

[System.Serializable]
public class SpriteFontCharacter
{
    public char cahracter;
    public Sprite sprite;
}