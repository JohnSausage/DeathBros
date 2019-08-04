using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectManager : MonoBehaviour
{
    [SerializeField]
    protected ComboCardDataSO[] comboCards;

    [SerializeField]
    protected bool[] cardEnabled = { false, false, false, false, false };



    protected Player player;
    protected float oldComboPower;
    protected float[] comboPowerLimits = { 20f, 40f, 60f, 80f, 100f };



    private void Start()
    {
        oldComboPower = 0;
        player = GetComponent<Player>();

        player.AChangeComboPower += UpdateComboCards;
    }



    public Damage GetModifiedDamage(Damage damage)
    {
        Damage returnDamage = damage;

        for (int i = 0; i < comboCards.Length; i++)
        {
            if(cardEnabled[i] == false)
            {
                continue;
            }

            if (comboCards[i] == null)
            {
                continue;
            }

            returnDamage = comboCards[i].ModifyDamage(returnDamage);
        }

        return returnDamage;
    }



    protected void UpdateComboCards(float comboPower)
    {
        for (int i = 0; i < comboCards.Length; i++)
        {
            UpdateComboCard(i, comboPower, comboPowerLimits[i]);
        }


        oldComboPower = comboPower;
    }



    private void UpdateComboCard(int index, float comboPower, float comboPowerLimit)
    {
        if (index >= comboCards.Length)
        {
            return;
        }

        if(comboCards[index] == null)
        {
            return;
        }

        if (oldComboPower < comboPowerLimit && comboPower >= comboPowerLimit)
        {
            comboCards[index].ApplyEffect(player);
            cardEnabled[index] = true;
        }
        else if (oldComboPower >= comboPowerLimit && comboPower < comboPowerLimit)
        {
            comboCards[index].RemoveEffect(player);
            cardEnabled[index] = false;
        }
    }
}
