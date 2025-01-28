using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DamageableEnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Vector3 defaultScale;
    private float defaultGravity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultScale = transform.localScale;
        rb = transform.parent.GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        defaultGravity = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaGravityScale = defaultGravity - rb.gravityScale;
        transform.localScale = defaultScale * Mathf.Min(1f, deltaGravityScale);
        if (rb.gravityScale < defaultGravity && !sprite.enabled)
		{
            sprite.enabled = true;
        }
        else if (rb.gravityScale >= defaultGravity && sprite.enabled)
		{
            sprite.enabled = false;
        }
    }
}
