using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField]
    protected Transform HealthSlider;
    protected SpriteRenderer spr;
    protected SpriteRenderer sprHealthSlider;
    protected Enemy enemy;

    protected float originalScaleX;
    protected float originalPosX;

    protected int fadeOutTimer = 0;
    protected int fadeOutDuration = 180;

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        spr = GetComponent<SpriteRenderer>();
        sprHealthSlider = HealthSlider.GetComponent<SpriteRenderer>();

        originalScaleX = HealthSlider.localScale.x;
        originalPosX = HealthSlider.localPosition.x;

        enemy.ATakesDamage += UpdateHealthBar;
        SetHealthBarTransparency(0);
    }

    private void FixedUpdate()
    {
        if(fadeOutTimer > 0)
        {
            fadeOutTimer--;
        }

        if(fadeOutTimer < 100)
        {
            SetHealthBarTransparency(fadeOutTimer / 100f);
        }
    }

    protected void UpdateHealthBar(Damage damage)
    {
        HealthSlider.localScale = new Vector2(originalScaleX * enemy.HealthPercent, HealthSlider.localScale.y);
        HealthSlider.localPosition = new Vector2(-0.5f + enemy.HealthPercent / 2, HealthSlider.localPosition.y);

        DisplayHealthBar();
    }

    protected void DisplayHealthBar()
    {
        SetHealthBarTransparency(1);

        fadeOutTimer = fadeOutDuration;
    }

    protected void SetHealthBarTransparency(float a)
    {
        spr.color = spr.color.SetTransparency(a);
        sprHealthSlider.color = sprHealthSlider.color.SetTransparency(a);
    }
}
