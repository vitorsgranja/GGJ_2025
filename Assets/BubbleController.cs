using UnityEngine;

public class BubbleController : MonoBehaviour
{
    private const int MAX_LIFE_TIME = 40;
    private const float DY_MOVEMENT = 0.1f;
    private const float VALUE = 20f;

    private float currentLifeTime = MAX_LIFE_TIME;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentLifeTime > 0f)
        {
            currentLifeTime -= Time.deltaTime;
        }
        else
		{
            Destroy(gameObject);
		}

        transform.position = new Vector3(transform.position.x, transform.position.y + DY_MOVEMENT);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CharacterController2D player;
        if (collision.TryGetComponent(out player))
		{
            player.AddLife(VALUE);
            Destroy(gameObject);
        }
    }
}
