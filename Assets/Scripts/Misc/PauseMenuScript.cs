using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // The pause menu panel
    public Text saveDataText; // Text to show save/load data
    private bool isPaused = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Toggle pause menu when Escape is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void PauseGame()
    {
        pauseMenuUI.SetActive(true); // Show the pause menu
        Time.timeScale = 0f; // Pause the game
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false); // Hide the pause menu
        Time.timeScale = 1f; // Resume game time
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
    }

    public void SaveData()
    {
        // Save a random number to PlayerPrefs (for example purposes)
        int randomNumber = UnityEngine.Random.Range(1, 100);
        PlayerPrefs.SetInt("SavedNumber", randomNumber);
        PlayerPrefs.Save();

        // Update UI with the saved data
        if (saveDataText)
            saveDataText.text = "Saved: " + randomNumber;
    }

    public void LoadData()
    {
        // Load the saved number from PlayerPrefs
        if (PlayerPrefs.HasKey("SavedNumber"))
        {
            int loadedNumber = PlayerPrefs.GetInt("SavedNumber");

            // Update UI with loaded data
            if (saveDataText)
                saveDataText.text = "Loaded: " + loadedNumber;
        }
        else
        {
            if (saveDataText)
                saveDataText.text = "No Data Saved!";
        }
    }

    public void QuitGame()
    {
        Application.Quit(); // Close the application
    }
}

