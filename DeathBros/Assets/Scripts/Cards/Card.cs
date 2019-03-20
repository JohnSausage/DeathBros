using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ECardColor { Red, Blue, Green }

[System.Serializable]
public class Card : MonoBehaviour
{
    public int level;
    public ECardColor color;
    public ECardColor colorLeft;
    public ECardColor colorRight;
    public float triggerPosition;

    public CardEffect cardEffect;

    [SerializeField]
    private Image cardBG;

    private void Start()
    {
        cardBG = GetComponent<Image>();

        Color replaceColor = new Color((200f / 255f), (50f / 255f), (200f / 255f), 1f);

        Texture2D oldTexture = cardBG.sprite.texture;

        Texture2D newTexture = new Texture2D(oldTexture.width, oldTexture.height);
        newTexture.filterMode = FilterMode.Point;

        int y = 0;
        while (y < newTexture.height)
        {
            int x = 0;
            while (x < newTexture.width)
            {
                if (oldTexture.GetPixel(x, y) == replaceColor)
                {
                    newTexture.SetPixel(x, y, Color.red);
                }
                else
                {
                    newTexture.SetPixel(x, y, oldTexture.GetPixel(x, y));
                }
                ++x;
            }
            ++y;
        }


        newTexture.Apply();

        cardBG.sprite = Sprite.Create(newTexture, cardBG.sprite.rect, Vector2.up);
    }


    public void ApplyEffect(Player player)
    {
        cardEffect.ApplyEffect(player);
    }
}

[System.Serializable]
public class CardEffect
{
    public virtual void ApplyEffect(Player player) { }
}

[System.Serializable]
public class CardEffect_SingleAttackStrength : CardEffect
{
    public EAttackType attackType;
    public float damageMultiplier;

    public override void ApplyEffect(Player player)
    {
        base.ApplyEffect(player);

        player.cardEffects.Add(this);
    }
}

public enum EAttackClass { Tilts, Aerials, Strongs, Specials, Throws }

[System.Serializable]
public class CardEffect_AllAttackStrength : CardEffect
{
    public EAttackClass attackClass;
    public float damageMultiplier;

    public override void ApplyEffect(Player player)
    {
        base.ApplyEffect(player);


    }
}