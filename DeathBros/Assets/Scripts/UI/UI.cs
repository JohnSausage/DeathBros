using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : _MB
{
    [SerializeField]
    private Color soulMeterColor;
    public Color SoulBarColor { get { return soulMeterColor; } }

    [SerializeField]
    private Slider soulMeterSlider;

    //[SerializeField]
    //private TextMeshProUGUI comboCounterText;

    //[SerializeField]
    //private TextMeshProUGUI messageText;

    //[SerializeField]
    //private GameObject ComboPanelPrefab;

    [SerializeField]
    private GameObject soulPanel;

    [SerializeField]
    private TextMeshProUGUI soulBankText;

    [SerializeField]
    private GameObject imageSoul;

    //public List<ComboPanel> comboPanels;
//
    public List<ComboCounter> comboCounters;
    public List<UIMessage> uiMessages;


    private int soulMeterFlashTimer = 0;

    public override void Init()
    {
        base.Init();

        //Player.EnemyHit += UpdateComboCounter;

        //Enemy.ATakesDamageAll += CheckComboCounter;

        //GameManager.Player.ASoulsChanged += UpdateSouls;
        //GameManager.Player.ASoulBankPlus += UpdateSoulBank;
        //GameManager.Player.ASoulMeterChanged += UpdateSoulMeter;

        //comboPanels = new List<ComboPanel>();
    }

    public override void LateInit()
    {
        base.LateInit();

        //UpdateSouls(GameManager.Player.currentSouls);
    }

    void FixedUpdate()
    {
        UpdateSoulMeterColor();

        //soulMeterSlider.value = GameManager.Player.SoulPercent;

        /*
        comboCounterText.text = "";

        for (int i = 0; i < comboCounters.Count; i++)
        {
            comboCounters[i].FixedUpdate();

            //comboCounterText.text += comboCounters[i].EnemyName + ": \n";
            //comboCounterText.text += comboCounters[i].ComboDamage + " Damage / ";
            //comboCounterText.text += comboCounters[i].HitCount + " Hits \n";

            if (comboCounters[i].ComboOver())
            {
                if (comboCounters[i].damages.Count > 1)
                {
                    // uiMessages.Add(new UIMessage("Combo Heal: " + comboCounters[i].ComboScore() + "\n", Color.green));
                    string message = "Combo on " + comboCounters[i].EnemyName + "\n";

                    for (int j = 0; j < comboCounters[i].damages.Count; j++)
                    {
                        message += comboCounters[i].damages[j].attackType + "\n";
                    }

                    message += "\n";

                    message += "Combo Score: " + comboCounters[i].ComboScore;

                    message += "\n";

                    uiMessages.Add(new UIMessage(message, Color.green));
                }

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
        */

    }

    private void UpdateSoulMeter(float newValue)
    {
        //Color flashColor;

        //if (GameManager.Player.SoulPercent >= soulMeterSlider.value)
        //{
        //    flashColor = Color.white;
        //}
        //else
        //{
        //    flashColor = Color.red;
        //}

        //FlashSoulMeterColor(flashColor);

        //soulMeterSlider.value = GameManager.Player.SoulPercent;
    }

    private void FlashSoulMeterColor(Color color)
    {
        soulMeterFlashTimer = 0;
        soulMeterSlider.fillRect.GetComponent<Image>().color = color;
    }

    private void UpdateSoulMeterColor()
    {
        soulMeterFlashTimer++;

        if (soulMeterFlashTimer > 60)
        {
            soulMeterSlider.fillRect.GetComponent<Image>().color = soulMeterColor;
        }
    }

    private void UpdateSouls(float souls)
    {
        for (int i = 0; i < soulPanel.transform.childCount; i++)
        {
            Destroy(soulPanel.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < souls; i++)
        {
            GameObject newImageSoul = Instantiate(imageSoul, soulPanel.transform);
            newImageSoul.GetComponent<RectTransform>().anchoredPosition = new Vector2(30 + i * 30, 0);
            //newImageSoul.transform.localPosition = new Vector2(30 + i * 30, 0);
        }
    }

    private void UpdateSoulBank(int soulBank)
    {
        soulBankText.text = soulBank.ToString();
    }

    /*
    private void CheckComboCounter(Damage damage, Character enemy)
    {
        if (damage.Owner != GameManager.Player) return;


        bool enemyFound = false;

        for (int i = 0; i < comboCounters.Count; i++)
        {
            if (comboCounters[i].enemy == enemy)
            {
                enemyFound = true;
                comboCounters[i].damages.Add(damage);
                comboCounters[i].ResetComboCounter();
            }
        }

        if (!enemyFound)
        {
            ComboCounter newComboCount = new ComboCounter((Enemy)enemy);
            newComboCount.damages.Add(damage);

            GameObject newPanel = Instantiate(ComboPanelPrefab);
            newPanel.transform.SetParent(transform);
            comboCounters.Add(newComboCount);
        }

    }
*/
}
[System.Serializable]
public class ComboCounter
{
    public string EnemyName { get; protected set; }

    public Enemy enemy;

    public List<Damage> damages;

    public float ComboDamage { get; protected set; }
    public float HitCount { get; protected set; }
    public float ComboScore { get; protected set; }

    private int comboResetDuration;
    private int timer;
    private float comboMultiplier;

    public static event Action<float> AComboIsOver;

    public ComboCounter(Enemy enemy)
    {
        damages = new List<Damage>();
        this.enemy = enemy;
        EnemyName = enemy.charName;
        comboMultiplier = enemy.ComboMultiplier;
        comboResetDuration = 90;
        timer = 0;

        enemy.AComboOver += SetComboOverInEvent;
    }

    public void FixedUpdate()
    {
        timer++;
    }

    public void CalculateComboScore()
    {
        ComboDamage = 0;

        if (damages.Count <= 1) return;

        List<EAttackType> attackTypeList = new List<EAttackType>();

        for (int i = 0; i < damages.Count; i++)
        {
            if (!attackTypeList.Contains(damages[i].attackType))
            {
                ComboDamage += damages[i].damageNumber;
                attackTypeList.Add(damages[i].attackType);
            }
            else
            {
                ComboDamage += damages[i].damageNumber * 0.5f;
            }


        }

        ComboScore = ComboDamage * comboMultiplier;
    }

    public void ResetComboCounter()
    {
        timer = 0;
    }

    private void SetComboOverInEvent(bool value)
    {
        timer = comboResetDuration + 1;
    }

    public bool ComboOver()
    {
        if (timer > comboResetDuration)
        {
            CalculateComboScore();

            enemy.AComboOver -= SetComboOverInEvent;

            if (AComboIsOver != null) AComboIsOver(ComboScore);

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