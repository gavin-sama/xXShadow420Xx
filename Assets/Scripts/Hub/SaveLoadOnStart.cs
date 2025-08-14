using System.IO;
using UnityEngine;

public class SaveLoadOnStart : MonoBehaviour
{
    private void Start()
    {
        if (PlayerPrefs.HasKey("runningLoad"))
        {
            DataSave.runningLoad = PlayerPrefs.GetInt("runningLoad");
            Debug.Log("Restored runningLoad to: " + DataSave.runningLoad);
        }
        else
        {
            Debug.LogWarning("No runningLoad found in PlayerPrefs!");
        }

        int slot = DataSave.runningLoad;
        if (slot >= 0 && slot <= 4)
        {
            string path = Application.persistentDataPath + "/playerData" + slot + ".json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                DataLoads loadedData = JsonUtility.FromJson<DataLoads>(json);

                // pick the correct slot
                PlayerData loadedSlotData = slot switch
                {
                    0 => loadedData.load0,
                    1 => loadedData.load1,
                    2 => loadedData.load2,
                    3 => loadedData.load3,
                    4 => loadedData.load4,
                    _ => null
                };

                if (loadedSlotData != null)
                {
                    // Apply to your systems
                    PlayerStats.maxHealth = loadedSlotData.healthPoints;
                    PlayerAttack.attackDamage = loadedSlotData.attackDamage;
                    PlayerAttack.attackRange = loadedSlotData.attackRange;
                    PlayerAttack.attackSpeed = loadedSlotData.attackSpeed;
                    PlayerStats.teethCurrency = loadedSlotData.teeth;

                    PlayerDataManager.killsLastRun = loadedSlotData.killsLastRun;
                    PlayerDataManager.totalKills = loadedSlotData.totalKills;

                    PlayerStats.hasResurrection = loadedSlotData.hasResurrection;
                    PlayerStats.extraCoins = loadedSlotData.extraCoins;
                    PlayerStats.lowHealthStealth = loadedSlotData.lowHealthStealth;

                    PlayerStats.permHealthUpgrades = loadedSlotData.permHealthUpgrades;
                    PlayerStats.permAttackUpgrades = loadedSlotData.permAttackUpgrades;
                    PlayerStats.permSpeedUpgrades = loadedSlotData.permSpeedUpgrades;
                }
            }
        }
    }

}

