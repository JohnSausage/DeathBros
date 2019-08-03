using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SimpleFrameAnimator : MonoBehaviour
{
    [SerializeField]
    protected List<Sprite> sprites;

    [SerializeField]
    protected int frameDuration = 5;


    protected SpriteRenderer spr;


    private int frameTimer = 0;
    private int spriteCounter = 0;

    private void Start()
    {
        spr = GetComponent<SpriteRenderer>();

        if(sprites.Count == 0)
        {
            return;
        }

        spr.sprite = sprites[spriteCounter];
    }

    private void FixedUpdate()
    {
        Animate();
    }

    protected void Animate()
    {
        if (sprites.Count == 0)
        {
            return;
        }

        frameTimer++;

        if (frameTimer >= frameDuration)
        {
            frameTimer = 0;

            spriteCounter++;

            if (spriteCounter >= sprites.Count)
            {
                spriteCounter = 0;
            }

            spr.sprite = sprites[spriteCounter];
        }
    }
}
