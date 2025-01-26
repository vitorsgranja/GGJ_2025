using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class BubbleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private GameObject bubbleIcon;
    [SerializeField] private TextMeshProUGUI buttonText;
    
    [Header("Animation Settings")]
    [SerializeField] private float scaleTime = 0.3f;
    [SerializeField] private float hoverScale = 1.2f;
    [SerializeField] private bool useTextEffect = true;
    [SerializeField] private Ease scaleEaseIn = Ease.OutBack;
    [SerializeField] private Ease scaleEaseOut = Ease.InBack;

    [Header("Float Settings")]
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatAmplitude = 10f;
    [SerializeField] private float horizontalFloatAmplitude = 5f;

    [Header("Visibility Settings")]
    [SerializeField] private bool alwaysShowBubble = true;
    [Tooltip("If false, the bubble will only show on hover")]

    private Vector3 originalBubbleScale;
    private Vector3 originalBubblePosition;
    private MonoBehaviour textEffect;
    private bool isHovered;
    private Tween currentScaleTween;
    private Sequence floatSequence;

    private void Start()
    {
        // Inicializa as referências de escala e posição
        if (bubbleIcon != null)
        {
            originalBubbleScale = bubbleIcon.transform.localScale;
            originalBubblePosition = bubbleIcon.transform.localPosition;
            bubbleIcon.transform.localScale = originalBubbleScale;
            bubbleIcon.SetActive(alwaysShowBubble); // Usa a configuração do inspector
        }

        // Procura pelo componente BubbleTextEffect se estiver usando o efeito
        if (useTextEffect && buttonText != null)
        {
            textEffect = buttonText.GetComponent("BubbleTextEffect") as MonoBehaviour;
            if (textEffect != null)
            {
                textEffect.enabled = false;
            }
        }
    }

    private void OnDestroy()
    {
        // Limpa as tweens ao destruir o objeto
        currentScaleTween?.Kill();
        floatSequence?.Kill();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if (bubbleIcon != null)
        {
            if (!alwaysShowBubble)
            {
                bubbleIcon.SetActive(true);
            }

            currentScaleTween?.Kill();
            floatSequence?.Kill();
            
            bubbleIcon.transform.localPosition = originalBubblePosition;
            
            currentScaleTween = bubbleIcon.transform
                .DOScale(originalBubbleScale * hoverScale, scaleTime)
                .SetEase(scaleEaseIn);

            // Cria a animação de flutuação
            floatSequence = DOTween.Sequence();
            
            // Movimento vertical
            floatSequence.Join(
                bubbleIcon.transform
                    .DOLocalMoveY(originalBubblePosition.y + floatAmplitude, floatSpeed)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo)
            );
            
            // Movimento horizontal
            floatSequence.Join(
                bubbleIcon.transform
                    .DOLocalMoveX(originalBubblePosition.x + horizontalFloatAmplitude, floatSpeed * 1.5f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo)
            );
        }
        
        if (textEffect != null)
        {
            textEffect.enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (bubbleIcon != null)
        {
            currentScaleTween?.Kill();
            floatSequence?.Kill();
            
            currentScaleTween = bubbleIcon.transform
                .DOScale(originalBubbleScale, scaleTime)
                .SetEase(scaleEaseOut)
                .OnComplete(() => {
                    if (!isHovered)
                    {
                        bubbleIcon.transform.localPosition = originalBubblePosition;
                        if (!alwaysShowBubble)
                        {
                            bubbleIcon.SetActive(false);
                        }
                    }
                });
        }
        
        if (textEffect != null)
        {
            textEffect.enabled = false;
        }
    }

    public void OnClick()
    {
        Debug.Log("Button clicked");
    }
} 