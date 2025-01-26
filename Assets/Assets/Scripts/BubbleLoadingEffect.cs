using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class BubbleLoadingEffect : MonoBehaviour
{
    [Header("Bolha Principal")]
    [SerializeField] private Image mainBubble;
    [SerializeField] private float minSize = 100f;
    [SerializeField] private float maxSize = 400f;
    [SerializeField] private float growthPerBubble = 5f;

    [Header("Mini Bolhas")]
    [SerializeField] private GameObject miniBubblePrefab;
    [SerializeField] private RectTransform spawnPoint;
    [SerializeField] private float spawnRate = 0.2f;
    [SerializeField] private float bubbleSpeed = 300f;
    [SerializeField] private float spawnWidth = 300f;

    [Header("Loading Text")]
    [SerializeField] private TMPro.TextMeshProUGUI loadingText;
    [SerializeField] private string loadingFormat = "{0}%";
    [SerializeField] private bool showDecimal = false;
    [SerializeField] private float minFontSize = 24f;
    [SerializeField] private float maxFontSize = 72f;

    [Header("Debug")]
    [SerializeField] private float simulatedLoadingTime = 5f;
    private float debugLoadingProgress = 0f;

    private float currentSize;
    private float targetSize;
    private bool isAnimating = false;
    private RectTransform mainBubbleRect;
    private Coroutine debugLoadingCoroutine;
    private float totalProgress = 0f;
    private float displayedProgress = 0f;

    private void Start()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        if (mainBubble != null)
        {
            mainBubbleRect = mainBubble.GetComponent<RectTransform>();
            
            if (!mainBubble.gameObject.GetComponent<CircleCollider2D>())
            {
                CircleCollider2D collider = mainBubble.gameObject.AddComponent<CircleCollider2D>();
                collider.isTrigger = true;
            }

            displayedProgress = 0f;
            currentSize = minSize;
            targetSize = minSize;
            SetBubbleSize(currentSize);
            mainBubble.color = new Color(1f, 1f, 1f, 1f);
            UpdateMainBubbleCollider();
            UpdateLoadingText(0f);
        }
    }

    private void UpdateMainBubbleCollider()
    {
        CircleCollider2D collider = mainBubble.GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            collider.radius = currentSize / 2f;
        }
    }

    [ContextMenu("Start Debug Loading")]
    public void StartDebugLoading()
    {
        Debug.Log("Iniciando Loading de Debug");
        ResetLoading();
        isAnimating = true;
        StartCoroutine(SpawnMiniBubbles());
        debugLoadingCoroutine = StartCoroutine(SimulateLoading());
    }

    [ContextMenu("Reset Loading")]
    public void ResetLoading()
    {
        Debug.Log("Resetando Loading");
        StopAllCoroutines();
        isAnimating = false;
        displayedProgress = 0f;
        currentSize = minSize;
        targetSize = minSize;
        SetBubbleSize(currentSize);
        UpdateLoadingText(0f);

        foreach (Transform child in transform)
        {
            if (child.gameObject != mainBubble.gameObject && 
                child.gameObject != loadingText.gameObject)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private IEnumerator SimulateLoading()
    {
        debugLoadingProgress = 0f;
        
        while (debugLoadingProgress < 1f)
        {
            debugLoadingProgress += Time.deltaTime / simulatedLoadingTime;
            UpdateProgress(debugLoadingProgress);
            yield return null;
        }

        Debug.Log("Loading Simulado Completo!");
        yield return new WaitForSeconds(1f);
        isAnimating = false;
    }

    private IEnumerator SpawnMiniBubbles()
    {
        Debug.Log("Iniciando spawn de bolhas");
        while (isAnimating)
        {
            SpawnMiniBubble();
            yield return new WaitForSeconds(spawnRate);
        }
    }

    private void SpawnMiniBubble()
    {
        if (spawnPoint == null || miniBubblePrefab == null) return;

        float randomX = Random.Range(-spawnWidth / 2f, spawnWidth / 2f);
        Vector2 spawnPosition = spawnPoint.anchoredPosition + new Vector2(randomX, 0);

        GameObject newBubble = Instantiate(miniBubblePrefab, transform);
        RectTransform rectTransform = newBubble.GetComponent<RectTransform>();
        
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = spawnPosition;

            // Adiciona componentes necessários para colisão
            if (!newBubble.GetComponent<CircleCollider2D>())
            {
                CircleCollider2D collider = newBubble.AddComponent<CircleCollider2D>();
                collider.isTrigger = true;
                collider.radius = rectTransform.rect.width / 2f;
            }

            if (!newBubble.GetComponent<Rigidbody2D>())
            {
                Rigidbody2D rb = newBubble.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0;
                rb.isKinematic = true;
            }

            // Adiciona o componente de movimento
            MiniBubbleMovement movement = newBubble.AddComponent<MiniBubbleMovement>();
            movement.Initialize(rectTransform, mainBubbleRect, bubbleSpeed);
        }
    }

    public void UpdateProgress(float progress)
    {
        totalProgress = Mathf.Clamp01(progress);
        targetSize = Mathf.Lerp(minSize, maxSize, totalProgress);
        Debug.Log($"Progress atualizado: {totalProgress * 100f}%");
    }

    private void UpdateLoadingText(float progress)
    {
        if (loadingText != null)
        {
            float percentage = progress * 100f;
            string text = showDecimal 
                ? string.Format(loadingFormat, percentage.ToString("F1"))
                : string.Format(loadingFormat, Mathf.RoundToInt(percentage));
            loadingText.text = text;

            float fontProgress = progress;
            float newFontSize = Mathf.Lerp(minFontSize, maxFontSize, fontProgress);
            loadingText.fontSize = newFontSize;
        }
    }

    private void SetBubbleSize(float size)
    {
        if (mainBubbleRect != null)
        {
            mainBubbleRect.sizeDelta = new Vector2(size, size);
            UpdateMainBubbleCollider();
        }
    }

    public void StopBubbleAnimation()
    {
        Debug.Log("Parando animação das bolhas");
        isAnimating = false;
        StopAllCoroutines();
        
        // Limpa todas as mini bolhas
        foreach (Transform child in transform)
        {
            if (child.gameObject != mainBubble.gameObject && child.gameObject != spawnPoint.gameObject)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void StartBubbleAnimation()
    {
        Debug.Log("Iniciando animação das bolhas");
        if (!IsSetupValid())
        {
            Debug.LogError("Setup não está válido para iniciar animação!");
            return;
        }

        isAnimating = true;
        StartCoroutine(SpawnMiniBubbles());
    }

    private bool IsSetupValid()
    {
        if (mainBubble == null)
        {
            Debug.LogError("Main Bubble não atribuída!");
            return false;
        }
        if (spawnPoint == null)
        {
            Debug.LogError("Spawn Point não atribuído!");
            return false;
        }
        if (miniBubblePrefab == null)
        {
            Debug.LogError("Mini Bubble Prefab não atribuído!");
            return false;
        }
        if (mainBubbleRect == null)
        {
            mainBubbleRect = mainBubble.GetComponent<RectTransform>();
            if (mainBubbleRect == null)
            {
                Debug.LogError("Main Bubble não tem RectTransform!");
                return false;
            }
        }
        return true;
    }

    public void GrowMainBubble()
    {
        float progressPerBubble = growthPerBubble / (maxSize - minSize);
        
        displayedProgress = Mathf.Min(displayedProgress + progressPerBubble, totalProgress);
        
        float newSize = Mathf.Lerp(minSize, maxSize, displayedProgress);
        currentSize = Mathf.Min(newSize, targetSize);
        
        SetBubbleSize(currentSize);
        UpdateLoadingText(displayedProgress);
        
        Debug.Log($"Progresso atual: {displayedProgress * 100f}%, Tamanho: {currentSize}");
    }
}

// Novo componente para controlar o movimento das mini bolhas
public class MiniBubbleMovement : MonoBehaviour
{
    private RectTransform myTransform;
    private RectTransform targetTransform;
    private float speed;
    private Vector2 startPosition;
    private float startTime;
    private Image myImage;
    private bool hasCollided = false;

    public void Initialize(RectTransform self, RectTransform target, float moveSpeed)
    {
        myTransform = self;
        targetTransform = target;
        speed = moveSpeed;
        startPosition = myTransform.anchoredPosition;
        startTime = Time.time;
        myImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (targetTransform == null || hasCollided) return;

        float journeyLength = Vector2.Distance(startPosition, targetTransform.anchoredPosition);
        float distanceCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distanceCovered / journeyLength;

        Vector2 currentPos = Vector2.Lerp(startPosition, targetTransform.anchoredPosition, fractionOfJourney);
        currentPos.x += Mathf.Sin(fractionOfJourney * 3f * Mathf.PI) * 30f;
        myTransform.anchoredPosition = currentPos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasCollided) return;
        
        if (other.gameObject.GetComponent<Image>() == null) return;

        BubbleLoadingEffect loadingEffect = transform.parent.GetComponent<BubbleLoadingEffect>();
        if (loadingEffect != null)
        {
            hasCollided = true;
            loadingEffect.GrowMainBubble(); // Chama o método de crescimento
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    private IEnumerator FadeOutAndDestroy()
    {
        float fadeOutDuration = 0.2f;
        float elapsedTime = 0f;
        Color startColor = myImage.color;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            myImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}