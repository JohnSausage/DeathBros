using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField]
    private Slider healthBar;

    void Start()
    {
        Player.PlayerHealthChanged += UpdatePlayerHealth;
    }

    void Update()
    {

    }

    private void UpdatePlayerHealth(float newValue)
    {
        healthBar.value = newValue;
    }
}
