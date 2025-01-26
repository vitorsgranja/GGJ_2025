using System;
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

  [Header("Dash Settings")]
  public float dashSpeed = 15f;
  public float dashDuration = 0.2f;
  public float dashCooldown = 1f;

  public GameMenu gameMenu;

  private Rigidbody2D rb;
  private AudioManager audioManager;
  private SpriteRenderer spriteRenderer;
  private float currentHealth;
  private bool isGrounded;
  private bool isJumping;
  private int currentWeaponIndex = 0;
  private float secondaryWeaponAmmo = 0;
  private bool canUseJetpack = false;
  private bool isDashing = false;
  private bool canDash = true;
  private bool invulnerability = false;

  private List<Queue<GameObject>> projectilePools = new List<Queue<GameObject>>();

  [Header("Animation")]
  private Animator playerAnima;
  private bool IsMove;
  private void Start() {
    audioManager = AudioManager.instance;
    rb = GetComponent<Rigidbody2D>();
    spriteRenderer = GetComponent<SpriteRenderer>();
    currentHealth = maxHealth;
    UpdatePlayerSize();
    InitializeProjectilePools();
    UpdatePlayerColor();

    playerAnima = GetComponent<Animator>();
  }

  private void Update() {
    if(!isDashing) {
      HandleMovement();
      HandleJumpAndJetpack();
    }
    HandleShooting();
    HandleWeaponSwitching();
    HandleDash();
    HandleMouseFunctions();
    UpdatePlayerSize();
    UpdatePlayerColor();
    playerAnima.SetFloat("xVelocity",Math.Abs(rb.linearVelocity.x));
    playerAnima.SetFloat("yVelocity",rb.linearVelocity.y);
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
      playerAnima.SetBool("IsJump",!isGrounded);
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
  }  private IEnumerator ResetInvulnerabilityState() {
    yield return new WaitForSeconds(1f);
    invulnerability = false;
  }

  private void HandleShooting() {
    if(Input.GetButtonDown("Fire1") && currentHealth > shootHealthConsumption) {
      if(currentWeaponIndex == 0 || secondaryWeaponAmmo > 0) {
        playerAnima.Play("Attack");
        audioManager.PlaySound(audioManager.effectList[1]);
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

  private void HandleDash() {
    if(Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing && rb.linearVelocityX != 0) {
      StartCoroutine(PerformDash());

    }

  }

  private IEnumerator PerformDash() {
    isDashing = true;
    canDash = false;

    float originalGravity = rb.gravityScale;
    rb.gravityScale = 0; // Disable gravity during dash
    Vector2 dashDirection = new Vector2(transform.localScale.x,0).normalized;

    this.GetComponent<TrailRenderer>().enabled = true;

    rb.linearVelocity = dashDirection * dashSpeed;
    yield return new WaitForSeconds(dashDuration);

    this.GetComponent<TrailRenderer>().enabled = false;

    rb.gravityScale = originalGravity;
    rb.linearVelocity = Vector2.zero;
    isDashing = false;

    yield return new WaitForSeconds(dashCooldown);
    canDash = true;

  }




  #region MousePosition
  private void HandleMouseFunctions() {
    // Captura a posi��o do mouse na tela
    Vector3 mousePosition = Input.mousePosition;
    mousePosition.z = 10f; // Ajuste a posi��o Z para um valor que esteja no plano 2D correto
                           // Converte a posi��o do mouse para o espa�o mundial
    Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
    FirePointRotation(worldMousePosition);
    PlayerSpritFlipping(worldMousePosition);
  }

  private void FirePointRotation(Vector3 mousePos) {
    // Calcula a dire��o entre o firePoint e o mouse
    Vector2 direction = (mousePos - firePoint.position).normalized;
    // Calcula o �ngulo da dire��o e aplica a rota��o no firePoint
    float angle = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
    firePoint.rotation = Quaternion.Euler(new Vector3(0,0,angle));
  }

  private void PlayerSpritFlipping(Vector3 mousePos) {
    // Verifica a posi��o do mouse em rela��o ao firePoint (ou ao personagem)
    if(mousePos.x < transform.position.x) {
      // Se o mouse estiver � esquerda do personagem, inverte o sprite
      spriteRenderer.flipX = true;
      // Se o mouse estiver � esquerda do personagem, inverte o firePoint
      firePoint.transform.localPosition = new Vector3(-0.1f,-0.043f,0);
    } else {
      // Se o mouse estiver � direita, o sprite n�o � invertido
      spriteRenderer.flipX = false;
      // Se o mouse estiver � esquerda do personagem, inverte o firePoint
      firePoint.transform.localPosition = new Vector3(0.1f,-0.043f,0);

    }
  }
  #endregion
  public void ConsumeHealth(float amount) {
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
      TailColor(baseColor);
    } else {
      float intensity = Mathf.Clamp01(secondaryWeaponAmmo / 10f);
      spriteRenderer.color = Color.Lerp(baseColor,secondaryWeaponColor,intensity);
      TailColor(Color.Lerp(baseColor,secondaryWeaponColor,intensity));
    }


  }
  public void TailColor(Color ColorTail) {
    TrailRenderer trail = GetComponent<TrailRenderer>();
    if(trail != null) {
      // Criar um gradiente para o TrailRenderer
      Gradient gradient = new Gradient();
      gradient.SetKeys(
          new GradientColorKey[] { new GradientColorKey(ColorTail,0.0f),new GradientColorKey(ColorTail,1.0f) },
          new GradientAlphaKey[] { new GradientAlphaKey(1.0f,0.0f),new GradientAlphaKey(0.0f,1.0f) }
        );

      // Atribuir o gradiente ao TrailRenderer
      trail.colorGradient = gradient;
    }
  }
  private void Die() {
    // Add logic for player death here
    audioManager.PlaySound(audioManager.effectList[2]);
    Debug.Log("Player died.");
    playerAnima.Play("Dead");
        gameMenu.Defeat();
    }

  public void AddLife(float life) {
    if(!invulnerability || life > 0) {
      currentHealth += life;
      if(currentHealth > maxHealth) {
        currentHealth = maxHealth;
      } else if(currentHealth <= 0) {
        Die();
      }
    }

    if(life < 0) {
      ResetInvulnerabilityState();
      invulnerability = true;
    }
  }
  private void OnTriggerEnter2D(Collider2D collision) {
    if(collision.CompareTag("Bubble")) {
      currentHealth += 10;
      currentHealth = Mathf.Clamp(currentHealth,0,maxHealth);
      Destroy(collision.gameObject);
    }

    if(collision.CompareTag("Enemy")) {
      //ConsumeHealth(20);
      audioManager.PlaySound(audioManager.effectList[3]);
    }

    if(collision.CompareTag("WeaponPickup")) {
      secondaryWeaponAmmo += 10; // Add ammo to the current pool
      Destroy(collision.gameObject);
    }
  }

  private void OnCollisionStay2D(Collision2D collision) {
    if(collision.gameObject.CompareTag("Ground") && rb.linearVelocity.y <= 0) {
      isGrounded = true;
      playerAnima.SetBool("IsJump",!isGrounded);
      canUseJetpack = false; // Disable jetpack when grounded
    }
  }

  private void OnCollisionExit2D(Collision2D collision) {
    if(collision.gameObject.CompareTag("Ground")) {
      isGrounded = false;
    }
  }
}
