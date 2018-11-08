using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : _MB
{
    private GameObject hitEffect1;
    private GameObject damageNumber;
    private GameObject cloudEffect;

    [SerializeField]
    private List<GameObject> effectGOs;

    public static EffectManager Instance { get; private set; }

    private void Awake()
    {
        base.Init();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        Object[] goArray = Resources.LoadAll("Effects");

        effectGOs = new List<GameObject>();

        for (int i = 0; i < goArray.Length; i++)
        {
            GameObject arrayGO = (GameObject)goArray[i];
            effectGOs.Add(arrayGO);
        }

        hitEffect1 = (GameObject)Resources.Load("Effects/Hit1");
        damageNumber = (GameObject)Resources.Load("Effects/DamageNumber");

        Character.TakesDamageAll += SpawnDamageNumber;
        Character.TakesDamageAll += SpawnHitEffect;
    }



    private void SpawnHitEffect(Damage damage, Vector2 position)
    {
        Instantiate(hitEffect1, damage.HitPosition, Quaternion.identity);

        //GameObject effect = Instantiate(hitEffect1, position, Quaternion.identity);
        //effect.GetComponent<Effect>().color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
    }

    private void SpawnDamageNumber(Damage damage, Vector2 position)
    {
        GameObject dmgNr = Instantiate(damageNumber, position + Vector2.up, Quaternion.identity);
        dmgNr.GetComponent<DamageNumber>().damageNumber = damage.damageNumber.ToString();

    }

    /*
    private void SpawnCloudEffect(Character chr)
    {
        Instantiate(cloudEffect, (Vector2)chr.transform.position + Vector2.down, Quaternion.identity);
    }
    */

    public static void SpawnEffect(string effectName, Vector2 position)
    {
        GameObject spawnGO = Instance.effectGOs.Find(x => x.name == effectName).gameObject;

        if (spawnGO != null)
            Instantiate(spawnGO, position, Quaternion.identity);
        else
            Debug.Log(spawnGO);
    }
}
