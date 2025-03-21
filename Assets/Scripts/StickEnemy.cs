
using System.Collections;
using UnityEngine;

public class StickEnemy : BaseEnemyController {
  private const float STICK_DAMAGE = 5f;
  private const float KNOCKBACK_FORCE = 15f;
  public override float MeleeDamage => STICK_DAMAGE;
  public override float KnockbackForce => KNOCKBACK_FORCE;

  [SerializeField] private float speed;
  [SerializeField] private float life;
  [SerializeField] private float jumpForce;
  [SerializeField] private LayerMask groundLayer;

  private Rigidbody2D rb;
  private bool isGrounded;
  private bool isGrounded1;
  private bool isGrounded2;
  private bool isGrounded3;
  private bool shouldJump;

  bool canJump = true;

  float maxSpeed = 0;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  override protected void Start() {
    base.Start();
    rb = GetComponent<Rigidbody2D>();
    maxSpeed = speed;
  }

  // Update is called once per frame
  void Update() {
    //is grounded?
    isGrounded1 = Physics2D.Raycast(transform.position,Vector2.down,1.15f,groundLayer);
    isGrounded2 = Physics2D.Raycast(transform.position,Vector2.down,1.15f,groundLayer);
    isGrounded3 = Physics2D.Raycast(transform.position,Vector2.down,1.15f,groundLayer);
    //Debug.DrawRay(transform.position + Vector3.left * 0.5f,Vector2.down * 1.15f,Color.red,1f);
    //Debug.DrawRay(transform.position + Vector3.right * 0.5f,Vector2.down * 1.15f,Color.blue,1f);
    if(isGrounded1 ||isGrounded2 || isGrounded3) {
      isGrounded = true;
    } else {
      isGrounded1=false;
    }

    //player direction
    float direction = Mathf.Sign(player.position.x - transform.position.x);

    //player above direction
    bool isPlayerAbove = Physics2D.Raycast(transform.position,Vector2.up,5f,1 << player.gameObject.layer);

    if(isGrounded) {
      //chase player
      rb.linearVelocity = new Vector2(direction * speed,rb.linearVelocity.y);

      RaycastHit2D groundInFront = Physics2D.Raycast(transform.position,new Vector2(direction,0),2f,groundLayer);

      RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(direction,0,0),Vector2.down,2f,groundLayer);

      RaycastHit2D plataformAbove = Physics2D.Raycast(transform.position,Vector2.up,5f,groundLayer);

      if(!groundInFront.collider && !gapAhead.collider) {
        shouldJump = true;
      } else if(isPlayerAbove && plataformAbove.collider) {
        shouldJump = true;
      }
    }
  }

  private void FixedUpdate() {
    if(isGrounded && shouldJump && canJump) {
      Vector2 direction = (player.position - transform.position).normalized;

      Vector2 jumpDirection = direction * jumpForce;

      rb.AddForce(new Vector2(jumpDirection.x,jumpForce),ForceMode2D.Impulse);
      canJump = false;
      StartCoroutine("EnableJumpAfterDelay");
    }
  }

  private void OnTriggerExit2D(Collider2D collision) {
    isGrounded = false;
  }
  private IEnumerator EnableJumpAfterDelay() {
    yield return new WaitForSeconds(2.5f);
    canJump = true;

  }

  override protected void OnTriggerEnter2D(Collider2D collision) {
    base.OnTriggerEnter2D(collision);
    if(collision.CompareTag("PlayerBullet")) {
      int playerProjectile = collision.transform.parent.GetComponent<PlayerProjectile>().weaponIndex;
      if(playerProjectile == 0) {
        rb.gravityScale -= 0.3f;
        StartCoroutine("BubbleEffectOff");

      }
      if(playerProjectile == 1) {
        speed -= 1f;
        if(speed <= 0) {
          speed = 0;
        }
        StartCoroutine("SlowEffectOff");
      }
    }
  }

  private void OnCollisionEnter2D(Collision2D collision) {
    if(collision.gameObject.CompareTag("OutOfBound")) {
      this.gameObject.SetActive(false);
    }

  }

  private IEnumerator BubbleEffectOff() {
    yield return new WaitForSeconds(10);
    rb.gravityScale = 1f;
  }
  private IEnumerator SlowEffectOff() {
    yield return new WaitForSeconds(4);
    speed += 1;
    if(speed > maxSpeed) {
      speed = maxSpeed;
    }
  }

  private void Death() {
    Debug.Log("Death");
    this.gameObject.SetActive(false);
  }
}
