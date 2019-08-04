using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPickUp : AutoPickUpItem
{
    [SerializeField]
    protected int skillCardIndex = 0;

    protected override void OnPickUp(Player player)
    {
        base.OnPickUp(player);

        player.AddSkillCard(skillCardIndex);
    }
}