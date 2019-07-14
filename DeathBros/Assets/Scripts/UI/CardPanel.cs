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

    [SerializeField]
    protected GameObject availableSkillsGO;

    [SerializeField]
    protected GameObject content_AvlSkills;

    [SerializeField]
    protected Button_CardDataSO cardButtonPrefab;

    protected List<Button_CardDataSO> buttons_AvlSkills;


    private void Awake()
    {
        buttons_AvlSkills = new List<Button_CardDataSO>();
    }

    public override void Enter()
    {
        base.Enter();

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

        for (int i = 0; i < GameManager.Instance.saveData.currentSkillIDs.Length; i++)
        {
            CardDataSO cardDataSO = CardManager.GetLoadedCardData(GameManager.Instance.saveData.currentSkillIDs[i]);

            buttons_CurrentSkills[i].SetCardData(cardDataSO);
        }


        SelectCurrentSkills();
    }

    public override void Execute()
    {
        base.Execute();

        foreach(Button_CardDataSO button in buttons_AvlSkills)
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
    }

    public void SelectCurrentSkills()
    {
        currentSkillsGO.SetActive(true);
        availableSkillsGO.SetActive(false);
    }
}
