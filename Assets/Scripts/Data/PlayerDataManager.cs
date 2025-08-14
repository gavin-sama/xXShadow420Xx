using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    static public int killsLastRun;
    static public int totalKills;

    private void Awake()
    {
        // Make this object persist between scenes
        DontDestroyOnLoad(gameObject);

        // Ensure only one copy exists
        var objs = FindObjectsOfType<PlayerDataManager>();
        if (objs.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void AddKill()
    {
        killsLastRun++;
        totalKills++;
        Debug.Log($"Kill added. Last run: {killsLastRun}, Total: {totalKills}");
    }

    public void ResetLastRunKills()
    {
        killsLastRun = 0;
    }
}
