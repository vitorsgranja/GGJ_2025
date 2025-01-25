using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    [Header("Enemy stats")]
    [SerializeField] private float life;
    [SerializeField] private float speed;

    [Space(10)]

    [Header("Check variables")]
    [SerializeField] private float hitWallDistance;
    [SerializeField] private float hitHoleDistance;
    [SerializeField] private float hitHoleDistanceFromPlayer;
    [SerializeField] private LayerMask wallLayer;

    private Rigidbody2D enemyRB;
    private bool moveRight = true;

    private void Update()
    {
       
    }

    private void TurnAroundHitWall()
    {
        if (moveRight)
        {
            RaycastHit2D wallInFront = Physics2D.Raycast(this.transform.position, Vector2.right, hitWallDistance, wallLayer);
            Debug.DrawRay(this.transform.position, Vector2.right, Color.red);

            if (wallInFront.collider != null)
            {
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
                moveRight = true;
                speed *= -1;
            }
        }
    }
}
