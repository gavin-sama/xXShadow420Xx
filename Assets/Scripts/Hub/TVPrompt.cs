using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using TMPro;

public class TVPrompt : MonoBehaviour
{
    public CanvasGroup promptGroup;          // "Press E" prompt UI
    public CanvasGroup videoCanvasGroup;     // Fullscreen video + dialogue UI
    public VideoPlayer videoPlayer;
    public VideoClip introClip;
    public VideoClip loopClip;
    public TextMeshProUGUI dialogueText;
    public float fadeDuration = 0.5f;

    private bool playerInRange = false;
    private bool videoStarted = false;
    private Coroutine dialogueCoroutine;

    public HubPlayerMovement playerMovement; // assign in inspector

    private bool promptIsFading = false;
    private bool videoIsFading = false;
    public MonoBehaviour cameraController; // Assign your camera script (e.g., MouseLook) in the Inspector

    private bool videoSequenceComplete = false;

    void Update()
    {
        if (playerInRange && !videoStarted && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed near TV");
            videoStarted = true;

            // Hide prompt and start video sequence
            StartCoroutine(FadeCanvasGroup(promptGroup, false));
            StartCoroutine(StartVideoSequence());
        }
    }

    IEnumerator StartVideoSequence()
    {
        cameraController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerMovement.SetMovementEnabled(false);

        yield return StartCoroutine(FadeCanvasGroup(videoCanvasGroup, true));

        PlayIntroVideo();

        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = null;
        }

        dialogueCoroutine = StartCoroutine(DialogueSequence());
    }

    IEnumerator DialogueSequence()
    {
        var killsLastRun = PlayerDataManager.Instance.playerData.killsLastRun;
        var totalKills = PlayerDataManager.Instance.playerData.totalKills;

        dialogueText.text = "Welcome to the nightly report...";
        yield return new WaitForSeconds((float)videoPlayer.clip.length);

        dialogueText.text = $"Breaking News! It's reported that around <color=red>{killsLastRun}</color> people are confirmed dead.";
        yield return new WaitForSeconds(4f);

        dialogueText.text = $"That makes the total amount of murders around <color=red>{totalKills}</color>!";
        yield return new WaitForSeconds(4f);

        dialogueText.text = "This is J.D. Stott signing off.";
        yield return new WaitForSeconds(4f);

        // Wait for player input (mouse click or E press)
        while (!Input.GetMouseButtonDown(0) && !Input.GetKeyDown(KeyCode.E))
        {
            yield return null;
        }

        yield return StartCoroutine(FadeCanvasGroup(videoCanvasGroup, false));

        videoPlayer.Stop();
        videoPlayer.clip = null;
        videoPlayer.loopPointReached -= OnIntroFinished;

        videoStarted = false;

        cameraController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerMovement.SetMovementEnabled(true);
    }

    void PlayIntroVideo()
    {
        videoPlayer.loopPointReached -= OnIntroFinished;
        videoPlayer.clip = introClip;
        videoPlayer.isLooping = false;
        videoPlayer.Play();
        videoPlayer.loopPointReached += OnIntroFinished;
    }

    void OnIntroFinished(VideoPlayer vp)
    {
        vp.loopPointReached -= OnIntroFinished;
        StartCoroutine(PlayLoopClipSmoothly());
    }

    IEnumerator PlayLoopClipSmoothly()
    {
        videoPlayer.clip = loopClip;
        videoPlayer.Prepare();

        // Wait until the loop clip is prepared to avoid flicker
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoPlayer.isLooping = true;
        videoPlayer.Play();
    }

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

            // Only fade prompt out, do NOT reset videoStarted or disable videoCanvas here!
            // This avoids interrupting an active video session
            StartCoroutine(FadeCanvasGroup(promptGroup, false));
        }
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, bool fadeIn)
    {
        bool fadingFlag = (cg == promptGroup) ? promptIsFading : videoIsFading;
        if (fadingFlag) yield break;

        if (cg == promptGroup) promptIsFading = true;
        else if (cg == videoCanvasGroup) videoIsFading = true;

        if (fadeIn)
            cg.gameObject.SetActive(true);

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

        if (!fadeIn)
            cg.gameObject.SetActive(false);

        if (cg == promptGroup) promptIsFading = false;
        else if (cg == videoCanvasGroup) videoIsFading = false;
    }
}
