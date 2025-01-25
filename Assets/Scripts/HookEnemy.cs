using UnityEngine;

public class HookEnemy : MonoBehaviour
{
    [SerializeField] private float life;
    [SerializeField] private float speed;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackCooldown;


    private Rigidbody2D enemyRB;
    private Vector2 direction;
    private GameObject player;

    private BoidController boid;
    private float distancePlayer;

    private void Start()
    {
        enemyRB = this.GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        boid = GetComponent<BoidController>();
    }
    private void Update()
    {
        distancePlayer = Vector2.Distance(transform.position, player.transform.position);
        boid.TargetPlayer();
        if (distancePlayer > attackDistance)
        {
            Chase();
        }
        else
        {
            Attack();
        }
    }

    private void FixedUpdate()
    {
        boid.Seek();
    }

    private void Attack()
    {
        Debug.Log("Attack");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        life--;
        if (life <= 0)
        {

        }
    }

    private void Death()
    {
        Debug.Log("Death");
        this.gameObject.SetActive(false);
    }

    private void Chase()
    {
        boid.Seek();
    }
}
