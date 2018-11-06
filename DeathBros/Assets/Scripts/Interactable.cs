using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour, ICanInteract
{
    public FrameAnimator anim { get; protected set; }

    private void Start()
    {
        anim = GetComponent<FrameAnimator>();

        anim.AnimationOver += Remove;
    }

    protected void Die()
    {
        anim.ChangeAnimation("die");
    }


    protected void Remove(FrameAnimation die)
    {
        if (die == anim.GetAnimation("die"))
            Destroy(gameObject);
    }

    public void StartInteraction(Character chr)
    {
        Debug.Log("ineract with " + transform.name);
    }
}

public interface ICanInteract
{
    void StartInteraction(Character chr);
}