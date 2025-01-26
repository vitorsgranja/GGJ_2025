using System.Collections;
using UnityEngine;

public class StickEnemy : MonoBehaviour
{
    private Transform player;
    [SerializeField] private float speed;
    [SerializeField] private float life;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackCooldown;


    private bool isGrounded;
    private bool shouldJump;
    private SpriteRenderer sprite;
    bool canJump = true;
    private Vector2 playerPosition;
    private float distancePlayer;
    float maxSpeed = 0;
    private float tempTimer;
    private bool isAttacking = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        maxSpeed = speed;
        sprite = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = player.transform.position - transform.position;
        distancePlayer = Vector2.Distance(transform.position, player.transform.position);
        //is grounded?
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);

        //player direction
        float directrion = Mathf.Sign(player.position.x - transform.position.x);

        //player above direction
        bool isPlayerAbove = Physics2D.Raycast(transform.position, Vector2.up, 5f, 1 << player.gameObject.layer);

        if (isGrounded)
        {
            if (distancePlayer > attackDistance)
            {
                //chase player
                rb.linearVelocity = new Vector2(directrion * speed, rb.linearVelocity.y);
                animator.Play("Move");
                RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(directrion, 0), 2f, groundLayer);

                RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(directrion, 0, 0), Vector2.down, 2f, groundLayer);

                RaycastHit2D plataformAbove = Physics2D.Raycast(transform.position, Vector2.up, 5f, groundLayer);

                if (!groundInFront.collider && !gapAhead.collider)
                {
                    shouldJump = true;
                }
                else if (isPlayerAbove && plataformAbove.collider)
                {
                    shouldJump = true;
                }
            }
            else if (distancePlayer <= attackDistance)
            {
                tempTimer -= Time.deltaTime;
                if (tempTimer <= 0)
                {
                    Attack2();
                }
            }

        }
        FlipSprite();
    }



    private void FixedUpdate()
    {
        if (isGrounded && shouldJump && canJump)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            Vector2 jumpDirection = direction * jumpForce;

            rb.AddForce(new Vector2(jumpDirection.x, jumpForce), ForceMode2D.Impulse);
            canJump = false;
            StartCoroutine("EnableJumpAfterDelay");
        }
    }


    private void Attack()
    {
        isAttacking = true;
        animator.Play("Attack1");
    }

    private void Attack2()
    {
        isAttacking = true;
        animator.Play("Attack2");
    }

    public void ResetAttackTimer()
    {
        isAttacking = false;
        tempTimer = attackCooldown;
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


    private void OnTriggerExit2D(Collider2D collision)
    {
        isGrounded = false;
    }
    private IEnumerator EnableJumpAfterDelay()
    {
        yield return new WaitForSeconds(2.5f);
        canJump = true;

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
                speed -= 1f;
                if (speed <= 0)
                {
                    speed = 0;
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
            //player.GetComponent<CharacterController2D>().AddLife(5);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            player.GetComponent<CharacterController2D>().AddLife(-5);

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
        speed += 1;
        if (speed > maxSpeed)
        {
            speed = maxSpeed;
        }
    }

    private void Death()
    {
        Debug.Log("Death");
        this.gameObject.SetActive(false);
    }
}
