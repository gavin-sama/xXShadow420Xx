using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CharacterSelect : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Dropdown characterDropdown;
    public GameObject characterSelectCanvas;
    public CrosshairManager crosshairManager;

    [Header("Prefabs - Evolution 1 Only")]
    public GameObject wizardEvo1;
    public GameObject brawlerEvo1;
    public GameObject dinoEvo1;

    private GameObject currentCharacter;

    public static string SelectedClass; // "Wizard", "Brawler", or "Dino"
    public static int CurrentEvolution = 1; // Always 1

    void Start()
    {
        // Ensure hub scene is unloaded if loaded
        if (SceneManager.GetSceneByBuildIndex(2).isLoaded)
            SceneManager.UnloadSceneAsync(2);

        characterSelectCanvas.SetActive(true);
        Time.timeScale = 0f; // Freeze for selection
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (characterSelectCanvas.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ReturnToHub()
    {
        SceneManager.LoadScene(2);
    }

    public void StartGame()
    {
        DataSave.LoadPlayerData();
        FindFirstObjectByType<PlayerDataManager>()?.ResetLastRunKills();

        string character = characterDropdown.options[characterDropdown.value].text.Trim();
        SelectedClass = character;

        // Set PlayerData.SelectedOutfitIndex based on dropdown selection
        switch (character)
        {
            case "Wizard":
                PlayerData.SelectedOutfitIndex = 0;
                break;
            case "Dino":
                PlayerData.SelectedOutfitIndex = 1;
                break;
            case "Brawler":
                PlayerData.SelectedOutfitIndex = 2;
                break;
            default:
                PlayerData.SelectedOutfitIndex = 0;
                break;
        }

        GameObject prefabToSpawn = GetPrefabFor(character);

        if (prefabToSpawn)
        {
            if (crosshairManager != null)
                crosshairManager.ActivateCrosshair(character);

            StartCoroutine(Spawn(prefabToSpawn));
            characterSelectCanvas.SetActive(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Debug.LogWarning("No prefab matched the selection!");
        }
    }

    private GameObject GetPrefabFor(string character)
    {
        switch (character)
        {
            case "Wizard": return wizardEvo1;
            case "Brawler": return brawlerEvo1;
            case "Dino": return dinoEvo1;
            default: return null;
        }
    }

    private IEnumerator Spawn(GameObject prefab)
    {
        Vector3 spawnPosition = currentCharacter ? currentCharacter.transform.position : new Vector3(0, 0, 0);
        Quaternion spawnRotation = currentCharacter ? currentCharacter.transform.rotation : Quaternion.identity;

        if (currentCharacter)
            Destroy(currentCharacter);

        currentCharacter = Instantiate(prefab, spawnPosition, spawnRotation);
        currentCharacter.name = "Player"; // Set name for lookup
        PlayerController.Instance = currentCharacter.transform;

        // Assign player to all enemies
        foreach (var enemy in FindObjectsByType<RangedEnemy>(FindObjectsSortMode.None))
            enemy.AssignPlayer(currentCharacter.transform);

        foreach (var enemy in FindObjectsByType<ShortRangeEnemy>(FindObjectsSortMode.None))
            enemy.AssignPlayer(currentCharacter.transform);

        foreach (var enemy in FindObjectsByType<GnarpyAI>(FindObjectsSortMode.None))
            enemy.AssignPlayer(currentCharacter.transform);

        yield return new WaitUntil(() => currentCharacter != null);
    }
}
