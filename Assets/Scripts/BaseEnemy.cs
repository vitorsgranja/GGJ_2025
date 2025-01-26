using System.Collections;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{

    [SerializeField] private float life;
    [SerializeField] private float speed;
    [SerializeField] private float wakeUpDistance;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackTimer;


    [Space(10)]
    [SerializeField] private float hitWallDistance;
    [SerializeField] private float hitHoleDistance;
    [SerializeField] private float hitHoleDistanceFromPlayer;
    [SerializeField] private LayerMask wallLayer;

    private Rigidbody2D rb;
    private bool moveRight = true;
    private Vector3 playerPosition;
    private float distancePlayer;
    private GameObject player;
    private Animator animator;
    private SpriteRenderer sprite;



    private bool wokeUp = false;
    private bool isSleeping = true;
    private bool isAttacking = false;
    private float maxSpeed;
    float tempTimer;
    private Collider2D collider;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        tempTimer = attackTimer;
        maxSpeed = speed;
    }
    private void Update()
    {
        playerPosition = player.transform.position - transform.position;
        distancePlayer = Vector2.Distance(transform.position, player.transform.position);
        if (tempTimer > 0)
        {
            tempTimer -= Time.deltaTime;
        }
        if (distancePlayer <= attackDistance && tempTimer <= 0)
        {
            isAttacking = true;
            animator.Play("Attack");
        }
    }
    private void FixedUpdate()
    {
        // if (wokeUp && !isAttacking)
        // {
        //     rb.linearVelocity = Vector2.right * speed;
        // }
    }

    public void Attack()
    {
        collider.enabled = true;
        
    }
    public void ResetAtack()
    {
        animator.Play("Sleep");
        isAttacking = false;
        collider.enabled = false;
        tempTimer = attackTimer;
    }


    private void TurnAroundHitWall()
    {
        if (moveRight)
        {
            RaycastHit2D wallInFront = Physics2D.Raycast(this.transform.position, Vector2.right, hitWallDistance, wallLayer);
            Debug.DrawRay(this.transform.position, Vector2.right, Color.red);
            if (wallInFront.collider != null)
            {
                sprite.flipX = false;
                moveRight = false;
                speed *= -1;
            }
        }
        else
        {
            RaycastHit2D wallInFront = Physics2D.Raycast(this.transform.position, Vector2.left, hitWallDistance, wallLayer);
            Debug.DrawRay(this.transform.position, Vector2.left, Color.red);
            if (wallInFront.collider != null)
            {
                sprite.flipX = true;
                moveRight = true;
                speed *= -1;
            }
        }
    }

    private void TurnAroundDetectHole()
    {
        if (moveRight)
        {
            RaycastHit2D holeInFront = Physics2D.Raycast(new Vector2(this.transform.position.x + hitHoleDistanceFromPlayer, this.transform.position.y), -Vector2.up, hitHoleDistance, wallLayer);
            Debug.DrawRay(new Vector2(transform.position.x + hitHoleDistanceFromPlayer, this.transform.position.y), Vector2.down, Color.red);
            if (holeInFront.collider == null)
            {
                sprite.flipX = false;
                moveRight = false;
                speed *= -1;
            }
        }
        else
        {
            RaycastHit2D holeInFront = Physics2D.Raycast(new Vector2(this.transform.position.x - hitHoleDistanceFromPlayer, this.transform.position.y), -Vector2.up, hitHoleDistance, wallLayer);
            Debug.DrawRay(new Vector2(transform.position.x - hitHoleDistanceFromPlayer, this.transform.position.y), Vector2.down, Color.red);
            if (holeInFront.collider == null)
            {
                sprite.flipX = true;
                moveRight = true;
                speed *= -1;
            }
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.GetComponent<CharacterController2D>().AddLife(-10);
        }
    }

}
