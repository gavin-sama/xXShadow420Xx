using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    public PlayerData playerData = new PlayerData();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void AddKill()
    {
        playerData.killsLastRun++;
        playerData.totalKills++;
    }

    public void ResetLastRunKills()
    {
        playerData.killsLastRun = 0;
    }
}
