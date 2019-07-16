using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Panel_AvlSkills : MenuPanel
{
    [SerializeField]
    protected CardDisplay cardDisplay;

    [SerializeField]
    protected Button button_CurrentSKills;

    [SerializeField]
    protected Button button_AvlSKills;

    [SerializeField]
    protected GameObject button_setSkill;

    [Space]

    [SerializeField]
    protected GameObject content_AvlSkills;

    [SerializeField]
    protected Button_CardDataSO cardButtonPrefab;

    [Space]

    [SerializeField]
    protected Panel_CurrentSkills panel_CurrentSkills;




    protected List<Button_CardDataSO> buttons_AvlSkills;


    private void Awake()
    {
        buttons_AvlSkills = new List<Button_CardDataSO>();
    }

    public override void Enter()
    {
        base.Enter();

        //check if in skill setting mode
        if(CardManager.setSpecialIndex >= 0)
        {
            button_setSkill.SetActive(true);
        }
        else
        {
            button_setSkill.SetActive(false);
        }

        //destroy and reload buttons for avlSkills
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

        //set up navigation
        if (buttons_AvlSkills.Count > 0)
        {        
            Button firstButtonInList = buttons_AvlSkills[0].GetComponent<Button>();

            //select first button
            firstButtonInList.Select();
            firstButtonInList.OnSelect(eventData);

            //set navigation for first button
            Navigation firstButtonInListNav = new Navigation();
            firstButtonInListNav.mode = Navigation.Mode.Explicit;

            firstButtonInListNav.selectOnUp = button_CurrentSKills;

            if (buttons_AvlSkills.Count > 1)
            {
                firstButtonInListNav.selectOnDown = buttons_AvlSkills[1].GetComponent<Button>();
            }

            firstButtonInList.navigation = firstButtonInListNav;

            //set navigation for currentSkillButton
            Navigation currentSkillsNav = button_CurrentSKills.navigation;
            currentSkillsNav.selectOnDown = firstButtonInList;
            button_CurrentSKills.navigation = currentSkillsNav;

            //set navigation for avlSkillButton
            Navigation avlSkillsNav = button_AvlSKills.navigation;
            avlSkillsNav.selectOnDown = firstButtonInList;
            button_AvlSKills.navigation = avlSkillsNav;
        }

        Vector2 offset = Vector2.down;

        //set navigation for buttons in list
        for (int i = 1; i < buttons_AvlSkills.Count; i++)
        {
            Button button = buttons_AvlSkills[i].GetComponent<Button>();
            Navigation buttonNav = button.navigation;
            buttonNav.mode = Navigation.Mode.Explicit;

            buttonNav.selectOnUp = buttons_AvlSkills[i - 1].GetComponent<Button>();

            if((i + 1) < buttons_AvlSkills.Count)
            {
                buttonNav.selectOnDown = buttons_AvlSkills[i + 1].GetComponent<Button>();
            }

            button.navigation = buttonNav;
        }

        //set position for buttons in list
        for (int i = 0; i < buttons_AvlSkills.Count; i++)
        {
            RectTransform rt = buttons_AvlSkills[i].GetComponent<RectTransform>();
            rt.anchorMax = new Vector2(1, 0.5f);
            rt.anchorMin = new Vector2(0, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            buttons_AvlSkills[i].transform.localPosition += (Vector3)(offset * i * 32);
        }
    }

    public override void Execute()
    {
        base.Execute();

        foreach (Button_CardDataSO button in buttons_AvlSkills)
        {
            if (button.gameObject == EventSystem.current.currentSelectedGameObject)
            {
                cardDisplay.SetCardData(button.cardDataSO);
            }
        }
    }

    public void Press_AvlSkillButton(Button_CardDataSO button)
    {
        if (CardManager.setSpecialIndex < 0)
        {
            return;
        }

        //set special in SaveData
        GameManager.SaveData.SetSpecialIDAtIndex(CardManager.setSpecialIndex, button.cardDataSO.cardID);

        //set special in CardManager
        CardManager.Instance.currentSkillsCardData[CardManager.setSpecialIndex] = button.cardDataSO;

        //set special in Player
        GameManager.Player.SetSpecialAttack(CardManager.setSpecialIndex, button.cardDataSO.attackStateSO);

        PauseMenu.Instance.ChangeMenuPanel(panel_CurrentSkills);
    }
}
