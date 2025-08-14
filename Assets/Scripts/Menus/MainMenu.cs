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

    public AudioSource musicSource;
    public float musicFadeInDuration = 2f;

    [SerializeField] private MainMenu mainMenu;

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
            musicSource.volume = Mathf.Lerp(0f, 0.1f, timer / musicFadeInDuration);
            yield return null;
        }

        musicSource.volume = 0.1f;
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

    public void StartNewGame(int saveSlot)
    {
        DataSave.runningLoad = saveSlot;
        PlayerPrefs.SetInt("runningLoad", saveSlot);
        PlayerPrefs.Save();

        DataSave.gameData = new DataLoads();

        PlayerData newPlayerData = new PlayerData();
        switch (saveSlot)
        {
            case 1: DataSave.gameData.load1 = newPlayerData; break;
            case 2: DataSave.gameData.load2 = newPlayerData; break;
            case 3: DataSave.gameData.load3 = newPlayerData; break;
            case 4: DataSave.gameData.load4 = newPlayerData; break;
        }

        DataSave.SavePlayerData();
        SceneManager.LoadScene(2);
    }
    
    private bool SaveExists()
    {
        for (int i = 0; i <= 4; i++) // start from 1, since slots are 1-4
        {
            string path = Application.persistentDataPath + "/playerData" + i + ".json";
            if (File.Exists(path))
                return true;
        }
        return false;
    }

    private int CountSaves()
    {
        int count = 0;
        for (int i = 0; i <= 4; i++)
        {
            string path = Application.persistentDataPath + "/playerData" + i + ".json";
            if (File.Exists(path))
                count++;
        }
        return count;
    }

    public void LoadGame(int slot)
    {
        string path = Application.persistentDataPath + "/playerData" + slot + ".json";

        if (File.Exists(path))
        {
            DataSave.runningLoad = slot;
            PlayerPrefs.SetInt("runningLoad", slot);
            PlayerPrefs.Save();

            SceneManager.LoadScene(2);
        }
        else
        {
            Debug.LogWarning("Save file not found for slot " + slot);
        }
    }

    public void OnSlotSelected(int slot)
    {
        if (SavesMenu.newGameMode)
            mainMenu.StartNewGame(slot);
        else
            mainMenu.LoadGame(slot);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
