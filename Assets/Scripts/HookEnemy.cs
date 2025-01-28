using UnityEngine;
using System.Collections;

public class HookEnemy : BaseEnemyController
{
    private const float HOOK_DAMAGE = 5f;
    private const float KNOCKBACK_FORCE = 15f;

    public override float MeleeDamage => HOOK_DAMAGE;
    public override float KnockbackForce => KNOCKBACK_FORCE;

    [SerializeField] private float life;
    [SerializeField] private float attackDistance = 0;
    [SerializeField] private float attackCooldown;

    private Rigidbody2D rb;
    private Vector2 direction;
    private Animator HookAnima;

    private BoidController boid;
    private float distancePlayer;

    private float maxSpeed;
    private bool isAttacking = false;
    float tempTimer = 0;
    private SpriteRenderer sprite;
    private Vector2 playerPosition;
    private AudioManager audioManager;
    override protected void Start()
    {
        base.Start();
        audioManager = AudioManager.instance;
        rb = this.GetComponent<Rigidbody2D>();
        boid = GetComponent<BoidController>();
        HookAnima = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        maxSpeed = boid.max_speed;
        tempTimer = attackCooldown;
    }
    private void Update()
    {
        distancePlayer = Vector2.Distance(transform.position, player.position);
        playerPosition = player.position - transform.position;

        if (distancePlayer > attackDistance)
        {
            boid.TargetPlayer();
            boid.Seek();
            HookAnima.Play("Move");
            //HookAnima.SetBool("IsMove", true);
        }
        else
        {
            if (!isAttacking)
            {
                HookAnima.Play("Idle");
                tempTimer -= Time.deltaTime;
                if(tempTimer <= 0)
                {
                    Attack();
                }
            }
            //HookAnima.SetBool("IsMove", false);
        }
        FlipSprite();
    }

    private void FlipSprite()
    {
        if (playerPosition.x > 0)
        {
            sprite.flipX = true;
        }
        else
        {
            sprite.flipX = false;
        }
    }
    private void Attack()
    {
        isAttacking = true;
        HookAnima.Play("Attack");
    }

    public void ResetAttackTimer()
    {
        isAttacking = false;
        tempTimer = attackCooldown;
    }
    override protected void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.CompareTag("PlayerBullet"))
        {
            int playerProjectile = collision.transform.parent.GetComponent<PlayerProjectile>().weaponIndex;
            if (playerProjectile == 0)
            {
                rb.gravityScale -= 0.3f;
                StartCoroutine("BubbleEffectOff");

            }
            if (playerProjectile == 1)
            {
                boid.max_speed -= 1f;
                if (boid.max_speed <= 0)
                {
                    boid.max_speed = 0;
                }
                StartCoroutine("SlowEffectOff");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OutOfBound"))
        {
            this.gameObject.SetActive(false);
        }
    }
    private IEnumerator BubbleEffectOff()
    {
        yield return new WaitForSeconds(4);
        rb.gravityScale += 0.3f;
    }
    private IEnumerator SlowEffectOff()
    {
        yield return new WaitForSeconds(4);
        boid.max_speed += 1;
        if (boid.max_speed > maxSpeed)
        {
            boid.max_speed = maxSpeed;
        }
    }

    private void Death()
    {
        HookAnima.Play("Dead");
        this.gameObject.SetActive(false);
    }
}
