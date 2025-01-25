using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterController2D : MonoBehaviour {
  [Header("Player Settings")]
  public float moveSpeed = 5f;
  public float maxHealth = 100f;
  public float shootHealthConsumption = 5f;
  public Transform firePoint;
  public Transform projectileParent; // Parent for pooled projectiles
  public Color baseColor = Color.blue;
  public Color secondaryWeaponColor = Color.magenta;

  [Header("Jump Settings")]
  public float initialJumpForce = 5;
  public float floatyGravityScale = 0.4f;
  public float fastFallMultiplier = 0.7f;

  [Header("Jetpack Settings")]
  public float jetpackForce = 0.3f;
  public float maxUpwardVelocity = 10f;
  public float jetpackHealthConsumption = 10f;

  [Header("Weapon Settings")]
  public GameObject[] projectilePrefabs;
  public int poolSize = 10;

  private Rigidbody2D rb;
  private SpriteRenderer spriteRenderer;
  private float currentHealth;
  private bool isGrounded;
  private bool isJumping;
  private int currentWeaponIndex = 0;
  private float secondaryWeaponAmmo = 0;
  private bool canUseJetpack = false;

  private List<Queue<GameObject>> projectilePools = new List<Queue<GameObject>>();

  private void Start() {
    rb = GetComponent<Rigidbody2D>();
    spriteRenderer = GetComponent<SpriteRenderer>();
    currentHealth = maxHealth;
    UpdatePlayerSize();
    InitializeProjectilePools();
    UpdatePlayerColor();
  }

  private void Update() {
    HandleMovement();
    HandleJumpAndJetpack();
    HandleShooting();
    HandleWeaponSwitching();
    HandleFirePointRotation();
    UpdatePlayerSize();
    UpdatePlayerColor();
  }

  private void InitializeProjectilePools() {
    foreach(var prefab in projectilePrefabs) {
      Queue<GameObject> pool = new Queue<GameObject>();
      for(int i = 0; i < poolSize; i++) {
        GameObject obj = Instantiate(prefab,projectileParent);
        obj.SetActive(false);
        obj.GetComponent<PlayerProjectile>().Initialize(this); // Pass reference to controller
        pool.Enqueue(obj);
      }
      projectilePools.Add(pool);
    }
  }

  private GameObject GetPooledProjectile(int weaponIndex) {
    if(projectilePools[weaponIndex].Count > 0) {
      GameObject obj = projectilePools[weaponIndex].Dequeue();
      obj.SetActive(true);
      return obj;
    }

    // If pool is empty, create a new projectile
    GameObject newProjectile = Instantiate(projectilePrefabs[weaponIndex],projectileParent);
    newProjectile.GetComponent<PlayerProjectile>().Initialize(this); // Pass reference to controller
    return newProjectile;
  }

  public void ReturnProjectileToPool(GameObject projectile,int weaponIndex) {
    projectile.SetActive(false);
    projectilePools[weaponIndex].Enqueue(projectile);
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
    if(Input.GetButtonDown("Fire1") && currentHealth > shootHealthConsumption) {
      if(currentWeaponIndex == 0 || secondaryWeaponAmmo > 0) {
        GameObject projectile = GetPooledProjectile(currentWeaponIndex);
        projectile.transform.position = firePoint.position;
        projectile.transform.rotation = firePoint.rotation;
        projectile.GetComponent<Rigidbody2D>().linearVelocity = firePoint.right * 10f; // Example speed

        ConsumeHealth(shootHealthConsumption);
        if(currentWeaponIndex != 0) {
          secondaryWeaponAmmo -= 1;
          if(secondaryWeaponAmmo <= 0) {
            currentWeaponIndex = 0;
          }
        }
      }
    }
  }

  private void HandleWeaponSwitching() {
    if(Input.GetKeyDown(KeyCode.Q)) {
      if(currentWeaponIndex == 0 && secondaryWeaponAmmo > 0) {
        currentWeaponIndex = 1;
      } else if(currentWeaponIndex == 1) {
        currentWeaponIndex = 0;
      }
    }
  }

  private void HandleFirePointRotation() {
    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Vector2 direction = (mousePosition - transform.position).normalized;
    firePoint.right = direction;

    if(firePoint.localScale.x < 0) {
      firePoint.localScale = new Vector3(firePoint.localScale.x * -1,firePoint.localScale.y,firePoint.localScale.z);
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
    float size = Mathf.Lerp(2.5f,7.5f,currentHealth / maxHealth);
    transform.localScale = new Vector3(size,size,1);
  }

  private void UpdatePlayerColor() {
    if(currentWeaponIndex == 0) {
      spriteRenderer.color = baseColor;
    } else {
      float intensity = Mathf.Clamp01(secondaryWeaponAmmo / 10f);
      spriteRenderer.color = Color.Lerp(baseColor,secondaryWeaponColor,intensity);
    }
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

    if(collision.CompareTag("WeaponPickup")) {
      secondaryWeaponAmmo += 10; // Add ammo to the current pool
      Destroy(collision.gameObject);
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