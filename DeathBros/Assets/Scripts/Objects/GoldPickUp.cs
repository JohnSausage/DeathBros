﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldPickUp : AutoPickUpItem
{
    [SerializeField]
    protected int goldAmount = 1;


    protected override void OnPickUp(Player player)
    {
        player.AddGold(1);
    }
}
