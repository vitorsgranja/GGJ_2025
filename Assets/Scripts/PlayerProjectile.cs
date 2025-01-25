using UnityEngine;

public class PlayerProjectile : MonoBehaviour {
  private CharacterController2D controller;
  public int weaponIndex;

  public void Initialize(CharacterController2D controllerRef) {
    controller = controllerRef;
  }

  private void OnCollisionEnter2D(Collision2D collision) {
    controller.ReturnProjectileToPool(gameObject,weaponIndex);
  }

  private void Update() {
    if(Mathf.Abs(transform.position.x) > 50 || Mathf.Abs(transform.position.y) > 50) {
      controller.ReturnProjectileToPool(gameObject,weaponIndex);
    }
  }
}
