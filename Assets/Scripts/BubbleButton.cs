using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class BubbleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI buttonText;
    
    [Header("Animation Settings")]
    [SerializeField] private bool useTextEffect = true;

    private MonoBehaviour textEffect;
    private bool isHovered;

    private void Start()
    {
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if (textEffect != null)
        {
            textEffect.enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (textEffect != null)
        {
            textEffect.enabled = false;
        }
    }

    public void OnClick()
    {
        
    }
} 