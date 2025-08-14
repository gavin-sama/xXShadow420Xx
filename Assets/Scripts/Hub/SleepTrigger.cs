using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class SleepTrigger : MonoBehaviour
{
    public CanvasGroup promptGroup;
    public string gameSceneName = "GameRun";
    public float fadeDuration = 0.5f;
    private bool isPlayerNear = false;
    private bool isFading = false;
    private CanvasGroup canvasGroup;
    public GameObject napPromptUI;
    private Coroutine fadeCoroutine;

    void Start()
    {
        // Get or add CanvasGroup component for fading
        canvasGroup = napPromptUI.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = napPromptUI.AddComponent<CanvasGroup>();

        // Start hidden (alpha=0), but active so it can fade
        napPromptUI.SetActive(true);
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void Update()
    {
        if (isPlayerNear)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(FadeAndLoad());
            }
        }
    }

    IEnumerator FadeAndLoad()
    {
        // Fade out prompt before loading scene
        yield return Fade(0f);
        SceneManager.LoadScene(gameSceneName);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            StartFade(1f);  // Fade in
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            StartFade(0f);  // Fade out
        }
    }

    void StartFade(float targetAlpha)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(Fade(targetAlpha));
    }

    IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        // Enable or disable interactability based on alpha
        canvasGroup.interactable = targetAlpha > 0.9f;
        canvasGroup.blocksRaycasts = targetAlpha > 0.9f;

        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        // Optionally disable GameObject if fully transparent
        // napPromptUI.SetActive(targetAlpha > 0f);
    }
}
