using UnityEngine;

public class BubbleSpawnController : MonoBehaviour
{
	private const float DELAY_START_SPAWN = 2;
	private const float DELAY_BUBBLES_GENERATION = 3;
	private const int MAX_BUBBLE_COUNT = 5;
	private const float INITIAL_SCALE = 0.5f;

	private float remainingTimeDelay = DELAY_START_SPAWN;
	private float remainingBubbleGenerationDelay = DELAY_BUBBLES_GENERATION;

	public int currentBubbleCount = 0;

	public Object bubblePrefab;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		// TODO: ANIMATION TO START
		transform.localScale = new Vector3(INITIAL_SCALE, INITIAL_SCALE);
	}

	// Update is called once per frame
	void Update()
	{
		if (remainingTimeDelay > 0f)
		{
			remainingTimeDelay -= Time.deltaTime;
			float newScale = (DELAY_START_SPAWN - remainingTimeDelay) / DELAY_START_SPAWN;
			transform.localScale = new Vector3(newScale, newScale);
		}
		else
		{
			if (currentBubbleCount < MAX_BUBBLE_COUNT)
			{
				if (remainingBubbleGenerationDelay > 0f)
				{
					remainingBubbleGenerationDelay -= Time.deltaTime;
				}
				else
				{
					GenerateBubbles();
				}
			}
		}
	}

	private void GenerateBubbles()
	{
		currentBubbleCount++;
		// TODO: CREATE BUBBLE
		Instantiate(bubblePrefab, transform.position, transform.rotation);
		if (currentBubbleCount >= MAX_BUBBLE_COUNT)
		{
			// TODO: DESTROY ANIMATION
			Destroy(gameObject);
		}

		remainingBubbleGenerationDelay = DELAY_BUBBLES_GENERATION;
	}
}
