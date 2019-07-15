using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Panel_CurrentSkills : MenuPanel
{
    [SerializeField]
    protected CardDisplay cardDisplay;

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

        CardManager.setSpecialIndex = -1;

        for (int i = 0; i < CardManager.Instance.currentSkillsCardData.Count; i++)
        {

            buttons_CurrentSkills[i].SetCardData(CardManager.Instance.currentSkillsCardData[i]);
        }
    }

    public override void Execute()
    {
        base.Execute();

        foreach (Button_CardDataSO button in buttons_CurrentSkills)
        {
            if (button.gameObject == EventSystem.current.currentSelectedGameObject)
            {
                cardDisplay.SetCardData(button.cardDataSO);
            }
        }
    }

    public void Press_SkillButton(int specialIndex)
    {
        CardManager.setSpecialIndex = specialIndex;

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
