using System.IO;
using UnityEngine;

public static class DataSave
{
    public static DataLoads gameData;
    public static int runningLoad;

    public static void SaveInstancePlayerData()
    {
        switch (runningLoad)
        {
            case 0:
                gameData.load0 = new PlayerData()
                {
                    selectedOutfitIndex = PlayerData.SelectedOutfitIndex,
                    healthPoints = PlayerStats.maxHealth,
                    attackDamage = PlayerAttack.attackDamage,
                    attackRange = PlayerAttack.attackRange,
                    attackSpeed = PlayerAttack.attackSpeed,
                    teeth = PlayerStats.teethCurrency,
                    killsLastRun = PlayerDataManager.killsLastRun,
                    totalKills = PlayerDataManager.totalKills,
                    hasResurrection = PlayerStats.hasResurrection,
                    extraCoins = PlayerStats.extraCoins,
                    lowHealthStealth = PlayerStats.lowHealthStealth,
                    permHealthUpgrades = PlayerStats.permHealthUpgrades,
                    permAttackUpgrades = PlayerStats.permAttackUpgrades,
                    permSpeedUpgrades = PlayerStats.permSpeedUpgrades
                };
                break;
            case 1:
                gameData.load1 = new PlayerData()
                {
                    selectedOutfitIndex = PlayerData.SelectedOutfitIndex,
                    healthPoints = PlayerStats.maxHealth,
                    attackDamage = PlayerAttack.attackDamage,
                    attackRange = PlayerAttack.attackRange,
                    attackSpeed = PlayerAttack.attackSpeed,
                    teeth = PlayerStats.teethCurrency,
                    
                    killsLastRun = PlayerDataManager.killsLastRun,
                    totalKills = PlayerDataManager.totalKills,

                    hasResurrection = PlayerStats.hasResurrection,
                    extraCoins = PlayerStats.extraCoins,
                    lowHealthStealth = PlayerStats.lowHealthStealth,
                    permHealthUpgrades = PlayerStats.permHealthUpgrades,
                    permAttackUpgrades = PlayerStats.permAttackUpgrades,
                    permSpeedUpgrades = PlayerStats.permSpeedUpgrades
                };
                break;
            case 2:
                gameData.load2 = new PlayerData()
                {
                    selectedOutfitIndex = PlayerData.SelectedOutfitIndex,
                    healthPoints = PlayerStats.maxHealth,
                    attackDamage = PlayerAttack.attackDamage,
                    attackRange = PlayerAttack.attackRange,
                    attackSpeed = PlayerAttack.attackSpeed,
                    teeth = PlayerStats.teethCurrency,
                    
                    killsLastRun = PlayerDataManager.killsLastRun,
                    totalKills = PlayerDataManager.totalKills,
                    
                    hasResurrection = PlayerStats.hasResurrection,
                    extraCoins = PlayerStats.extraCoins,
                    lowHealthStealth = PlayerStats.lowHealthStealth,
                    permHealthUpgrades = PlayerStats.permHealthUpgrades,
                    permAttackUpgrades = PlayerStats.permAttackUpgrades,
                    permSpeedUpgrades = PlayerStats.permSpeedUpgrades
                };
                break;
            case 3:
                gameData.load3 = new PlayerData()
                {
                    selectedOutfitIndex = PlayerData.SelectedOutfitIndex,
                    healthPoints = PlayerStats.maxHealth,
                    attackDamage = PlayerAttack.attackDamage,
                    attackRange = PlayerAttack.attackRange,
                    attackSpeed = PlayerAttack.attackSpeed,
                    teeth = PlayerStats.teethCurrency,
                    
                    killsLastRun = PlayerDataManager.killsLastRun,
                    totalKills = PlayerDataManager.totalKills,
                    
                    hasResurrection = PlayerStats.hasResurrection,
                    extraCoins = PlayerStats.extraCoins,
                    lowHealthStealth = PlayerStats.lowHealthStealth,
                    permHealthUpgrades = PlayerStats.permHealthUpgrades,
                    permAttackUpgrades = PlayerStats.permAttackUpgrades,
                    permSpeedUpgrades = PlayerStats.permSpeedUpgrades
                };
                break;
            case 4:
                gameData.load4 = new PlayerData()
                {
                    selectedOutfitIndex = PlayerData.SelectedOutfitIndex,
                    healthPoints = PlayerStats.maxHealth,
                    attackDamage = PlayerAttack.attackDamage,
                    attackRange = PlayerAttack.attackRange,
                    attackSpeed = PlayerAttack.attackSpeed,
                    teeth = PlayerStats.teethCurrency,
                    
                    killsLastRun = PlayerDataManager.killsLastRun,
                    totalKills = PlayerDataManager.totalKills,
                    
                    hasResurrection = PlayerStats.hasResurrection,
                    extraCoins = PlayerStats.extraCoins,
                    lowHealthStealth = PlayerStats.lowHealthStealth,
                    permHealthUpgrades = PlayerStats.permHealthUpgrades,
                    permAttackUpgrades = PlayerStats.permAttackUpgrades,
                    permSpeedUpgrades = PlayerStats.permSpeedUpgrades
                };
                break;
        }
    }

