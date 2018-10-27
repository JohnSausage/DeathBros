using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    [SerializeField]
    private Slider healthBar;

    [SerializeField]
    private TextMeshProUGUI comboCounterText;

    [SerializeField]
    private TextMeshProUGUI messageText;

    public List<ComboCount> comboCounters;
    public List<UIMessage> uiMessages;

    void Start()
    {
        Player.PlayerHealthChanged += UpdatePlayerHealth;
        HitboxManager.PlayerHitsEnenmy += UpdateComboCounter;
    }

    void FixedUpdate()
    {
        comboCounterText.text = "";

        for (int i = 0; i < comboCounters.Count; i++)
        {
            comboCounters[i].FixedUpdate();

            comboCounterText.text += comboCounters[i].EnemyName + ": ";
            comboCounterText.text += comboCounters[i].ComboDamage;
            comboCounterText.text += "\n";

            if (comboCounters[i].ComboOver())
            {
                uiMessages.Add(new UIMessage("Combo Heal: " + comboCounters[i].ComboDamage, Color.green));

                comboCounters.Remove(comboCounters[i]);
            }
        }

        messageText.text = "";

        for (int i = 0; i < uiMessages.Count; i++)
        {
            messageText.color = uiMessages[i].color;
            messageText.text += uiMessages[i].text;

            if (uiMessages[i].Over()) uiMessages.Remove(uiMessages[i]);
        }
    }

    private void UpdatePlayerHealth(float newValue)
    {
        healthBar.value = newValue;
    }

    private void UpdateComboCounter(Enemy enemy, Damage damage)
    {
        bool enemyFound = false;

        for (int i = 0; i < comboCounters.Count; i++)
        {
            if (comboCounters[i].enemy == enemy)
            {
                enemyFound = true;
                comboCounters[i].AddDamage(damage.damageNumber);
            }
        }

        if (!enemyFound)
        {
            ComboCount newComboCount = new ComboCount(enemy);
            newComboCount.AddDamage(damage.damageNumber);

            comboCounters.Add(newComboCount);
        }
    }
}

[System.Serializable]
public class ComboCount
{
    public string EnemyName { get; protected set; }

    public Enemy enemy;

    public float ComboDamage { get; protected set; }

    private int comboResetDuration;
    private int timer;

    public static event Action<float> ComboIsOver;

    public ComboCount(Enemy enemy)
    {
        this.enemy = enemy;
        EnemyName = enemy.charName;
        comboResetDuration = 120;
        timer = 0;
    }

    public void FixedUpdate()
    {
        timer++;
    }

    public void AddDamage(float damageNumber)
    {
        ComboDamage += damageNumber;
        timer = 0;
    }

    public bool ComboOver()
    {
        if (timer > comboResetDuration)
        {
            if (ComboIsOver != null) ComboIsOver(ComboDamage);
            return true;
        }

        return false;
    }
}

[System.Serializable]
public class UIMessage
{
    public string text;
    public Color color;

    private int duration = 180;
    private int timer;

    public UIMessage(string text, Color color)
    {
        this.text = text;
        this.color = color;
    }

    public bool Over()
    {
        timer++;

        if (timer > duration)
        {
            return true;
        }
        return false;
    }
}