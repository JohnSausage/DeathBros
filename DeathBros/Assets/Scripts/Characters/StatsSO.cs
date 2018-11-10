using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Characters/StatsSO")]
public class StatsSO : ScriptableObject
{
    public string charName;
    public List<Stat> stats;
}
