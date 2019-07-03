using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : _MB
{
    [SerializeField]
    protected GameObject dialoguePanel;

    [SerializeField]
    protected SpriteFontText text1;

    [SerializeField]
    protected SpriteFontText text2;

    public static DialogueManager Instance { get; protected set; }

    public Queue<Dialogue> dialogueBits;

    protected IDialogueStarter currentDialogueStarter;

    protected bool playerConfirm;
    protected bool waitForCoonfirm;

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

        dialoguePanel.SetActive(false);
    }

    protected void FixedUpdate()
    {
        playerConfirm = false;

        if(GameManager.Player.Interact == true)
        {
            playerConfirm = true;
        }

        if(waitForCoonfirm == true)
        {
            if(playerConfirm == true)
            {
                waitForCoonfirm = false;
                playerConfirm = false;

                if(currentDialogueStarter != null)
                {
                    currentDialogueStarter.EndConversation();
                }

                dialoguePanel.SetActive(false);
            }
        }
    }

    public static void DisplayComment(Dialogue dialogue)
    {
        Instance.StartCoroutine(Instance.DisplayTextCoroutine(dialogue));
        Instance.StartCoroutine(Instance.TurnTextOffAfterTime(2f));
    }

    protected IEnumerator DisplayTextCoroutine(Dialogue dialogue)
    {
        text1.color = dialogue.color;
        text2.color = dialogue.color;


        text1.SetText("");
        text2.SetText("");


        dialoguePanel.SetActive(true);

        string line = "";

        foreach(char c in dialogue.line1)
        {
            line += c;
            text1.SetText(line);
            text2.SetText("");
            yield return null;
            yield return null;

            if(playerConfirm == true)
            {
                playerConfirm = false;
                break;
            }
        }

        text1.SetText(dialogue.line1);

        if (dialogue.line2 == "")
        {
            waitForCoonfirm = true;
        }
        else
        {
            StartCoroutine(DisplaySecondLine(dialogue));
        }
    }

    protected IEnumerator DisplaySecondLine(Dialogue dialogue)
    {
        string line = "";

        foreach (char c in dialogue.line2)
        {
            line += c;
            text2.SetText(line);
            yield return null;
            yield return null;

            if (playerConfirm == true)
            {
                playerConfirm = false;
                break;
            }
        }

        text2.SetText(dialogue.line2);

        waitForCoonfirm = true;
    }

    protected IEnumerator TurnTextOffAfterTime(float timeS)
    {
        yield return new WaitForSeconds(timeS);
        dialoguePanel.SetActive(false);
    }


    public static void StartDialogue(IDialogueStarter dialogueStarter, Dialogue dialogue)
    {
        Instance.currentDialogueStarter = dialogueStarter;
        dialogueStarter.StartConversation();

        Instance.dialoguePanel.SetActive(true);

        Instance.StartCoroutine(Instance.DisplayTextCoroutine(dialogue));
    }
}

[System.Serializable]
public class Dialogue
{
    [TextArea(1,10)]
    public string line1;

    [TextArea(1, 10)]
    public string line2;

    public Color color = Color.black;

    public Dialogue(string line1, string line2 = "")
    {
        this.line1 = line1;
        this.line2 = line2;
    }
}

public enum EDialogueAction { None, Talking, Laughing, Agreeing, Disagreeing };

public interface IDialogueStarter
{
    FrameAnimator GetFanim();
    string GetActionAnim(EDialogueAction dialogueAction);
    void StartConversation();
    void EndConversation();
}