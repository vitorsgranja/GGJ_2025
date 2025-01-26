using UnityEngine;

public class BubbleFloating : MonoBehaviour
{
    [Header("Float Settings")]
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatHeight = 0.5f;
    [SerializeField] private float horizontalFloat = 0.3f;
    
    private Vector3 startPosition;
    private float randomOffset;

    private void Start()
    {
        startPosition = transform.localPosition;
        randomOffset = Random.Range(0f, 2f * Mathf.PI); // Offset aleatório para cada bolha
    }

    private void Update()
    {
        float verticalOffset = Mathf.Sin((Time.time + randomOffset) * floatSpeed) * floatHeight;
        float horizontalOffset = Mathf.Cos((Time.time + randomOffset) * floatSpeed * 0.6f) * horizontalFloat;
        
        transform.localPosition = startPosition + new Vector3(horizontalOffset, verticalOffset, 0);
    }
} 