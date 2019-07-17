using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Panel_AvlSkills : MenuPanel
{
    [SerializeField]
    protected CardDisplay cardDisplay_main;

    [SerializeField]
    protected CardDisplay cardDisplay_up1;

    [SerializeField]
    protected CardDisplay cardDisplay_up2;

    [SerializeField]
    protected CardDisplay cardDisplay_down1;

    [SerializeField]
    protected CardDisplay cardDisplay_down2;

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

    protected int currentScrollPosition;
    protected const int scrollAtPosition = 3;


    private void Awake()
    {
        buttons_AvlSkills = new List<Button_CardDataSO>();
    }

    public override void Enter()
    {
        base.Enter();

        currentScrollPosition = 0;

        //check if in skill setting mode
        if (CardManager.setSpecialIndex >= 0)
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

            if ((i + 1) < buttons_AvlSkills.Count)
            {
                buttonNav.selectOnDown = buttons_AvlSkills[i + 1].GetComponent<Button>();
            }

            button.navigation = buttonNav;
        }

        //set position for buttons in list
        for (int i = 0; i < buttons_AvlSkills.Count; i++)
        {
            RectTransform rt = buttons_AvlSkills[i].GetComponent<RectTransform>();
            rt.anchorMax = new Vector2(0, 1f);
            rt.anchorMin = new Vector2(0, 1f);
            rt.pivot = new Vector2(0, 1f);
            rt.sizeDelta = new Vector2(192, rt.sizeDelta.y);
            buttons_AvlSkills[i].transform.localPosition += (Vector3)(offset * i * 32);
        }
    }

    public override void Execute()
    {
        base.Execute();

        int listCount = buttons_AvlSkills.Count;

        for (int i = 0; i < listCount; i++)
        {
            Button_CardDataSO button = buttons_AvlSkills[i];

            if (button.gameObject == EventSystem.current.currentSelectedGameObject)
            {
                if ((i - 2) > 0)
                {
                    cardDisplay_up2.SetCardData(buttons_AvlSkills[i - 2].cardDataSO);
                }
                else
                {
                    cardDisplay_up2.SetEmpty();
                }

                if ((i - 1) > 0)
                {
                    cardDisplay_up1.SetCardData(buttons_AvlSkills[i - 1].cardDataSO);
                }
                else
                {
                    cardDisplay_up1.SetEmpty();
                }

                cardDisplay_main.SetCardData(button.cardDataSO);

                if ((i + 1) < listCount)
                {
                    cardDisplay_down1.SetCardData(buttons_AvlSkills[i + 1].cardDataSO);
                }
                else
                {
                    cardDisplay_down1.SetEmpty();
                }

                if ((i + 2) < listCount)
                {
                    cardDisplay_down2.SetCardData(buttons_AvlSkills[i + 2].cardDataSO);
                }
                else
                {
                    cardDisplay_down2.SetEmpty();
                }



                if ((i - scrollAtPosition) > currentScrollPosition && i < (listCount - scrollAtPosition))
                {
                    currentScrollPosition++;// = i - scrollAtPosition;

                    MoveButtonsY(32);
                }

                if ((i - scrollAtPosition) < currentScrollPosition && (i >= scrollAtPosition))
                {
                    currentScrollPosition--;

                    MoveButtonsY(-32);
                }
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

    public void MoveButtonsY(float units)
    {
        foreach (Button_CardDataSO button in buttons_AvlSkills)
        {
            //button.transform.localPosition += Vector3.up * y;
            //button.transform.Translate(Vector3.up * units / 32f);
            StartCoroutine(CMoveTransform(button.transform, units));
        }
    }

    public void ResetLocalButtonPosition()
    {
        foreach (Button_CardDataSO button in buttons_AvlSkills)
        {
            button.transform.localPosition = button.OriginalLocalPosition;
        }
    }

    protected IEnumerator CMoveTransform(Transform trf, float y)
    {
        for (int i = 0; i < 10; i++)
        {
            trf.Translate(Vector3.up * y / 32f / 10f);
            yield return null;
        }
    }
}
