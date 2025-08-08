using TMPro;
using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class CharacterMenuUI : MonoBehaviour
{
    public TMP_Dropdown characterDropdown;
    public TMP_Dropdown evolutionDropdown;
    public GameObject characterSelectCanvas;

    // Prefabs
    public GameObject wizardEvo1, wizardEvo2, wizardEvo3;
    public GameObject brawlerEvo1, brawlerEvo2, brawlerEvo3;
    public GameObject dinoEvo1, dinoEvo2, dinoEvo3;

    private GameObject currentCharacter;

    void Start()
    {
        if (SceneManager.GetSceneByBuildIndex(2).isLoaded)
            SceneManager.UnloadSceneAsync(2);
        characterSelectCanvas.SetActive(true);
        Time.timeScale = 0f; // Freeze game until selection
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
        string character = characterDropdown.options[characterDropdown.value].text.Trim();
        int evolution = evolutionDropdown.value + 1;

        GameObject prefabToSpawn = null;
        switch (character)
        {
            case "Wizard":
                prefabToSpawn = evolution == 1 ? wizardEvo1 :
                                evolution == 2 ? wizardEvo2 : wizardEvo3;
                break;
            case "Brawler":
                prefabToSpawn = evolution == 1 ? brawlerEvo1 :
                                evolution == 2 ? brawlerEvo2 : brawlerEvo3;
                break;
            case "Dino":
                prefabToSpawn = evolution == 1 ? dinoEvo1 :
                                evolution == 2 ? dinoEvo2 : dinoEvo3;
                break;
        }

        if (prefabToSpawn)
        {
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

    IEnumerator Spawn(GameObject prefab)
    {
        Vector3 spawnPosition = currentCharacter ? currentCharacter.transform.position : new Vector3(-32, 0, -75);
        Quaternion spawnRotation = currentCharacter ? currentCharacter.transform.rotation : Quaternion.identity;

        if (currentCharacter)
            Destroy(currentCharacter);

        currentCharacter = Instantiate(prefab, spawnPosition, spawnRotation);
        currentCharacter.name = "Player"; // Set name for lookup
        PlayerController.Instance = currentCharacter.transform;

        foreach (RangedEnemy enemy in FindObjectsByType<RangedEnemy>(FindObjectsSortMode.None))
        {
            enemy.AssignPlayer(currentCharacter.transform);
        }
        foreach (ShortRangeEnemy enemy in FindObjectsByType<ShortRangeEnemy>(FindObjectsSortMode.None))
        {
            enemy.AssignPlayer(currentCharacter.transform);
        }

        yield return new WaitUntil(() => currentCharacter != null);
    }
}
