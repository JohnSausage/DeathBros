using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Effect : MonoBehaviour
{
    public Color color { set { spr.color = value; } }

    [SerializeField]
    private bool destroyAfterAnimation = true;

    [SerializeField]
    private int frameDuration;

    [SerializeField]
    private List<Sprite> sprites;

    private SpriteRenderer spr;
    private float timer = 0;
    private int counter = 0;

    void Awake()
    {
        spr = GetComponent<SpriteRenderer>();

        if (sprites.Count > 0) spr.sprite = sprites[counter];
    }

    private void Update()
    {
        timer += Time.unscaledDeltaTime;

        if (timer >= frameDuration / 60)
        {
            counter++;

            if (sprites.Count > counter)
            {
                spr.sprite = sprites[counter];
                timer = 0;
            }
            else
            {
                if (destroyAfterAnimation)
                    Destroy(gameObject);
                else
                    counter = 0;
            }
        }
    }
}
