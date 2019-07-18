using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Panel_AvlSkills : MenuPanel
{
    [SerializeField]
    protected CardDisplay cardDisplay_main;
    protected Vector2 orgPosMain;

    [SerializeField]
    protected CardDisplay cardDisplay_up1;
    protected Vector2 orgPosUp1;

    [SerializeField]
    protected CardDisplay cardDisplay_up2;
    protected Vector2 orgPosUp2;

    [SerializeField]
    protected CardDisplay cardDisplay_down1;
    protected Vector2 orgPosDown1;

    [SerializeField]
    protected CardDisplay cardDisplay_down2;
    protected Vector2 orgPosDown2;

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
    protected Button_CardDataSO button_selected;

    protected int currentScrollPosition;
    protected const int scrollAtPosition = 3;
    protected int cardIndex;
    protected int selectedCardIndex;

    private void Awake()
    {
        buttons_AvlSkills = new List<Button_CardDataSO>();

        orgPosMain = cardDisplay_main.transform.localPosition;
        orgPosUp1 = cardDisplay_up1.transform.localPosition;
        orgPosUp2 = cardDisplay_up2.transform.localPosition;
        orgPosDown1 = cardDisplay_down1.transform.localPosition;
        orgPosDown2 = cardDisplay_down2.transform.localPosition;

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

        selectedCardIndex = 0;
        SetCardsToIndex(selectedCardIndex);
    }

    public override void Execute()
    {
        base.Execute();

        int listCount = buttons_AvlSkills.Count;


        for (cardIndex = 0; cardIndex < listCount; cardIndex++)
        {
            Button_CardDataSO button = buttons_AvlSkills[cardIndex];

            if (button.gameObject == EventSystem.current.currentSelectedGameObject && button_selected != button)
            {
                button_selected = button;


                if (selectedCardIndex > cardIndex)
                {
                    StartCoroutine(CMoveAllCardsDown(cardIndex));
                }
                else if (selectedCardIndex < cardIndex)
                {
                    StartCoroutine(CMoveAllCardsUp(cardIndex));
                }
                else
                {
                    SetCardsToIndex(cardIndex);
                }

                selectedCardIndex = cardIndex;

                //move buttons up
                if ((cardIndex - scrollAtPosition) > currentScrollPosition && cardIndex < (listCount - scrollAtPosition))
                {
                    currentScrollPosition++;

                    MoveButtonsY(32);
                }

                //move buttons down
                if ((cardIndex - scrollAtPosition) < currentScrollPosition && (cardIndex >= scrollAtPosition))
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
        for (int i = 0; i < buttons_AvlSkills.Count; i++)
        {
            StartCoroutine(CMoveTransform(buttons_AvlSkills[i].transform, units));
        }
    }

    public void ResetLocalButtonPosition()
    {
        foreach (Button_CardDataSO button in buttons_AvlSkills)
        {
            button.transform.localPosition = button.OriginalLocalPosition;
        }
    }

    protected IEnumerator CMoveTransform(Transform tr, float y)
    {
        int steps = 10;

        for (int i = 0; i < steps; i++)
        {
            tr.Translate(Vector3.up * y / 32f / steps);
            yield return null;
        }
    }

    protected IEnumerator CMoveAllCardsUp(int index)
    {
        int steps = 10;

        for (int i = 0; i < steps; i++)
        {
            yield return null;

            Vector2 offset;

            offset = orgPosDown1 - orgPosDown2;
            cardDisplay_down2.transform.Translate(offset / 32f / steps);

            offset = orgPosMain - orgPosDown1;
            cardDisplay_down1.transform.Translate(offset / 32f / steps);

            offset = orgPosUp1 - orgPosMain;
            cardDisplay_main.transform.Translate(offset / 32f / steps);

            offset = orgPosUp2 - orgPosUp1;
            cardDisplay_up1.transform.Translate(offset / 32f / steps);
        }


        //CardDisplay tmp = cardDisplay_down2;
        //cardDisplay_down2 = cardDisplay_down1;
        //cardDisplay_down1 = cardDisplay_main;
        //cardDisplay_main = cardDisplay_up1;
        //cardDisplay_up1 = cardDisplay_up2;
        //cardDisplay_up2 = tmp;


        //SetCardsEmpty();
        cardDisplay_down2.transform.localPosition = orgPosDown2;
        cardDisplay_down1.transform.localPosition = orgPosDown1;
        cardDisplay_main.transform.localPosition = orgPosMain;
        cardDisplay_up1.transform.localPosition = orgPosUp1;
        cardDisplay_up2.transform.localPosition = orgPosUp2;
        SetCardsToIndex(index);
    }

    protected IEnumerator CMoveAllCardsDown(int index)
    {
        int steps = 10;

        for (int i = 0; i < steps; i++)
        {
            yield return null;
            Vector2 offset;

            offset = orgPosUp2 - orgPosUp1;
            cardDisplay_up2.transform.Translate(offset / 32f / steps);

            offset = orgPosMain - orgPosUp1;
            cardDisplay_up1.transform.Translate(offset / 32f / steps);

            offset = orgPosDown1 - orgPosMain;
            cardDisplay_main.transform.Translate(offset / 32f / steps);

            offset = orgPosDown2 - orgPosDown1;
            cardDisplay_down1.transform.Translate(offset / 32f / steps);        
        }


        //CardDisplay tmp = cardDisplay_up2;
        //cardDisplay_up2   = cardDisplay_up1;
        //cardDisplay_up1   = cardDisplay_main;
        //cardDisplay_main  = cardDisplay_down1;
        //cardDisplay_down1 = cardDisplay_down2;
        //cardDisplay_down2 = tmp;

        //SetCardsEmpty();
        SetCardNumber(index, -2, cardDisplay_down2);
        cardDisplay_down2.transform.localPosition = orgPosDown2;

        SetCardNumber(index, -1, cardDisplay_down1);
        cardDisplay_down1.transform.localPosition = orgPosDown1;

        SetCardNumber(index, 0, cardDisplay_main);
        cardDisplay_main.transform.localPosition = orgPosMain;

        SetCardNumber(index, +1, cardDisplay_up1);
        cardDisplay_up1.transform.localPosition = orgPosUp1;

        SetCardNumber(index, +2, cardDisplay_up2);
        cardDisplay_up2.transform.localPosition = orgPosUp2;


        //SetCardsToIndex(index);
    }

    protected void SetCardsEmpty()
    {
        cardDisplay_up2.SetEmpty();
        cardDisplay_up1.SetEmpty();
        cardDisplay_main.SetEmpty();
        cardDisplay_down1.SetEmpty();
        cardDisplay_down2.SetEmpty();
    }

    protected void SetCardNumber(int index, int offset, CardDisplay cardDisplay)
    {
        if ((index + offset) >= 0 && (index + offset) < buttons_AvlSkills.Count)
        {
            cardDisplay.SetCardData(buttons_AvlSkills[index + offset].cardDataSO);
        }
        else
        {
            cardDisplay.SetEmpty();
        }
    }

    protected void SetCardsToIndex(int index)
    {
        Debug.Log("setting cards to index " + index);
        if (index >= buttons_AvlSkills.Count)
        {
            return;
        }

        if ((index - 2) >= 0)
        {
            cardDisplay_up2.SetCardData(buttons_AvlSkills[index - 2].cardDataSO);
        }
        else
        {
            cardDisplay_up2.SetEmpty();
        }

        if ((index - 1) >= 0)
        {
            cardDisplay_up1.SetCardData(buttons_AvlSkills[index - 1].cardDataSO);
        }
        else
        {
            cardDisplay_up1.SetEmpty();
        }

        cardDisplay_main.SetCardData(buttons_AvlSkills[index].cardDataSO);


        if ((index + 1) < buttons_AvlSkills.Count)
        {
            cardDisplay_down1.SetCardData(buttons_AvlSkills[index + 1].cardDataSO);
        }
        else
        {
            cardDisplay_down1.SetEmpty();
        }

        if ((index + 2) < buttons_AvlSkills.Count)
        {
            cardDisplay_down2.SetCardData(buttons_AvlSkills[index + 2].cardDataSO);
        }
        else
        {
            cardDisplay_down2.SetEmpty();
        }
    }
}
