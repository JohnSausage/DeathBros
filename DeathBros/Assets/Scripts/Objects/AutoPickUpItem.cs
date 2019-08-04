using UnityEngine;

public class AutoPickUpItem : MonoBehaviour, IAutoPickup
{
    [SerializeField]
    protected string spawnSound;

    [SerializeField]
    protected string pickUpSound;

    protected int waitTilPickUpTime = 30;
    private int timer = 0;

    protected SimpleController ctr;


    private void Awake()
    {
        ctr = GetComponent<SimpleController>();
    }


    private void Start()
    {
        AudioManager.PlaySound(spawnSound);
    }




    private void FixedUpdate()
    {
        if (timer < waitTilPickUpTime)
        {
            timer++;
        }
    }

    protected virtual void OnPickUp(Player player)
    {

    }


    public void GetPickedUp(Player player)
    {
        if (timer >= waitTilPickUpTime)
        {
            OnPickUp(player);

            AudioManager.PlaySound(pickUpSound);

            Destroy(gameObject);
        }
    }



    public void ApplyRandomVelocityUp()
    {
        Vector2 vel = new Vector2(Random.Range(-3f, 3f), Random.Range(12f, 15f));

        ctr.SetVelocity(vel);
    }



    public void Spawn(Vector2 spawnVelocity)
    {
        ApplyRandomVelocityUp();
    }
}


public interface IAutoPickup
{
    void GetPickedUp(Player player);
    void Spawn(Vector2 spawnVelocity);
}