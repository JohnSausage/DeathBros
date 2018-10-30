using UnityEngine;
using TMPro;

public class ComboPanel : MonoBehaviour
{
    public TextMeshProUGUI tmProText;
    public RectTransform timerPanel;

    public string Text { get { return tmProText.text; } set { tmProText.text = value; } }
    public Color TextColor { get { return tmProText.color; } set { tmProText.color = value; } }

    public int duration = 120;
    private int timer = 0;

    private float timerPanelStartWidth;

    public bool TimeUp { get { return timer > duration; } }

    private void Start()
    {
        timerPanelStartWidth = timerPanel.sizeDelta.x;
    }
    private void FixedUpdate()
    {
        timer++;

        timerPanel.sizeDelta = new Vector2(timerPanelStartWidth * (duration - timer) / duration, timerPanel.sizeDelta.y);

        if (TimeUp)
            Destroy(gameObject);
    }
}