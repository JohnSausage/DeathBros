using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IDialogueStarter, ICanInteract
{
    [SerializeField]
    protected Dialogue[] dialogue;

    [SerializeField]
    protected string talkingAnim;

    [SerializeField]
    protected string agreeingAnim;

    [SerializeField]
    protected string disagreeingAnim;

    protected FrameAnimator fanim;
    protected bool isTalking;

    protected void Start()
    {
        fanim = GetComponent<FrameAnimator>();
    }

    public string GetActionAnim(EDialogueAction dialogueAction)
    {
        string retVal = "";

        switch (dialogueAction)
        {
            case EDialogueAction.Talking:
                {
                    retVal = talkingAnim;
                    break;
                }

            case EDialogueAction.Agreeing:
                {
                    retVal = agreeingAnim;
                    break;
                }

            case EDialogueAction.Disagreeing:
                {
                    retVal = disagreeingAnim;
                    break;
                }

            default:
                {
                    break;
                }
        }

        return retVal;
    }

    public FrameAnimator GetFanim()
    {
        return fanim;
    }

    public void StartConversation()
    {
        isTalking = true;
    }

    public void EndConversation()
    {
        isTalking = false;
    }

    public void StartInteraction(Character chr)
    {

        DialogueManager.StartDialogue(this, dialogue[0]);

    }
}
