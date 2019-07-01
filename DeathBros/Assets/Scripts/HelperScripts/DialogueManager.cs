using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : _MB
{
    [SerializeField]
    protected GameObject dialoguePanel;
    protected SpriteFontText text;

    public static DialogueManager Instance { get; protected set; }

    public override void Init()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        base.Init();

        text = dialoguePanel.GetComponentInChildren<SpriteFontText>();

        dialoguePanel.SetActive(false);
    }

    public static void QueueDialogue(string dialogueText)
    {
        Instance.StartCoroutine(Instance.DisplayTextCoroutine(dialogueText));
    }

    protected IEnumerator DisplayTextCoroutine(string text)
    {
        this.text.SetText(text);

        dialoguePanel.SetActive(true);

        yield return new WaitForSeconds(3f);

        dialoguePanel.SetActive(false);
    }
}
