using System.Text.RegularExpressions;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class HookEnemy : MonoBehaviour
{
    [SerializeField] private float life;
    [SerializeField] private float attackDistance = 0;
    [SerializeField] private float attackCooldown;

    private Rigidbody2D rb;
    private Vector2 direction;
    private GameObject player;
    private Animator HookAnima;

    private BoidController boid;
    private float distancePlayer;

    private float maxSpeed;
    private bool isAttacking = false;
    float tempTimer = 0;
    private SpriteRenderer sprite;
    private Vector2 playerPosition;
    private AudioManager audioManager;
    private void Start()
    {
        audioManager = AudioManager.instance;
        player = GameObject.FindGameObjectWithTag("Player");
        rb = this.GetComponent<Rigidbody2D>();
        boid = GetComponent<BoidController>();
        HookAnima = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        maxSpeed = boid.max_speed;
        tempTimer = attackCooldown;
    }
    private void Update()
    {
        distancePlayer = Vector2.Distance(transform.position, player.transform.position);
        playerPosition = player.transform.position - transform.position;

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
    private void OnTriggerEnter2D(Collider2D collision)
    {
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
