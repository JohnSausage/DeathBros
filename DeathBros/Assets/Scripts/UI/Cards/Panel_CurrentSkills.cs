using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Panel_CurrentSkills : MenuPanel
{
    [SerializeField]
    protected List<CardDisplay> cardDisplays;

    [SerializeField]
    protected List<Button_CardDataSO> buttons_CurrentSkills;

    [Space]

    [SerializeField]
    protected Button button_setSkill;

    [Space]

    [SerializeField]
    protected MenuPanel panel_AvlSkills;

    protected Button_CardDataSO button_skillToSet;

    public override void Enter()
    {
        base.Enter();

        if (button_skillToSet != null)
        {
            button_skillToSet.GetComponent<Button>().Select();
            button_skillToSet.OnSelect(eventData);

            button_skillToSet = null;
        }

        CardManager.setSpecialIndex = -1;

        for (int i = 0; i < CardManager.Instance.currentSkillsCardData.Count; i++)
        {
            buttons_CurrentSkills[i].SetCardData(CardManager.Instance.currentSkillsCardData[i]);
        }

        for (int i = 0; i < buttons_CurrentSkills.Count; i++)
        {
            cardDisplays[i].SetCardData(buttons_CurrentSkills[i].cardDataSO);
            cardDisplays[i].SetOffsetOnSelect(Vector2.left * 9f);
        }
    }

    public override void Execute()
    {
        base.Execute();

        for (int i = 0; i < buttons_CurrentSkills.Count; i++)
        {
            if ((buttons_CurrentSkills[i].gameObject == EventSystem.current.currentSelectedGameObject) ||
                (buttons_CurrentSkills[i] == button_skillToSet))
            {
                cardDisplays[i].MoveToPositionOnSelect();
            }
            else
            {
                cardDisplays[i].Deselect();
            }
        }
    }

    public void Press_SkillButton(int specialIndex)
    {
        CardManager.setSpecialIndex = specialIndex;

        Button_CardDataSO pressedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button_CardDataSO>();

        if(pressedButton != null)
        {
            button_skillToSet = pressedButton;
        }

        button_setSkill.Select();
        button_setSkill.OnSelect(eventData);
    }

    public void Press_SetButton()
    {
        if (CardManager.setSpecialIndex < 0)
        {
            return;
        }


        PauseMenu.Instance.ChangeMenuPanel(panel_AvlSkills);
    }
}
