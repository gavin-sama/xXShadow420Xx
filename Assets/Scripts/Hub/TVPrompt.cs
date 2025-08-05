using UnityEngine;
using TMPro;

public class TVPrompt : MonoBehaviour
{
    public CanvasGroup promptGroup;
    public float fadeDuration = 0.5f;
    private bool playerInRange = false;
    private bool isFading = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            StartCoroutine(FadeCanvasGroup(promptGroup, true));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            StartCoroutine(FadeCanvasGroup(promptGroup, false));
        }
    }

    System.Collections.IEnumerator FadeCanvasGroup(CanvasGroup cg, bool fadeIn)
    {
        if (isFading) yield break;
        isFading = true;

        float start = cg.alpha;
        float end = fadeIn ? 1f : 0f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            cg.alpha = Mathf.Lerp(start, end, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cg.alpha = end;
        isFading = false;
    }
}
