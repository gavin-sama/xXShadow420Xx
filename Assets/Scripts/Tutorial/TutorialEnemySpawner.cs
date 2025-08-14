using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialEnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject zombiePrefab;
    public GameObject mailbotPrefab;
    public GameObject gnarpyPrefab; 

    [Header("UI Elements")]
    public GameObject enemySpawnerPanel;
    public TMP_Text zombieCountText;
    public TMP_Text mailbotCountText;
    public TMP_Text gnarpyCountText; 
    public Button zombiePlusButton;
    public Button zombieMinusButton;
    public Button mailbotPlusButton;
    public Button mailbotMinusButton;
    public Button gnarpyPlusButton; 
    public Button gnarpyMinusButton;
    public Button applyButton;

    [Header("Spawn Settings")]
    public Transform spawnArea;
    private BoxCollider spawnBox;

    private int zombieCount = 0;
    private int mailbotCount = 0;
    private int gnarpyCount = 0; 
    private const int maxPerType = 5;

    void Start()
    {
        spawnBox = spawnArea.GetComponent<BoxCollider>();

        zombiePlusButton.onClick.AddListener(() => ChangeCount(ref zombieCount, 1));
        zombieMinusButton.onClick.AddListener(() => ChangeCount(ref zombieCount, -1));
        mailbotPlusButton.onClick.AddListener(() => ChangeCount(ref mailbotCount, 1));
        mailbotMinusButton.onClick.AddListener(() => ChangeCount(ref mailbotCount, -1));
        gnarpyPlusButton.onClick.AddListener(() => ChangeCount(ref gnarpyCount, 1)); 
        gnarpyMinusButton.onClick.AddListener(() => ChangeCount(ref gnarpyCount, -1)); 

        applyButton.onClick.AddListener(SpawnEnemies);

        UpdateUI();
    }

    void Update()
    {
        // Only allow opening spawner panel when game is not paused
        if (Time.timeScale > 0f && Input.GetKeyDown(KeyCode.M))
        {
            bool isActive = enemySpawnerPanel.activeSelf;
            enemySpawnerPanel.SetActive(!isActive);

            if (!isActive)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }


    void ChangeCount(ref int count, int delta)
    {
        count = Mathf.Clamp(count + delta, 0, maxPerType);
        UpdateUI();
    }

    void UpdateUI()
    {
        zombieCountText.text = zombieCount.ToString();
        mailbotCountText.text = mailbotCount.ToString();
        gnarpyCountText.text = gnarpyCount.ToString(); 
    }

    Vector3 GetRandomPositionInBox()
    {
        Vector3 center = spawnBox.center + spawnBox.transform.position;
        Vector3 size = spawnBox.size;

        float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float y = center.y;
        float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        return new Vector3(x, y, z);
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < zombieCount; i++)
        {
            Instantiate(zombiePrefab, GetRandomPositionInBox(), Quaternion.identity);
        }

        for (int i = 0; i < mailbotCount; i++)
        {
            Instantiate(mailbotPrefab, GetRandomPositionInBox(), Quaternion.identity);
        }

        for (int i = 0; i < gnarpyCount; i++) 
        {
            Instantiate(gnarpyPrefab, GetRandomPositionInBox(), Quaternion.identity);
        }
    }
}
