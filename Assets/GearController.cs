using System.Collections;
using UnityEngine;

public class GearController : MonoBehaviour
{
    private const float GEAR_MAX_LIFE = 5f;

    public MapController mapController;

    private float gearCurrentLife = GEAR_MAX_LIFE;

    private Animator animator;

    private AudioManager audioManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        audioManager = AudioManager.instance;
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
        if (collision.transform.parent.TryGetComponent<PlayerProjectile>(out _) && !animator.GetBool("IsRoll"))
		{
            DamageGear(1);
            audioManager.PlaySound(audioManager.effectList[9]);
            animator.SetBool("IsRoll", true);

            StartCoroutine(nameof(CancelRoll));
		}
	}

    private IEnumerator CancelRoll()
    {
        yield return new WaitForSeconds(1f);
        animator.SetBool("IsRoll", false);
    }
}
