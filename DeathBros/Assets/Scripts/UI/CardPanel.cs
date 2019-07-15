using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardPanel : MenuPanel
{
    [SerializeField]
    protected CardDisplay cardDisplay;

    [SerializeField]
    protected GameObject currentSkillsGO;

    [SerializeField]
    protected List<Button_CardDataSO> buttons_CurrentSkills;
    protected Button_CardDataSO button_selectedSkill;

    [SerializeField]
    protected GameObject availableSkillsGO;

    [SerializeField]
    protected GameObject content_AvlSkills;

    [SerializeField]
    protected Button button_setSkill;

    [SerializeField]
    protected Button_CardDataSO cardButtonPrefab;

    protected List<Button_CardDataSO> buttons_AvlSkills;

    protected bool displayingCurrentSkills;

    protected enum ECardPanelState { Normal, SkillCardSelected, CurrentSkillSelected };
    protected ECardPanelState cardPanelState;

    private void Awake()
    {
        buttons_AvlSkills = new List<Button_CardDataSO>();
    }

    public override void Enter()
    {
        base.Enter();

        cardPanelState = ECardPanelState.Normal;

        for (int i = 0; i < buttons_AvlSkills.Count; i++)
        {
            Destroy(buttons_AvlSkills[i].gameObject);
        }

        buttons_AvlSkills.Clear();

        foreach (CardDataSO cardData in CardManager.Instance.cardDataSOList)
        {
            Button_CardDataSO newButton = Instantiate(cardButtonPrefab, content_AvlSkills.transform);
            newButton.SetCardData(cardData);

            buttons_AvlSkills.Add(newButton);
        }

        for (int i = 0; i < CardManager.Instance.currentSkillsCardData.Count; i++)
        {

            buttons_CurrentSkills[i].SetCardData(CardManager.Instance.currentSkillsCardData[i]);
        }


        SelectCurrentSkills();
    }

    public override void Execute()
    {
        base.Execute();

        switch (cardPanelState)
        {
            case ECardPanelState.Normal: //Display buttons and switch selected card in cardDisplay
                {
                    foreach (Button_CardDataSO button in buttons_AvlSkills)
                    {
                        if (button.gameObject == EventSystem.current.currentSelectedGameObject)
                        {
                            cardDisplay.SetCardData(button.cardDataSO);
                        }
                    }

                    foreach (Button_CardDataSO button in buttons_CurrentSkills)
                    {
                        if (button.gameObject == EventSystem.current.currentSelectedGameObject)
                        {
                            cardDisplay.SetCardData(button.cardDataSO);
                            button_selectedSkill = button;
                        }
                    }

                    break;
                }

            case ECardPanelState.SkillCardSelected:
                {
                    break;
                }


            case ECardPanelState.CurrentSkillSelected:
                {
                    break;
                }

            default:
                {
                    break;
                }
        }

    }

    public override void Exit()
    {
        base.Exit();
    }


    public void SelectAvailableSkills()
    {
        currentSkillsGO.SetActive(false);
        availableSkillsGO.SetActive(true);

        displayingCurrentSkills = false;

    }

    public void SelectCurrentSkills()
    {
        currentSkillsGO.SetActive(true);
        availableSkillsGO.SetActive(false);

        displayingCurrentSkills = true;
    }

    public void SetSkillButtonPressed()
    {
        if (displayingCurrentSkills)
        {
            cardPanelState = ECardPanelState.CurrentSkillSelected;

            SelectAvailableSkills();
        }
        else
        {
            cardPanelState = ECardPanelState.SkillCardSelected;

            SelectCurrentSkills();
        }
    }

    public void PressButtonCardData(int index)
    {
        Debug.Log(cardPanelState);

        Button_CardDataSO pressedButtonCardDataButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button_CardDataSO>();

        if (pressedButtonCardDataButton == null)
        {
            Debug.Log("pressedButtonCardDataButton == null");
            return;
        }

        if (cardPanelState == ECardPanelState.Normal)
        {
            button_setSkill.Select();
            button_setSkill.OnSelect(eventData);
        }

        if (cardPanelState == ECardPanelState.CurrentSkillSelected)
        {
            button_selectedSkill.SetCardData(pressedButtonCardDataButton.cardDataSO);

            CardManager.Instance.currentSkillsCardData[button_selectedSkill.specialIndex] = pressedButtonCardDataButton.cardDataSO;
            CardManager.SaveSkillIDs();
            CardManager.SetPlayerSkills();

            cardPanelState = ECardPanelState.Normal;

            SelectCurrentSkills();

            Button setSelectedButton = button_selectedSkill.GetComponent<Button>();

            setSelectedButton.Select();
            setSelectedButton.OnSelect(eventData);

        }
    }
}
