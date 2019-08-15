using System;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectManager : MonoBehaviour
{
    [SerializeField]
    protected string buffEffectName, debuffEffectName;

    [SerializeField]

    protected string buffAudioName, debuffAudioName;


    [SerializeField]
    protected ComboCardDataSO[] comboCards;

    [SerializeField]
    protected bool[] cardEnabled = { false, false, false, false, false };



    protected Player player;
    protected float oldComboPower;
    protected float[] comboPowerLimits = { 0f, 20f, 40f, 60f, 80f, 100f };

    public event Action<int, ComboCardDataSO> AUpdateComboCard;
    public event Action<int, bool> AUpdateComboCardStatus;

    private void Start()
    {
        oldComboPower = 0;
        player = GetComponent<Player>();

        player.AChangeComboPower += UpdateComboCards;

        //@@@ maybe remove later
        //triggers all update card events to trigger updates
        for (int i = 0; i < 5; i++)
        {
            if (i >= comboCards.Length)
            {
                break;
            }

            if (comboCards[i] == null)
            {
                continue;
            }

            if (AUpdateComboCard != null) AUpdateComboCard(i, comboCards[i]);
        }
    }



    public Damage GetModifiedDamage(Damage damage)
    {
        Damage returnDamage = damage;

        for (int i = 0; i < comboCards.Length; i++)
        {
            if (cardEnabled[i] == false)
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
            UpdateComboCard(i, comboPower, comboPowerLimits[i], comboPowerLimits[i + 1]);
        }


        oldComboPower = comboPower;
    }



    private void UpdateComboCard(int index, float comboPower, float lowerComboPowerLimit, float upperComboPowerLimit)
    {
        if (index >= comboCards.Length)
        {
            return;
        }

        if (comboCards[index] == null)
        {
            return;
        }

        if (oldComboPower < upperComboPowerLimit && comboPower >= upperComboPowerLimit && cardEnabled[index] == false)
        {
            comboCards[index].ApplyEffect(player);
            cardEnabled[index] = true;
            EffectManager.SpawnEffect(buffEffectName, transform);
            AudioManager.PlaySound(buffAudioName);

            if (AUpdateComboCardStatus != null) AUpdateComboCardStatus(index, true);
        }
        else if (oldComboPower >= lowerComboPowerLimit && comboPower < lowerComboPowerLimit)
        {
            comboCards[index].RemoveEffect(player);
            cardEnabled[index] = false;
            EffectManager.SpawnEffect(debuffEffectName, transform);
            AudioManager.PlaySound(debuffAudioName);

            if (AUpdateComboCardStatus != null) AUpdateComboCardStatus(index, false);
        }
    }
}
