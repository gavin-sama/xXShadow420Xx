using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject savesCanvas;
    public Button newButton;
    public Button loadButton;

    private void Start()
    {
        savesCanvas.SetActive(false);
        SavesMenu.savesCanvas = savesCanvas;

        newButton.interactable = (CountSaves() < 4);
        loadButton.interactable = SaveExists();
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
