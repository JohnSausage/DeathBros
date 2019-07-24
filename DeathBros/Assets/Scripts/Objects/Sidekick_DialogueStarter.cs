using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sidekick_DialogueStarter : MonoBehaviour
{
    [SerializeField]
    protected Dialogue triggerdDialogue;

    protected bool triggered = false;

    protected SidekickController sidekick;

    private void Awake()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void Start()
    {
        sidekick = FindObjectOfType<SidekickController>();

        if (sidekick == null)
        {
            Debug.Log("Sidekick not found in DialogueStarter!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered == true)
        {
            return;
        }

        if (collision.transform.tag == "Player")
        {
            DialogueManager.StartDialogue(sidekick, triggerdDialogue);
            triggered = true;
        }
    }
}
