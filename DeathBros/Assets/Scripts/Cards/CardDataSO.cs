using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/SkillCardData")]
public class CardDataSO : ScriptableObject
{
    public int cardID;

    [Space]
    [Space]

    public string title;
    public ECardColor cardColor;
    public Sprite picture;

    [Space]

    public int damage;
    public EStatusEffectType element;
    public int cost;
    public string type;

    [Space]

    [TextArea(3,5)]
    public string details;

    [Space]

    public StaticAttackStateSpecial attackStateSO;
}
