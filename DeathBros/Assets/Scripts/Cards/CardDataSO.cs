using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/CardData")]
public class CardDataSO : ScriptableObject
{
    public string title;
    public ECardColor cardColor;
    public Sprite picture;

    [Space]

    public int level;
    public EStatusEffectType element;

    [Space]
    public string details;
}
