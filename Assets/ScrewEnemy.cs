using UnityEngine;

public class ScrewEnemy : BaseEnemyController
{
	private const float SCREW_DAMAGE = 10f;
	private const float KNOCKBACK_FORCE = 20f;
	public override float MeleeDamage => SCREW_DAMAGE;
	public override float KnockbackForce => KNOCKBACK_FORCE;
}
