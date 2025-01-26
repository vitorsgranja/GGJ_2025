using System.Text.RegularExpressions;
using System;
using UnityEngine;

public class HookEnemy : MonoBehaviour
{
    [SerializeField] private float life;
    [SerializeField] private float speed;
    [SerializeField] private float attackDistance=0;
    [SerializeField] private float attackCooldown;


    private Rigidbody2D enemyRB;
    private Vector2 direction;
    private GameObject player;
    private Animator HookAnima;

    private BoidController boid;
    private float distancePlayer;
    
    

    private void Start()
    {
        
        enemyRB = this.GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        boid = GetComponent<BoidController>();
        HookAnima = GetComponent<Animator>();
    }
    private void Update()
    {
        distancePlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distancePlayer > attackDistance)
        {
            boid.TargetPlayer();
            boid.Seek();s
            HookAnima.SetBool("IsMove", true);
        }
        else
        {
             HookAnima.SetBool("IsMove", false);
            Attack();
        }
        
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
}
