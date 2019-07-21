using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Switch : MonoBehaviour, ICanInteract, ITrigger
{
    [SerializeField]
    protected List<Sprite> sprites;

    [SerializeField]
    protected bool state;

    protected SpriteRenderer spr;

    public event Action ATriggered;

    private void Start()
    {
        spr = GetComponent<SpriteRenderer>();

        if(sprites.Count < 3)
        {
            return;
        }

        if(state == true)
        {
            spr.sprite = sprites[2];
        }
        else
        {
            spr.sprite = sprites[0];
        }
    }

    public void StartInteraction(Character chr)
    {
        if(state == false)
        {
            StartCoroutine(CSwitchOn());
        }
        else
        {
            StartCoroutine(CSwitchOff());
        }
    }

    public bool TriggerOn()
    {
        return state;
    }

    protected IEnumerator CSwitchOn()
    {
        if (sprites.Count >= 3)
        {

            spr.sprite = sprites[1];
            yield return new WaitForSeconds(5f / 60f);

            spr.sprite = sprites[2];
        }

        state = true;

        if(ATriggered != null)
        {
            ATriggered();
        }

        yield return null;
    }

    protected IEnumerator CSwitchOff()
    {
        if (sprites.Count >= 3)
        {
            spr.sprite = sprites[1];
            yield return new WaitForSeconds(5f / 60f);

            spr.sprite = sprites[0];
        }

        state = false;

        if (ATriggered != null)
        {
            ATriggered();
        }

        yield return null;
    }
}
