using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    public string damageNumber { set { text.SetText(value); } }

    public Color color { get; set; }

    [SerializeField]
    private SpriteFontText text;

    [SerializeField]
    private int frameDuration = 60;
    private float timer = 0;

    private void Awake()
    {
        color = Color.white;
    }
    private void Update()
    {
        timer += Time.unscaledDeltaTime;

        if (timer > frameDuration / 120)
        {
            text.color = new Color(color.r, color.g, color.b, 1 - timer / (frameDuration / 60));
            text.updateText = true;
        }

        if (timer > frameDuration / 60)
        {
            Destroy(gameObject);
        }
    }
}
