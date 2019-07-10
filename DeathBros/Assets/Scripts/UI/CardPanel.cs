using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPanel : MenuPanel
{
    [SerializeField]
    protected GameObject currentSkillsGO;

    [SerializeField]
    protected GameObject availableSkillsGO;


    public override void Enter()
    {
        base.Enter();

        SelectCurrentSkills();
    }

    public override void Execute()
    {
        base.Execute();
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
