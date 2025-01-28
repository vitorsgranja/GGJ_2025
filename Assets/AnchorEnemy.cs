using UnityEngine;

public class AnchorEnemy : BaseEnemyController
{
	private const float ANCHOR_DAMAGE = 30f;
	private const float KNOCKBACK_FORCE = 30f;
	public override float MeleeDamage => ANCHOR_DAMAGE;
	public override float KnockbackForce => KNOCKBACK_FORCE;
}
