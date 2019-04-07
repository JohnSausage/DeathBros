using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectSO : ScriptableObject
{
    public string cardName;

    public ECardColor color = ECardColor.White;

    public Sprite cardPicture;

    public virtual void AcitivateEffect(Character chr)
    {

    }

    public virtual void DeactivateEffect(Character chr)
    {

    }
}
