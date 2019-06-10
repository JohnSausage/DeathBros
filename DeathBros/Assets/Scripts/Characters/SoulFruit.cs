using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulFruit : MonoBehaviour, ICanTakeDamage
{
    public string animFruitFull;
    public string animFruitEmpty;

    public EFruitState fruitState = EFruitState.Full;

    public float rechargeTimeInSeconds = 5;

    public int soulBubbleAmount = 10;

    public enum EFruitState { None, Full, Empty };

    protected FrameAnimator fanim;

    protected void Start()
    {
        fanim = GetComponent<FrameAnimator>();
        fanim.ChangeAnimation(animFruitFull);
    }

    protected void FixedUpdate()
    {

    }

    public void GetHit(Damage damage)
    {
        if (fruitState == EFruitState.Full)
        {
            fruitState = EFruitState.Empty;
            fanim.ChangeAnimation(animFruitEmpty);

            EffectManager.SpawnSoulBubbles(soulBubbleAmount, transform.position);

            StartCoroutine(IRechargeFruit());
        }
    }


    protected IEnumerator IRechargeFruit()
    {
        yield return new WaitForSeconds(rechargeTimeInSeconds);
        fruitState = EFruitState.Full;
        fanim.ChangeAnimation(animFruitFull);
    }
}
