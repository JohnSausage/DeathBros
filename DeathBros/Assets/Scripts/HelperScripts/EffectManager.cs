using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : _MB
{
    [SerializeField]
    private Color colorHit;
    public static Color ColorHit { get { return Instance.colorHit; } }

    [SerializeField]
    private Color colorAttack;
    public static Color ColorAttack { get { return Instance.colorAttack; } }

    [SerializeField]
    private Color colorDefend;
    public static Color ColorDefend { get { return Instance.colorDefend; } }

    [SerializeField]
    private Material colorShaderMaterial;
    public static Material ColorShaderMaterial { get { return Instance.colorShaderMaterial; } }

    [SerializeField]
    private Material defaultMaterial;
    public static Material DefaultMaterial { get { return Instance.defaultMaterial; } }

    private GameObject hitEffect1;
    private GameObject damageNumber;
    private GameObject cloudEffect;

    [SerializeField]
    private List<GameObject> effectGOs;

    public static EffectManager Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(this);
        }

        Object[] goArray = Resources.LoadAll("Effects");

        effectGOs = new List<GameObject>();

        for (int i = 0; i < goArray.Length; i++)
        {
            GameObject arrayGO = (GameObject)goArray[i];
            effectGOs.Add(arrayGO);
        }

        hitEffect1 = (GameObject)Resources.Load("Effects/Hit1");
        damageNumber = (GameObject)Resources.Load("Effects/DamageNumber");
    }

    protected override void Start()
    {
        base.Start();
        Character.ATakesDamageAll += SpawnDamageNumber;
        Character.ATakesDamageAll += SpawnHitEffect;
    }



    private void SpawnHitEffect(Damage damage, Character chr)
    {
        Instantiate(hitEffect1, damage.HitPosition, Quaternion.identity);

        //GameObject effect = Instantiate(hitEffect1, position, Quaternion.identity);
        //effect.GetComponent<Effect>().color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
    }

    private void SpawnDamageNumber(Damage damage, Character chr)
    {
        GameObject dmgNr = Instantiate(damageNumber, chr.Position + Vector2.up, Quaternion.identity);
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

    public static void SpawnSoulBubbles(int amount, Vector2 position)
    {
        GameObject spawnGO = Instance.effectGOs.Find(x => x.name == "SoulBubble").gameObject;

        for (int i = 0; i < amount; i++)
        {
            Instantiate(spawnGO, position, Quaternion.identity);
        }
    }
}
