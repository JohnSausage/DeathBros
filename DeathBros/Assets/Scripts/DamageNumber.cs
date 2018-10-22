using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public string damageNumber { set { text.text = value; } }

    [SerializeField]
    private TextMeshProUGUI text;

    [SerializeField]
    private int frameDuration = 60;
    private float timer = 0;

    private void Update()
    {
        timer += Time.unscaledDeltaTime;

        if (timer > frameDuration / 120)
        {
            text.color = new Color(1, 1, 1, 1 - timer / (frameDuration / 60));
        }

        if (timer > frameDuration / 60)
        {
            Destroy(gameObject);
        }
    }
}
