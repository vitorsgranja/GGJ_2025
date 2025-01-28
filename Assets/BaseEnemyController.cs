using UnityEngine;

public abstract class BaseEnemyController : MonoBehaviour
{
	protected Transform player;
	public virtual float Life { get; }
	public virtual float MeleeDamage { get; }
	public virtual float KnockbackForce { get; }
	//public EnemyProjectile projectile;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	protected virtual void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	protected virtual void OnCollisionStay2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag(player.tag))
		{
			if (collision.gameObject.transform.TryGetComponent<CharacterController2D>(out var p))
			{
				Vector2 directionNormal = new Vector2(0f, 0f);
				if (collision.gameObject.transform.position.x < transform.position.x)
				{
					directionNormal.x = 1f;
				}
				else
				{
					directionNormal.x = -1f;
				}

				if (collision.gameObject.transform.position.y < transform.position.y)
				{
					directionNormal.y = 0.02f;
				}
				else
				{
					directionNormal.y = -0.05f;
				}
				Vector2 knockback = directionNormal * KnockbackForce;
				p.ApplyDamage(MeleeDamage, knockback);
			}
		}
	}

	protected virtual void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag(player.tag))
		{
			if (collision.transform.parent != null && collision.transform.parent.TryGetComponent<CharacterController2D>(out var p))
			{
				Debug.Log("DAMAGE PLAYER " + MeleeDamage);
				Vector2 directionNormal = new Vector2(0f, 0f);
				if (collision.transform.position.x > transform.position.x)
				{
					directionNormal.x = 1f;
				}
				else
				{
					directionNormal.x = -1f;
				}
				Vector2 knockback = directionNormal * KnockbackForce;
				p.ApplyDamage(MeleeDamage, knockback);
			}
		}
	}
}
