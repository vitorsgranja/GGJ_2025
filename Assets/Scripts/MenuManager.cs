using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject menuButtons;
    
    [Header("Animation Settings")]
    [SerializeField] private float panelTransitionTime = 0.5f;
    [SerializeField] private Ease panelEaseIn = Ease.OutBack;
    [SerializeField] private Ease panelEaseOut = Ease.InBack;
    
    [Header("Menu Animation Settings")]
    [SerializeField] private float menuSlideDistance = 1000f; // Distância que o menu desce

    [Header("Options Bubble Settings")]
    [SerializeField] private float bubbleRiseDistance = 1000f;
    [SerializeField] private float bubbleScaleTime = 0.6f;
    [SerializeField] private float bubbleRiseTime = 1f;
    [SerializeField] private Ease bubbleEase = Ease.OutExpo;
    [SerializeField] private float bubbleWobbleStrength = 50f;
    
    private Vector3 optionsPanelOriginalPosition;
    private Vector3 mainMenuOriginalPosition;

    [Header("Game Settings")]
    [SerializeField] private string gameSceneName = "GameScene";

    private bool isAnimating = false; // Flag para controlar o estado da animação

    private void Start()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
            mainMenuPanel.transform.localScale = Vector3.one;
            mainMenuOriginalPosition = mainMenuPanel.transform.localPosition;
        }
        
        if (optionsPanel != null)
        {
            optionsPanelOriginalPosition = optionsPanel.transform.localPosition;
            optionsPanel.SetActive(false);
            optionsPanel.transform.localScale = Vector3.zero;
        }

        if (menuButtons != null)
        {
            menuButtons.transform.localScale = Vector3.one;
        }
    }

    public void StartGame()
    {
        // Anima o menu para baixo e diminui a escala
        mainMenuPanel.transform.DOLocalMoveY(mainMenuOriginalPosition.y - menuSlideDistance, panelTransitionTime)
            .SetEase(panelEaseOut);
        
        mainMenuPanel.transform.DOScale(0, panelTransitionTime)
            .SetEase(panelEaseOut)
            .OnComplete(() => {
                SceneManager.LoadScene(gameSceneName);
            });
    }

    public void OpenOptions()
    {
        if (isAnimating) return; // Previne múltiplos cliques durante a animação
        isAnimating = true;

        // Anima o menu principal para baixo
        Sequence menuExitSequence = DOTween.Sequence();

        menuExitSequence.Append(mainMenuPanel.transform
            .DOLocalMoveY(mainMenuOriginalPosition.y - menuSlideDistance, panelTransitionTime)
            .SetEase(panelEaseOut));

        menuExitSequence.Join(mainMenuPanel.transform
            .DOScale(0, panelTransitionTime)
            .SetEase(panelEaseOut));

        menuExitSequence.OnComplete(() => {
            mainMenuPanel.SetActive(false);
        });

        // Prepara e mostra o painel de opções
        optionsPanel.SetActive(true);
        optionsPanel.transform.localScale = Vector3.zero;
        optionsPanel.transform.localPosition = optionsPanelOriginalPosition - new Vector3(0, bubbleRiseDistance, 0);

        // Sequência de animação da bolha
        Sequence bubbleSequence = DOTween.Sequence();

        bubbleSequence.Append(optionsPanel.transform
            .DOLocalMoveY(optionsPanelOriginalPosition.y, bubbleRiseTime)
            .SetEase(bubbleEase));

        bubbleSequence.Join(optionsPanel.transform
            .DOLocalMoveX(optionsPanelOriginalPosition.x + Random.Range(-bubbleWobbleStrength, bubbleWobbleStrength), bubbleRiseTime)
            .SetEase(Ease.InOutSine));

        bubbleSequence.Join(optionsPanel.transform
            .DOScale(1, bubbleScaleTime)
            .SetEase(panelEaseIn));

        bubbleSequence.OnComplete(() => {
            isAnimating = false; // Libera para novas interações apenas quando a animação terminar
        });
    }

    public void CloseOptions()
    {
        if (isAnimating) return; // Previne múltiplos cliques durante a animação
        isAnimating = true;

        // Mostra e anima o menu principal de volta
        mainMenuPanel.SetActive(true);
        mainMenuPanel.transform.localScale = Vector3.zero;
        mainMenuPanel.transform.localPosition = mainMenuOriginalPosition - new Vector3(0, menuSlideDistance, 0);

        Sequence menuEnterSequence = DOTween.Sequence();
        
        menuEnterSequence.Append(mainMenuPanel.transform
            .DOLocalMove(mainMenuOriginalPosition, panelTransitionTime)
            .SetEase(panelEaseIn));
        
        menuEnterSequence.Join(mainMenuPanel.transform
            .DOScale(1, panelTransitionTime)
            .SetEase(panelEaseIn));

        // Anima o painel de opções para baixo
        Sequence bubbleSequence = DOTween.Sequence();

        bubbleSequence.Append(optionsPanel.transform
            .DOLocalMoveY(optionsPanelOriginalPosition.y - bubbleRiseDistance, bubbleRiseTime)
            .SetEase(Ease.InBack));

        bubbleSequence.Join(optionsPanel.transform
            .DOScale(0, bubbleScaleTime)
            .SetEase(panelEaseOut));

        bubbleSequence.OnComplete(() => {
            optionsPanel.SetActive(false);
            optionsPanel.transform.localPosition = optionsPanelOriginalPosition;
            isAnimating = false; // Libera para novas interações apenas quando a animação terminar
        });
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void OnDestroy()
    {
        if (mainMenuPanel != null) DOTween.Kill(mainMenuPanel.transform);
        if (optionsPanel != null) DOTween.Kill(optionsPanel.transform);
        if (menuButtons != null) DOTween.Kill(menuButtons.transform);
    }
} 