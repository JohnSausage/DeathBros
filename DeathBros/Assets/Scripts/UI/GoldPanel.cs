using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldPanel : MonoBehaviour
{
    [SerializeField]
    protected SpriteFontText goldText;

    private void Start()
    {
        Player player = (Player)FindObjectOfType(typeof(Player));

        player.APlayerGoldUpdate += UpdateGoldText;

        UpdateGoldText(player.Gold);
    }

    private void UpdateGoldText(int goldAmount)
    {
        goldText.SetText(goldAmount.ToString());
    }
}
