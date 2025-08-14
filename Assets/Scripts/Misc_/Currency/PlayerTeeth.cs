using UnityEngine;

public class PlayerTeeth : MonoBehaviour
{
    public static PlayerTeeth Instance;

    public int teeth = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddTeeth(int amount)
    {
        PlayerStats.teethCurrency += amount; // update the stat that actually gets saved
        Debug.Log($"Teeth: {PlayerStats.teethCurrency}");
        DataSave.SavePlayerData();
    }
}
