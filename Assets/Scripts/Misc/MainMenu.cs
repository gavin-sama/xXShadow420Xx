using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{    
    public GameObject newButton;
    public GameObject loadButton;

    private int gameSaves;

    private void Start()
    {
        gameSaves = SavesMenu.Saves;

        if (gameSaves == 4)
            newButton.GetComponent<Button>().interactable = false;

        if (gameSaves == 0)
            loadButton.GetComponent<Button>().interactable = false;
    }

    public void NewGame()
    {
        // Open the canvas for the 4 load saves
        // Overwrite the option chosen
        // Clone and launch the fresh start scene

        SavesMenu.Saves += 1;

        SceneManager.LoadScene("Load1");
    }

    public void LoadGame()
    {
        // Open the canvas for the 4 load saves
        // Launch the option chosen

        SavesMenu.savesCanvas.SetActive(true);
    }

    public void Options()
    {
        // Open the settings canvas
    }

    public void Exit()
    {
        Application.Quit();
    }
}
