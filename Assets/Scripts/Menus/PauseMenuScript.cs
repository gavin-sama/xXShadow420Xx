using UnityEngine;
using TMPro; // for TextMeshPro
using UnityEngine.SceneManagement;
using System;
using UnityEngine.InputSystem;
using UnityEngine.UI; // <-- Needed for Button

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // The pause menu panel
    public GameObject controlsContent; // assign in inspector
    public TMP_Text saveDataText; // Text to show save/load data
    private bool isPaused = false;

    public GameObject player;
    private InputAction _pause;

    [Header("Return to Hub")]
    public Button returnButton; // Assign your Return to Hub button in the Inspector
    public string hubSceneName = "Hub";

    void Start()
    {
        player = GameObject.Find("Player");
        if (player != null)
        {
            _pause = player.GetComponent<PlayerInput>().actions["Pause"];
        }

        // Hide the button if we're already in the hub
        if (SceneManager.GetActiveScene().name == hubSceneName && returnButton != null)
        {
            returnButton.gameObject.SetActive(false);
        }

        Time.timeScale = 1f; // Always ensure time is running
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
    }

    void Update()
    {
        // Retry finding player if not assigned
        if (player == null)
        {
            player = GameObject.Find("Player");
            if (player != null)
            {
                _pause = player.GetComponent<PlayerInput>().actions["Pause"];
                Debug.Log("PauseMenu: Player found and input assigned.");
            }
        }

        // Toggle pause menu when Pause key is pressed
        if (_pause != null && _pause.WasPressedThisFrame())
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        int randomNumber = UnityEngine.Random.Range(1, 100);
        PlayerPrefs.SetInt("SavedNumber", randomNumber);
        PlayerPrefs.Save();

        if (saveDataText)
            saveDataText.text = "Saved: " + randomNumber;
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey("SavedNumber"))
        {
            int loadedNumber = PlayerPrefs.GetInt("SavedNumber");
            if (saveDataText)
                saveDataText.text = "Loaded: " + loadedNumber;
        }
        else
        {
            if (saveDataText)
                saveDataText.text = "No Data Saved!";
        }
    }

    public void ReturnToHub()
    {
        // Safety check — shouldn't be possible if button is hidden
        if (SceneManager.GetActiveScene().name == hubSceneName)
        {
            Debug.LogWarning("Already in the hub — can't return to hub again!");
            return;
        }

        // Load the hub scene
        SceneManager.LoadScene(hubSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleControls()
    {
        if (controlsContent != null)
        {
            bool isActive = controlsContent.activeSelf;
            controlsContent.SetActive(!isActive);
        }
    }
}
