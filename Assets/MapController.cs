using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    private const float MAP_LIFE_SECONDS = 20f;
    private const float MAP_SIZE_DIVIDER = 6;
    private const int MAP_BORDER_STEP = 314;
    private const float MAP_MIN_LIFE = 60f;
    private const int VALVE_LIFE = 10;
    private const int MAX_TIME_UNTIL_NEXT_BUBBLE_SPAWNER = 20;
    private const int MIN_TIME_UNTIL_NEXT_BUBBLE_SPAWNER = 5;
    private const float MAX_MAP_LIFE = 100f;
    private const float ASCENSION_SPEED = 0.003f;

    public LineRenderer mapBorderLineRenderer;
    public float mapLife = MAX_MAP_LIFE;
    public float time;
    public int currentValveLife = VALVE_LIFE;
    public List<Vector3> bubbleSpawnPoints;

    public Object bubbleSpawnerPrefab;

    private float timeRemaining = MAP_LIFE_SECONDS;
    private bool isAscending = false;
    private List<Vector2> colliderPath = new List<Vector2>();
    public Camera cameraObject;

    public float remainingTimeUntilNextBubbleSpawner;
    public LineCollision2 lineCollision;

    private SpriteRenderer sprite = null;
    private bool isBordersDrawn = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DrawBorders();
        SetRemainingTimeBubbleSpawner();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeRemaining > 0f)
        {
            var deltaTime = Time.deltaTime;
            timeRemaining -= deltaTime;
            time = timeRemaining;
            UpdateMapLife(-100*deltaTime / MAP_LIFE_SECONDS);
        }
        else
		{
            Ascend();
        }

        if (remainingTimeUntilNextBubbleSpawner > 0f)
		{
            remainingTimeUntilNextBubbleSpawner -= Time.deltaTime;
		}
        else
		{
            SpawnBubbleSpawner();
            SetRemainingTimeBubbleSpawner();
		}

        if (isAscending)
		{
            transform.position = new Vector3(transform.position.x, transform.position.y + ASCENSION_SPEED);
			if (transform.position.y >= 9 && !TryGetComponent<Camera>(out _))
			{
                cameraObject.transform.parent = transform;
			}

            DrawBorders();
		}
    }

    void UpdateMapLife(float value)
	{
        mapLife = Mathf.Max(MAP_MIN_LIFE, mapLife += value);
        DrawBorders();
	}

    void DrawBorders()
	{
        var radius = GetRadius();

        colliderPath.Clear();

        mapBorderLineRenderer.positionCount = MAP_BORDER_STEP + 1;
        int currentStep;
        Vector3 firstPosition = new Vector3(0, 0, 0);

        float maxY = 0f, maxX = 0f, minY = 0f, minX = 0f;

        for (currentStep = 0; currentStep < MAP_BORDER_STEP; currentStep++)
		{
            float borderProgress = (float) currentStep / MAP_BORDER_STEP;

            float currentRadiant = borderProgress * 2 * Mathf.PI;

            float xScaled = Mathf.Cos(currentRadiant);
            float yScaled = Mathf.Sin(currentRadiant);

            float x = xScaled * radius;
            float y = yScaled * radius;

            colliderPath.Add(new Vector2(transform.position.x + x, transform.position.y + y));
            maxY = Mathf.Max(maxY, y);
            Vector3 currentPosition = new Vector3(transform.position.x + x, transform.position.y + y/* + (MAX_MAP_LIFE - mapLife) / 6f*/, 0);
            if (currentStep == 0)
            {
                firstPosition = currentPosition;
                minX = maxX = currentPosition.x;
                minY = maxY = currentPosition.y;
            }
            else
            {
                minX = Mathf.Min(minX, currentPosition.x);
                maxX = Mathf.Max(maxX, currentPosition.x);
                minY = Mathf.Min(minY, currentPosition.y);
                maxY = Mathf.Min(maxY, currentPosition.y);
            }

            mapBorderLineRenderer.SetPosition(currentStep, currentPosition);
		}

        mapBorderLineRenderer.SetPosition(currentStep, firstPosition + new Vector3(mapBorderLineRenderer.startWidth / 2, 0, 0));

        if (!isAscending)
        {
            if (sprite != null)
            {
                var currentSize = sprite.bounds.size.x;

                var goalSize = mapBorderLineRenderer.bounds.size.x;
                var newSpriteScale = goalSize / currentSize;
            
                sprite.transform.localScale *= newSpriteScale+0.08f;

                lineCollision.transform.localScale = new Vector3(mapLife / MAX_MAP_LIFE, mapLife / MAX_MAP_LIFE);
            }
        }

        if (!isBordersDrawn)
		{
            lineCollision.UpdateCollision();
		}

        isBordersDrawn = true;
    }

    void DamageValve()
	{
        currentValveLife--;
        if (currentValveLife <= 0)
		{
            Ascend();
		}
	}

    private void Ascend()
	{
        // Start bubble ascension
        isAscending = true;
    }

    private float GetRadius()
	{
        return mapLife / MAP_SIZE_DIVIDER;
	}

    private Vector3 GetRandomSpawnPointOnFloor()
	{
        var xEnd = Random.Range(0, 2 * GetRadius() + 1f) - GetRadius();
        return new Vector3(xEnd, 0f);
	}

    public Vector3[] GetPositions()
	{
        Vector3[] positions = new Vector3[MAP_BORDER_STEP + 1];
        mapBorderLineRenderer.GetPositions(positions);
        return positions;
	}

    public float GetWidth()
	{
        return mapBorderLineRenderer.startWidth;
	}

    private void SpawnBubbleSpawner()
	{
        Instantiate(bubbleSpawnerPrefab, GetRandomSpawnPointOnFloor(), new Quaternion());
    }

    private void SetRemainingTimeBubbleSpawner()
	{
        remainingTimeUntilNextBubbleSpawner = Random.Range(MIN_TIME_UNTIL_NEXT_BUBBLE_SPAWNER, MAX_TIME_UNTIL_NEXT_BUBBLE_SPAWNER);
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// TODO: TREAT COLLISION
	}
}
