using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    public GameObject player;
    public Vector3 target;
    public Vector2 target2D;

    public float max_speed;
    public float max_force;
    public float mass;

    public Vector3 vel;
    private Vector2 vel2D;

    private float distancerayRightHit;
    private float distancerayLeftHit;
    private float obstaclevoidForce = 0;
    int myLayerMask;
    Vector2 desired_vel, steering;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform.position;
        vel2D = new Vector2(0, 0);
        myLayerMask = LayerMask.GetMask("Default");
        myLayerMask += LayerMask.GetMask("Hole - DuringJump");

    }
    public void TargetPlayer()
    {
        target = player.transform.position;
    }
    public void Seek()
    {
        Vector2 middleTarget = (target - transform.position).normalized;
        //Debug.DrawRay(transform.position, middleTarget * 5, Color.red);
        Vector2 tempTarget = new Vector3(middleTarget.y, -middleTarget.x, 0);
        Vector2 rightTarget = (2 * middleTarget + tempTarget).normalized;
        //Debug.DrawRay(transform.position, rightTarget * 6, Color.magenta);
        Vector2 leftTarget = (2 * middleTarget - tempTarget).normalized;
        // Debug.DrawRay(transform.position, leftTarget * 6, Color.blue);
        AvoidWall(rightTarget, middleTarget, leftTarget);

        desired_vel = (target - transform.position).normalized;
        desired_vel = (desired_vel + tempTarget.normalized * obstaclevoidForce).normalized;
        desired_vel = desired_vel * max_speed;
        steering = desired_vel - vel2D;
        steering = Vector2.ClampMagnitude(steering, max_force);
        steering = steering / mass;

        vel2D = Vector2.ClampMagnitude(vel2D + steering, max_speed);
        vel = vel2D;

        transform.position = transform.position + vel * Time.deltaTime;
        transform.position += vel.normalized * Time.deltaTime;
    }
    void AvoidWall(Vector2 rightTarget, Vector2 middleTarget, Vector2 leftTarget)
    {
        RaycastHit2D rayRight, rayMiddle, rayLeft;
        rayRight = Physics2D.Raycast(transform.position, rightTarget, 6, myLayerMask);      //rayvast da direita
        rayMiddle = Physics2D.Raycast(transform.position, middleTarget, 5, myLayerMask);  // raycast do meio
        rayLeft = Physics2D.Raycast(transform.position, leftTarget, 6, myLayerMask);     //raycast da esquerda  
        RaycastHit2D hitRight, hitMiddle, hitLeft;

        obstaclevoidForce = 0;

        hitRight = rayRight;
        if (hitRight)
        {
            distancerayRightHit = rayRight.distance;

        }
        else
        {
            distancerayRightHit = 999;
        }
        hitLeft = rayLeft;
        if (hitLeft)
        {
            distancerayLeftHit = rayLeft.distance;
        }
        else
        {
            distancerayLeftHit = 999;
        }
        hitMiddle = rayMiddle;
        if (hitMiddle)
        {
            if (distancerayRightHit > distancerayLeftHit * 1.05f)
            {
                obstaclevoidForce = (1 / distancerayLeftHit) * 5;
            }
            else if (distancerayLeftHit > distancerayRightHit * 1.05f)
            {
                obstaclevoidForce = -(1 / distancerayRightHit) * 5;
            }
        }

    }
}