using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    public string statName;

    public StatMod statMod;

    public void AddBuff(Character chr)
    {
        chr.GetStat("statName").AddMod(statMod);
    }
}
