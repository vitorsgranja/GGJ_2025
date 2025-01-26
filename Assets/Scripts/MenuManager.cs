using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject coral;
    [SerializeField] private GameObject menuButtons;
    
    [Header("Animation Settings")]
    [SerializeField] private float panelTransitionTime = 0.5f;
    [SerializeField] private float coralTransitionTime = 0.8f;
    [SerializeField] private Ease panelEaseIn = Ease.OutBack;
    [SerializeField] private Ease panelEaseOut = Ease.InBack;
    [SerializeField] private Ease coralEase = Ease.InOutQuart;
    
    [Header("Coral Animation Settings")]
    [SerializeField] private float coralRotation = -30f; // Rotação do coral ao sair
    [SerializeField] private float coralMoveX = 300f; // Distância que o coral move para a direita
    
    [Header("Game Settings")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("Options Bubble Settings")]
    [SerializeField] private float bubbleRiseDistance = 1000f; // Distância que a bolha sobe
    [SerializeField] private float bubbleScaleTime = 0.6f;
    [SerializeField] private float bubbleRiseTime = 1f;
    [SerializeField] private Ease bubbleEase = Ease.OutExpo;
    [SerializeField] private float bubbleWobbleStrength = 50f; // Força do movimento lateral
    
    private Vector3 coralOriginalPosition;
    private Quaternion coralOriginalRotation;
    private Vector3 optionsPanelOriginalPosition;
    private Vector3 mainMenuOriginalPosition;

    private void Start()
    {
        // Configuração inicial dos painéis
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

        // Salva a posição e rotação original do coral
        if (coral != null)
        {
            coralOriginalPosition = coral.transform.localPosition;
            coralOriginalRotation = coral.transform.localRotation;
        }

        // Configura os botões do menu
        if (menuButtons != null)
        {
            menuButtons.transform.localScale = Vector3.one;
        }

        // Salva a posição original do mainMenuPanel também
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
            mainMenuPanel.transform.localScale = Vector3.one;
        }
    }

    public void StartGame()
    {
        // Anima o coral para fora
        if (coral != null)
        {
            coral.transform.DOLocalMoveX(coralOriginalPosition.x + coralMoveX, coralTransitionTime)
                .SetEase(coralEase);
            coral.transform.DORotate(new Vector3(0, 0, coralRotation), coralTransitionTime)
                .SetEase(coralEase);
        }

        // Transição do painel e carrega a cena
        mainMenuPanel.transform.DOScale(0, panelTransitionTime)
            .SetEase(panelEaseOut)
            .OnComplete(() => {
                SceneManager.LoadScene(gameSceneName);
            });
    }

    public void OpenOptions()
    {
        // Anima o coral, o painel principal e os botões juntos
        Sequence menuExitSequence = DOTween.Sequence();

        if (coral != null)
        {
            menuExitSequence.Join(coral.transform.DOLocalMoveX(coralOriginalPosition.x + coralMoveX, coralTransitionTime)
                .SetEase(coralEase));
            menuExitSequence.Join(coral.transform.DORotate(new Vector3(0, 0, coralRotation), coralTransitionTime)
                .SetEase(coralEase));
        }

        // Move o painel principal junto com o coral
        menuExitSequence.Join(mainMenuPanel.transform.DOLocalMoveX(mainMenuOriginalPosition.x + coralMoveX, coralTransitionTime)
            .SetEase(coralEase));

        // Esconde o menu principal ao final da animação
        menuExitSequence.OnComplete(() => {
            mainMenuPanel.SetActive(false);
        });

        // Prepara e mostra o painel de opções
        optionsPanel.SetActive(true);
        optionsPanel.transform.localScale = Vector3.zero;
        optionsPanel.transform.localPosition = optionsPanelOriginalPosition - new Vector3(0, bubbleRiseDistance, 0);

        // Sequência de animação da bolha
        Sequence bubbleSequence = DOTween.Sequence();

        // Movimento de subida com wobble
        bubbleSequence.Append(optionsPanel.transform
            .DOLocalMoveY(optionsPanelOriginalPosition.y, bubbleRiseTime)
            .SetEase(bubbleEase));

        // Adiciona um pequeno movimento lateral (wobble)
        bubbleSequence.Join(optionsPanel.transform
            .DOLocalMoveX(optionsPanelOriginalPosition.x + Random.Range(-bubbleWobbleStrength, bubbleWobbleStrength), bubbleRiseTime)
            .SetEase(Ease.InOutSine));

        // Escala a bolha
        bubbleSequence.Join(optionsPanel.transform
            .DOScale(1, bubbleScaleTime)
            .SetEase(panelEaseIn));
    }

    public void CloseOptions()
    {
        Sequence menuEnterSequence = DOTween.Sequence();

        // Anima o coral de volta para sua posição original exata
        if (coral != null)
        {
            menuEnterSequence.Join(coral.transform.DOLocalMove(coralOriginalPosition, coralTransitionTime)
                .SetEase(coralEase));
            menuEnterSequence.Join(coral.transform.DORotateQuaternion(coralOriginalRotation, coralTransitionTime)
                .SetEase(coralEase));
        }

        // Mostra e anima o menu principal de volta para sua posição original exata
        mainMenuPanel.SetActive(true);
        menuEnterSequence.Join(mainMenuPanel.transform.DOLocalMove(mainMenuOriginalPosition, coralTransitionTime)
            .SetEase(coralEase));

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
        // Limpa todas as tweens ao destruir o objeto
        if (coral != null) DOTween.Kill(coral.transform);
        if (mainMenuPanel != null) DOTween.Kill(mainMenuPanel.transform);
        if (optionsPanel != null) DOTween.Kill(optionsPanel.transform);
        if (menuButtons != null) DOTween.Kill(menuButtons.transform);
    }
} 