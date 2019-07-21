using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenWall : MonoBehaviour
{
    protected List<SpriteRenderer> tiles;

    private void Start()
    {
        tiles = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                StartCoroutine(CTurnInvisible(tiles[i]));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                StartCoroutine(CTurnVisible(tiles[i]));
            }
        }
    }


    private IEnumerator CTurnInvisible(SpriteRenderer spr)
    {
        int steps = 15;

        for (int i = 0; i < steps; i++)
        {
            float a = (float)((float)(steps - i) / (float)steps);

            SetTransparency(spr, a);
            yield return null;
        }

        SetTransparency(spr, 0f);
    }

    private IEnumerator CTurnVisible(SpriteRenderer spr)
    {
        int steps = 15;

        for (int i = 0; i < steps; i++)
        {
            float a = (float)((float)(i + 1) / (float)steps);

            SetTransparency(spr, a);
            yield return null;
        }

        SetTransparency(spr, 1f);
    }

    private void SetTransparency(SpriteRenderer spr, float a)
    {
        spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, a);
    }
}
