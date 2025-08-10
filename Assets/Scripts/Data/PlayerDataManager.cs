using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
     //public PlayerData playerData = new PlayerData();

    static public int killsLastRun;
    static public int totalKills;

    private void Awake()
    {
        
    }

    public void AddKill()
    {
        killsLastRun++;
        totalKills++;
    }

    public void ResetLastRunKills()
    {
        killsLastRun = 0;
    }
}
