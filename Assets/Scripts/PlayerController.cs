using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour {
  [Header("Player Settings")]
  public float moveSpeed = 5f;
  public float maxHealth = 100f;
  public float shootHealthConsumption = 5f;
  public Transform firePoint;
  public GameObject bubbleProjectile;

  [Header("Jump Settings")]
  public float initialJumpForce = 5;
  public float floatyGravityScale = 0.4f;
  public float fastFallMultiplier = 0.7f;

  [Header("Jetpack Settings")]
  public float jetpackForce = 0.3f;
  public float maxUpwardVelocity = 10f;
  public float jetpackHealthConsumption = 10f;

  [Header("Weapon Settings")]
  public GameObject[] weapons;

  private Rigidbody2D rb;
  private Animator animator;
  private float currentHealth;
  private bool isGrounded;
  private bool isJumping;
  private int currentWeaponIndex = 0;
  private bool canUseJetpack = false;


  private void Start() {
    rb = GetComponent<Rigidbody2D>();
    animator = GetComponent<Animator>();
    currentHealth = maxHealth;
    UpdatePlayerSize();
  }

  private void Update() {
    HandleMovement();
    HandleJumpAndJetpack();
    HandleShooting();
    HandleWeaponSwitching();
    UpdatePlayerSize();
  }

  private void HandleMovement() {
    float moveInput = Input.GetAxis("Horizontal");
    rb.linearVelocity = new Vector2(moveInput * moveSpeed,rb.linearVelocity.y);

    if(moveInput != 0) {
      transform.localScale = new Vector3(Mathf.Sign(moveInput),1,1);
    }
  }

  private void HandleJumpAndJetpack() {
    if(Input.GetButtonDown("Jump") && isGrounded) {
      isGrounded = false;
      isJumping = true;
      rb.linearVelocity = new Vector2(rb.linearVelocity.x,initialJumpForce);
      StartCoroutine(EnableJetpackAfterDelay());
    }

    if(isJumping) {
      if(rb.linearVelocity.y > 0) {
        rb.gravityScale = floatyGravityScale;
      } else if(rb.linearVelocity.y < 0) {
        rb.gravityScale = fastFallMultiplier;
        isJumping = false;
      }
    } else {
      rb.gravityScale = 0.5f; // Normal gravity scale
    }

    if(Input.GetButton("Jump") && canUseJetpack && currentHealth > 0) {
      Debug.Log("JETPACK!");
      rb.AddForce(Vector2.up * jetpackForce * rb.gravityScale);
      if(rb.linearVelocity.y > maxUpwardVelocity) {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x,maxUpwardVelocity);
      }
      ConsumeHealth(jetpackHealthConsumption * Time.deltaTime);
    }
  }
  private IEnumerator EnableJetpackAfterDelay() {
    yield return new WaitForSeconds(0.4f);
    if(!isGrounded) {
      canUseJetpack = true;
    }
  }
  private void HandleShooting() {
    //if(Input.GetButtonDown("Fire1") && currentHealth > shootHealthConsumption) {
    //  Instantiate(bubbleProjectile,firePoint.position,firePoint.rotation);
    //  ConsumeHealth(shootHealthConsumption);
    //}
  }

  private void HandleWeaponSwitching() {
    if(Input.GetKeyDown(KeyCode.Q)) {
      currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
      for(int i = 0; i < weapons.Length; i++) {
        weapons[i].SetActive(i == currentWeaponIndex);
      }
    }
  }

  private void ConsumeHealth(float amount) {
    currentHealth -= amount;
    currentHealth = Mathf.Clamp(currentHealth,0,maxHealth);
    if(currentHealth <= 0) {
      Die();
    }
  }

  private void UpdatePlayerSize() {
    float size = Mathf.Lerp(0.5f,1.5f,currentHealth / maxHealth);
    transform.localScale = new Vector3(size,size,1);
  }

  private void Die() {
    // Add logic for player death here
    Debug.Log("Player died.");
  }

  private void OnTriggerEnter2D(Collider2D collision) {
    if(collision.CompareTag("Bubble")) {
      currentHealth += 10;
      currentHealth = Mathf.Clamp(currentHealth,0,maxHealth);
      Destroy(collision.gameObject);
    }

    if(collision.CompareTag("Enemy")) {
      ConsumeHealth(20);
    }
  }

  private void OnCollisionStay2D(Collision2D collision) {
    if(collision.gameObject.CompareTag("Ground")) {
      isGrounded = true;
      canUseJetpack = false; // Disable jetpack when grounded
    }
  }

  private void OnCollisionExit2D(Collision2D collision) {
    if(collision.gameObject.CompareTag("Ground")) {
      isGrounded = false;
    }
  }
}
