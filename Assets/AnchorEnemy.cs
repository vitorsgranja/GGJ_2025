using UnityEngine;

public class AnchorEnemy : MonoBehaviour
{
    private Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag(player.tag))
		{
            if (collision.TryGetComponent<CharacterController2D>(out var p))
			{
                p.AddLife(-30f);
			}
		}
	}
}
