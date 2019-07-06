using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : _MB
{
    [SerializeField]
    protected GameObject dialoguePanel;

    [SerializeField]
    protected SpriteFontText text;



    public bool InDialogue { get; protected set; }


    protected Queue<DialogueLine> dialogueLineQueue;

    protected DialogueLine currentLine;

    protected IDialogueStarter currentDialogueStarter;

    protected bool isTypingLine;
    protected IEnumerator ILineTyper;
    protected IEnumerator IDialogueEnder;

    [SerializeField]
    protected FrameAnimatorUI fanimUI;


    public static DialogueManager Instance { get; protected set; }

    /// <summary>
    /// Initalization
    /// </summary>
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

        dialogueLineQueue = new Queue<DialogueLine>();

        GameManager.Player.APlayerInteract += OnPlayerConfirm;
    }



    /// <summary>
    /// Starts a dialogue with the IDialogueStarter
    /// </summary>
    /// <param name="dialogueStarter"></param>
    /// <param name="dialogue"></param>
    public static void StartDialogue(IDialogueStarter dialogueStarter, Dialogue dialogue)
    {
        if (Instance.InDialogue == true)
        {
            return;
        }


        Instance.currentDialogueStarter = dialogueStarter;
        dialogueStarter.StartConversation();

        Instance.TextSetup(dialogue);
    }



    /// <summary>
    /// Displays a dialogue that automatically disappears after 2 seconds
    /// </summary>
    /// <param name="dialogue"></param>
    public static void DisplayComment(IDialogueStarter dialogueStarter, Dialogue dialogue)
    {
        if (Instance.InDialogue == true)
        {
            return;
        }

        Instance.currentDialogueStarter = dialogueStarter;
        dialogueStarter.StartConversation();

        Instance.TextSetup(dialogue);

        Instance.IDialogueEnder = Instance.CTurnTextOffAfterTime(2f);
        Instance.StartCoroutine(Instance.IDialogueEnder);
    }

    /// <summary>
    /// Types out the line character by character
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    protected IEnumerator CTypeLine(string line)
    {
        isTypingLine = true;

        string displayedLine = "";

        foreach (char c in line)
        {
            displayedLine += c;
            text.SetText(displayedLine);
            yield return null;
            yield return null;

        }

        isTypingLine = false;
    }

    /// <summary>
    /// Stops typing and shows complete line
    /// </summary>
    protected void FinishLine()
    {
        StopCoroutine(ILineTyper);

        text.SetText(currentLine.line);
        isTypingLine = false;
    }


    /// <summary>
    /// Automatically ends the dialogue after the timeS
    /// </summary>
    /// <param name="timeS"></param>
    /// <returns></returns>
    protected IEnumerator CTurnTextOffAfterTime(float timeS)
    {
        yield return new WaitForSeconds(timeS);
        EndDialogue();
    }


    
    /// <summary>
    /// Sets text color, resets text and queues up the dialogue lines
    /// </summary>
    /// <param name="dialogue"></param>
    protected void TextSetup(Dialogue dialogue)
    {
        text.SetText("");

        dialoguePanel.SetActive(true);

        dialogueLineQueue.Clear();

        foreach (DialogueLine line in dialogue.dialogueLines)
        {
            dialogueLineQueue.Enqueue(line);
        }

      
        InDialogue = true;

        DisplayNextLine();
    }



    /// <summary>
    /// Queues the next line of the dialogue and ends the dialogue if this was the last line
    /// </summary>
    protected void DisplayNextLine()
    {
        if (dialogueLineQueue.Count == 0)
        {
            IDialogueEnder = CTurnTextOffAfterTime(0.1f);
            StartCoroutine(IDialogueEnder);
            return;
        }

        if (IDialogueEnder != null)
        {
            StopCoroutine(IDialogueEnder);
        }

        currentLine = dialogueLineQueue.Dequeue();

        text.SetText("");
        text.color = currentLine.color;

        if (currentDialogueStarter != null)
        {
            fanimUI.CopyFrameAnimator(currentDialogueStarter.GetFanim());
            fanimUI.ChangeAnimation(currentDialogueStarter.GetEmotionAnim(currentLine.dialogueEmotion));
        }

        ILineTyper = CTypeLine(currentLine.line);
        StartCoroutine(ILineTyper);
    }



    /// <summary>
    /// Disables the text and ends the dialogue
    /// </summary>
    protected void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        InDialogue = false;

        if (currentDialogueStarter != null)
        {
            currentDialogueStarter.EndConversation();
            currentDialogueStarter = null;
        }
    }



    /// <summary>
    /// Executed on Player Interact event
    /// </summary>
    protected void OnPlayerConfirm()
    {
        if (isTypingLine == true)
        {
            FinishLine();
            return;
        }

        if (InDialogue == true)
        {
            DisplayNextLine();
            return;
        }
    }
}



[System.Serializable]
public class Dialogue
{
    public DialogueLine[] dialogueLines;
}

[System.Serializable]
public class DialogueLine
{
    [TextArea(1, 10)]
    public string line;

    public Color color = Color.black;

    public EDialogueEmotion dialogueEmotion = EDialogueEmotion.Talking;

    public DialogueLine(string line)
    {
        this.line = line;
    }
}

public enum EDialogueEmotion { None, Talking, Laughing, Agreeing, Disagreeing };

public interface IDialogueStarter
{
    FrameAnimator GetFanim();
    string GetEmotionAnim(EDialogueEmotion dialogueEmotion);
    void StartConversation();
    void EndConversation();
}