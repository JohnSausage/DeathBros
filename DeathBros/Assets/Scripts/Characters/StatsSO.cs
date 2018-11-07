using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Characters/StatsSO")]
public class StatsSO : ScriptableObject
{
    public List<Stat> stats;

    public void Init(List<Stat> statList)
    {
        for (int i = 0; i < stats.Count; i++)
        {
            statList.Add(stats[i]);
        }
    }
}
