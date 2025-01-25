using UnityEngine;

public class CharacterController2D : MonoBehaviour {
  [Header("Player Settings")]
  public float moveSpeed = 5f;
  public float jumpForce = 100f;
  public float jetpackForce = 10f;
  public float maxHealth = 100f;
  public float jetpackHealthConsumption = 10f;
  public float shootHealthConsumption = 5f;
  public Transform firePoint;
  public GameObject bubbleProjectile;

  [Header("Weapon Settings")]
  public GameObject[] weapons;

  private Rigidbody2D rb;
  private Animator animator;
  private float currentHealth;
  private bool isGrounded;
  private int currentWeaponIndex = 0;

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
      rb.linearVelocity = new Vector2(rb.linearVelocity.x,jumpForce);
    }

    if(Input.GetButton("Jump") && !isGrounded && currentHealth > 0) {
      rb.AddForce(Vector2.up * jetpackForce);
      ConsumeHealth(jetpackHealthConsumption * Time.deltaTime);
    }
  }

  private void HandleShooting() {
    if(Input.GetButtonDown("Fire1") && currentHealth > shootHealthConsumption) {
      Instantiate(bubbleProjectile,firePoint.position,firePoint.rotation);
      ConsumeHealth(shootHealthConsumption);
    }
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
    }
  }

  private void OnCollisionExit2D(Collision2D collision) {
    if(collision.gameObject.CompareTag("Ground")) {
      isGrounded = false;
    }
  }
}
