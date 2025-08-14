using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject savesCanvas;
    public Button newButton;
    public Button loadButton;
    public GameObject pauseMenuPanel; // Assign your pause/options panel in Inspector

    public AudioSource musicSource;
    public float musicFadeInDuration = 2f;

    private void Start()
    {
        savesCanvas.SetActive(false);
        SavesMenu.savesCanvas = savesCanvas;

        newButton.interactable = (CountSaves() < 4);
        loadButton.interactable = SaveExists();

        if (musicSource != null && musicSource.clip != null)
        {
            musicSource.loop = false; // no auto-loop
            StartCoroutine(HandleLoopingMusicWithFade());
        }
    }

    private IEnumerator HandleLoopingMusicWithFade()
    {
        while (true)
        {
            // Start fade-in and play music
            yield return StartCoroutine(FadeInMusic());

            // Wait until song ends
            yield return new WaitUntil(() => !musicSource.isPlaying);

            // Optionally wait before restarting
            yield return new WaitForSeconds(0.5f); // tiny pause
        }
    }

    private IEnumerator FadeInMusic()
    {
        musicSource.Stop();
        musicSource.volume = 0f;
        musicSource.Play();

        float timer = 0f;
        while (timer < musicFadeInDuration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, 0.4f, timer / musicFadeInDuration);
            yield return null;
        }

        musicSource.volume = 0.4f;
    }

    public void NewGame()
    {
        savesCanvas.SetActive(true);
        SavesMenu.newGameMode = true;
    }

    public void LoadGame()
    {
        if (SaveExists())
        {
            savesCanvas.SetActive(true);
            SavesMenu.newGameMode = false;
        }
    }

    private bool SaveExists()
    {
        for (int i = 0; i < 4; i++)
        {
            string path = Application.persistentDataPath + "/save" + i + ".json";
            if (File.Exists(path))
                return true;
        }
        return false;
    }

    

    public void OpenOptions()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
    }

    private int CountSaves()
    {
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            string path = Application.persistentDataPath + "/save" + i + ".json";
            if (File.Exists(path))
                count++;
        }
        return count;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
