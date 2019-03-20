using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    public string statName;
    public float baseValue;

    public float CurrentValue { get; private set; }

    public event System.Action<float> EStatBaseValueChanged;

    public Stat() { }

    public Stat(string statName, float baseValue)
    {
        this.statName = statName;
        this.baseValue = baseValue;

        CurrentValue = baseValue;
    }

    public Stat Clone()
    {
        return new Stat(statName, baseValue);
    }

    private List<StatMod> mods = new List<StatMod>();

    public void FixedUpdate()
    {
        CurrentValue = baseValue;

        for (int i = 0; i < mods.Count; i++)
        {
            mods[i].FixedUpdate();

            if (mods[i].TimeUp)
            {
                mods.Remove(mods[i]);
            }
            else
            {
                if (mods[i].isAdditive)
                {
                    CurrentValue += mods[i].modValue;
                }
            }
        }

        for (int i = 0; i < mods.Count; i++)
        {
            if (mods[i].isMultiplicative)
            {
                CurrentValue *= mods[i].modValue;
            }
        }
    }

    public void AddMod(StatMod mod)
    {
        mods.Add(mod);
    }

    public void AddToBaseValue(float value)
    {
        baseValue += value;

        if (EStatBaseValueChanged != null) EStatBaseValueChanged(baseValue);
    }
}

[System.Serializable]
public class StatMod
{
    public float modValue;
    public bool isAdditive;
    public bool isMultiplicative;
    public int durationInFrames;
    private int timer;

    public bool TimeUp { get { return timer <= 0; } }

    public void Apply()
    {
        timer = durationInFrames;
    }

    public void FixedUpdate()
    {
        timer--;
    }
}


[System.Serializable]
public class StatChange
{
    public string statName;
    public float addValue;

    public void ExecuteStatChange(Character chr)
    {
        //stats.FindStat(statName).AddToBaseValue(addValue);
        chr.GetStat(statName).AddToBaseValue(addValue);
    }
}
