using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    public Stat movespeed = new Stat("Movespeed", 3);
    public Stat jumpStrength = new Stat("JumpStrength", 20);
    public Stat gravity = new Stat("Gravity", -1);
    public Stat jumps = new Stat("Jumps", 2);

    [Space]

    public float velocityXSmoothing = 3;
    public float maxSlopeUpAngle = 45;
    public float maxSlopeDownAngle = 45;

    public List<Stat> stats;

    public void Init()
    {
        stats.Add(movespeed);
        stats.Add(jumpStrength);
        stats.Add(gravity);
        stats.Add(jumps);
    }

    public void AddStat(Stat stat)
    {
        stats.Add(stat);
    }

    public void FixedUpdate()
    {
        for (int i = 0; i < stats.Count; i++)
        {
            stats[i].FixedUpdate();
        }
    }

    public Stat FindStat(string statName)
    {
        return stats.Find(x => x.statName == statName);
    }
}


[System.Serializable]
public class Stat
{
    public string statName;
    public float baseValue;

    public float CurrentValue { get; private set; }

    public Stat() { }

    public Stat(string statName, float baseValue)
    {
        this.statName = statName;
        this.baseValue = baseValue;
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

    public void Aplly()
    {
        timer = durationInFrames;
    }

    public void FixedUpdate()
    {
        timer--;
    }
}
