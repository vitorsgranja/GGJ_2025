using UnityEngine;

public class GearController : MonoBehaviour
{
    private const float GEAR_MAX_LIFE = 5f;

    public MapController mapController;

    private float gearCurrentLife = GEAR_MAX_LIFE;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DamageGear(float damage)
	{
        print("Damage gear");
        gearCurrentLife -= damage;
        if (gearCurrentLife <= 0f)
		{
            mapController.RemoveGear(gameObject);
            Destroy(gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
        // TODO: CHANGE TO CORRECT WAY
        print("ontrigger " + collision);
        if (collision.transform.parent.TryGetComponent<PlayerProjectile>(out _))
		{
            DamageGear(1);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
        print("on collision");
    }
}