    public static void SavePlayerData()
    {
        if (gameData != null)
        {
            DataLoads data = new DataLoads()
            {
                saves = gameData.saves,
                load0 = (gameData.load0 ??= null), 
                load1 = (gameData.load1 ??= null),
                load2 = (gameData.load2 ??= null),
                load3 = (gameData.load3 ??= null),
                load4 = (gameData.load4 ??= null)
            };

            string json = JsonUtility.ToJson(data);
            string path = Application.persistentDataPath + "/playerData" + runningLoad + ".json";
            System.IO.File.WriteAllText(path, json);
            Debug.Log("Saved player data to: " + path);
        }
    }

    public static void LoadInstancePlayerData()
    {
        switch (runningLoad)
        {
            case 0:
                if (gameData.load0 == null) 
                    goto default;
                PlayerData.SelectedOutfitIndex = gameData.load0.selectedOutfitIndex;
                PlayerStats.maxHealth = gameData.load0.healthPoints;
                PlayerAttack.attackDamage = gameData.load0.attackDamage;
                PlayerAttack.attackSpeed = gameData.load0.attackSpeed;
                PlayerStats.teethCurrency = gameData.load0.teeth;
                PlayerDataManager.killsLastRun = gameData.load0.killsLastRun;
                PlayerDataManager.totalKills = gameData.load0.totalKills;
                PlayerStats.hasResurrection = gameData.load0.hasResurrection;
                PlayerStats.extraCoins = gameData.load0.extraCoins;
                PlayerStats.lowHealthStealth = gameData.load0.lowHealthStealth;
                PlayerStats.permHealthUpgrades = gameData.load0.permHealthUpgrades;
                PlayerStats.permAttackUpgrades = gameData.load0.permAttackUpgrades;
                PlayerStats.permSpeedUpgrades = gameData.load0.permSpeedUpgrades;
                break;
            case 1:
                if (gameData.load1 == null)
                {
                    Debug.Log("Go TO Default Activated beepboop");
                    goto default;
                }
                PlayerData.SelectedOutfitIndex = gameData.load1.selectedOutfitIndex;
                PlayerStats.maxHealth = gameData.load1.healthPoints;
                PlayerAttack.attackDamage = gameData.load1.attackDamage;
                PlayerAttack.attackSpeed = gameData.load1.attackSpeed;
                PlayerStats.teethCurrency = gameData.load1.teeth;
                PlayerDataManager.killsLastRun = gameData.load1.killsLastRun;
                PlayerDataManager.totalKills = gameData.load1.totalKills;
                PlayerStats.hasResurrection = gameData.load1.hasResurrection;
                PlayerStats.extraCoins = gameData.load1.extraCoins;
                PlayerStats.lowHealthStealth = gameData.load1.lowHealthStealth;
                PlayerStats.permHealthUpgrades = gameData.load1.permHealthUpgrades;
                PlayerStats.permAttackUpgrades = gameData.load1.permAttackUpgrades;
                PlayerStats.permSpeedUpgrades = gameData.load1.permSpeedUpgrades;
                break;
            case 2:
                if (gameData.load2 == null)
                        goto default;
                PlayerData.SelectedOutfitIndex = gameData.load2.selectedOutfitIndex;
                PlayerStats.maxHealth = gameData.load2.healthPoints;
                PlayerAttack.attackDamage = gameData.load2.attackDamage;
                PlayerAttack.attackSpeed = gameData.load2.attackSpeed;
                PlayerStats.teethCurrency = gameData.load2.teeth;
                PlayerDataManager.killsLastRun = gameData.load2.killsLastRun;
                PlayerDataManager.totalKills = gameData.load2.totalKills;
                PlayerStats.hasResurrection = gameData.load2.hasResurrection;
                PlayerStats.extraCoins = gameData.load2.extraCoins;
                PlayerStats.lowHealthStealth = gameData.load2.lowHealthStealth;
                PlayerStats.permHealthUpgrades = gameData.load2.permHealthUpgrades;
                PlayerStats.permAttackUpgrades = gameData.load2.permAttackUpgrades;
                PlayerStats.permSpeedUpgrades = gameData.load2.permSpeedUpgrades;
                break;
            case 3:
                if (gameData.load3 == null)
                    goto default;
                PlayerData.SelectedOutfitIndex = gameData.load3.selectedOutfitIndex;
                PlayerStats.maxHealth = gameData.load3.healthPoints;
                PlayerAttack.attackDamage = gameData.load3.attackDamage;
                PlayerAttack.attackSpeed = gameData.load3.attackSpeed;
                PlayerStats.teethCurrency = gameData.load3.teeth;
                PlayerDataManager.killsLastRun = gameData.load3.killsLastRun;
                PlayerDataManager.totalKills = gameData.load3.totalKills;
                PlayerStats.hasResurrection = gameData.load3.hasResurrection;
                PlayerStats.extraCoins = gameData.load3.extraCoins;
                PlayerStats.lowHealthStealth = gameData.load3.lowHealthStealth;
                PlayerStats.permHealthUpgrades = gameData.load3.permHealthUpgrades;
                PlayerStats.permAttackUpgrades = gameData.load3.permAttackUpgrades;
                PlayerStats.permSpeedUpgrades = gameData.load3.permSpeedUpgrades;
                break;
            case 4:
                if (gameData.load4 == null)
                    goto default;
                PlayerData.SelectedOutfitIndex = gameData.load4.selectedOutfitIndex;
                PlayerStats.maxHealth = gameData.load4.healthPoints;
                PlayerAttack.attackDamage = gameData.load4.attackDamage;
                PlayerAttack.attackSpeed = gameData.load4.attackSpeed;
                PlayerStats.teethCurrency = gameData.load4.teeth;
                PlayerDataManager.killsLastRun = gameData.load4.killsLastRun;
                PlayerDataManager.totalKills = gameData.load4.totalKills;
                PlayerStats.hasResurrection = gameData.load4.hasResurrection;
                PlayerStats.extraCoins = gameData.load4.extraCoins;
                PlayerStats.lowHealthStealth = gameData.load4.lowHealthStealth;
                PlayerStats.permHealthUpgrades = gameData.load4.permHealthUpgrades;
                PlayerStats.permAttackUpgrades = gameData.load4.permAttackUpgrades;
                PlayerStats.permSpeedUpgrades = gameData.load4.permSpeedUpgrades;
                break;
            default:
                Debug.Log("Default Activated beepboop");
                PlayerData tempData = new PlayerData();
                PlayerData.SelectedOutfitIndex = tempData.selectedOutfitIndex;
                PlayerStats.maxHealth = tempData.healthPoints;
                PlayerAttack.attackDamage = tempData.attackDamage;
                PlayerAttack.attackSpeed = tempData.attackSpeed;
                PlayerStats.teethCurrency = tempData.teeth;
                PlayerDataManager.killsLastRun = tempData.killsLastRun;
                PlayerDataManager.totalKills = tempData.totalKills;
                PlayerStats.hasResurrection = tempData.hasResurrection;
                PlayerStats.extraCoins = tempData.extraCoins;
                PlayerStats.lowHealthStealth = tempData.lowHealthStealth;
                PlayerStats.permHealthUpgrades = tempData.permHealthUpgrades;
                PlayerStats.permAttackUpgrades = tempData.permAttackUpgrades;
                PlayerStats.permSpeedUpgrades = tempData.permSpeedUpgrades;
                break;
        }
    }

    public static void LoadPlayerData()
    {
        string path = Application.persistentDataPath + "/playerData" + runningLoad + ".json"; // use runningLoad
        if (File.Exists(path))
        {
            Debug.Log(path);
            string json = System.IO.File.ReadAllText(path);
            DataLoads loadedData = JsonUtility.FromJson<DataLoads>(json);

            gameData = new DataLoads()
            {
                saves = loadedData.saves,
                load0 = (loadedData.load0 ??= null),
                load1 = (loadedData.load1 ??= null),
                load2 = (loadedData.load2 ??= null),
                load3 = (loadedData.load3 ??= null),
                load4 = (loadedData.load4 ??= null)
            };

            SavesMenu.Saves = loadedData.saves;
        }
        else
        {
            Debug.Log("Yo this shit exists part 2");
            gameData = new DataLoads();
            SavesMenu.Saves = 0;
        }
    }
}
