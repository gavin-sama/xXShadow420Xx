using TMPro;
using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

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
        Vector3 spawnPosition = currentCharacter ? currentCharacter.transform.position : new Vector3(0, 0, 0);
        Quaternion spawnRotation = currentCharacter ? currentCharacter.transform.rotation : Quaternion.identity;

        if (currentCharacter)
            Destroy(currentCharacter);

        currentCharacter = Instantiate(prefab, spawnPosition, spawnRotation);

        yield return new WaitUntil(() => currentCharacter != null);

        // Assign to camera
        var freeLook = Object.FindFirstObjectByType<CinemachineCamera>();
        if (freeLook)
        {
            freeLook.Follow = currentCharacter.transform;
            freeLook.LookAt = currentCharacter.transform;
        }

        // Assign playerCamera in PlayerMovement if needed
        PlayerMovement movement = currentCharacter.GetComponent<PlayerMovement>();
        if (movement)
        {
            Camera mainCam = Camera.main;
            if (mainCam)
            {
                movement.playerCamera = mainCam;
            }
        }
    }
}
