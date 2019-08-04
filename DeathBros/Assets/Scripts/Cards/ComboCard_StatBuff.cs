
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/ComboCard: Stat buff")]
public class ComboCard_StatBuff : ComboCardDataSO
{
    public string statName;

    public float addAmount;

    public override void ApplyEffect(Player player)
    {
        base.ApplyEffect(player);

        player.GetStat(statName).AddToBaseValue(addAmount);
    }

    public override void RemoveEffect(Player player)
    {
        base.RemoveEffect(player);

        player.GetStat(statName).AddToBaseValue(-addAmount);
    }
}
