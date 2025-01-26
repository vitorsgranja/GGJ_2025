using UnityEngine;

public class PlayerProjectile : MonoBehaviour {
  private CharacterController2D controller;
  public int weaponIndex;

  private Rigidbody2D rb;

  public void Initialize(CharacterController2D controllerRef) {
    controller = controllerRef;
    rb = GetComponent<Rigidbody2D>();
  }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player") && !collision.CompareTag("Ground"))
        {
            controller.ReturnProjectileToPool(gameObject,weaponIndex);
        }        
    }


  private void Update() {
    // Retorna o projétil se sair dos limites
    if(Mathf.Abs(transform.position.x) > 50 || Mathf.Abs(transform.position.y) > 200) {
      controller.ReturnProjectileToPool(gameObject,weaponIndex);
    }
  }

  private void FixedUpdate() {
    // Atualiza a rotação do projétil para apontar na direção do movimento
    if(rb != null && rb.linearVelocity.sqrMagnitude > 0.01f) { // Evita cálculos desnecessários para velocidades muito baixas
      float angle = Mathf.Atan2(rb.linearVelocity.y,rb.linearVelocity.x) * Mathf.Rad2Deg - 90f;
      transform.rotation = Quaternion.Euler(0,0,angle);
    }
  }
}