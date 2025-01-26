using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    [SerializeField]
    private string sceneToLoad; // Nome da cena que será carregada (definido no Inspector)

    [SerializeField]
    private GameObject loadingScreen; // Referência ao objeto da tela de loading

    [SerializeField]
    private UnityEngine.UI.Slider loadingBar; // Opcional: barra de progresso

    [SerializeField]
    private float minimumLoadingTime = 2f; // Tempo mínimo de loading em segundos

    [SerializeField]
    private bool useSimulatedLoading = false; // Ativar/desativar loading simulado

    [SerializeField]
    private float simulatedLoadingTime = 3f; // Tempo simulado de loading

    [SerializeField]
    private BubbleLoadingEffect bubbleEffect; // Referência ao efeito de bolhas

    public void LoadScene()
    {
        loadingScreen.SetActive(true); // Ativa a tela de loading
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        float startTime = Time.time;
        float loadingProgress = 0f;
        float elapsedTime = 0f;

        bubbleEffect.StartBubbleAnimation();

        while (!operation.isDone)
        {
            elapsedTime = Time.time - startTime;
            float realProgress = Mathf.Clamp01(operation.progress / 0.9f);

            if (useSimulatedLoading)
            {
                float simulatedProgress = Mathf.Clamp01(elapsedTime / simulatedLoadingTime);
                loadingProgress = Mathf.Max(simulatedProgress, realProgress);
            }
            else
            {
                float normalizedTime = Mathf.Clamp01(elapsedTime / minimumLoadingTime);
                loadingProgress = Mathf.Max(normalizedTime, realProgress);
            }

            bubbleEffect.UpdateProgress(loadingProgress);

            bool timeCondition = useSimulatedLoading ? 
                               (elapsedTime >= simulatedLoadingTime) : 
                               (elapsedTime >= minimumLoadingTime);

            if (timeCondition && operation.progress >= 0.9f)
            {
                bubbleEffect.StopBubbleAnimation();
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
} 