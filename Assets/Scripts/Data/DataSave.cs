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
            case 1:
                gameData.load1 = new PlayerData()
                {
                    healthPoints = PlayerStats.maxHealth,
                    attackDamage = PlayerAttack.attackDamage,
                    attackRange = PlayerAttack.attackRange,
                    attackSpeed = PlayerAttack.attackSpeed,
                    teeth = PlayerStats.teethCurrency
                };
                break;
            case 2:
                gameData.load2 = new PlayerData()
                {
                    healthPoints = PlayerStats.maxHealth,
                    attackDamage = PlayerAttack.attackDamage,
                    attackRange = PlayerAttack.attackRange,
                    attackSpeed = PlayerAttack.attackSpeed,
                    teeth = PlayerStats.teethCurrency
                };
                break;
            case 3:
                gameData.load3 = new PlayerData()
                {
                    healthPoints = PlayerStats.maxHealth,
                    attackDamage = PlayerAttack.attackDamage,
                    attackRange = PlayerAttack.attackRange,
                    attackSpeed = PlayerAttack.attackSpeed,
                    teeth = PlayerStats.teethCurrency
                };
                break;
            case 4:
                gameData.load4 = new PlayerData()
                {
                    healthPoints = PlayerStats.maxHealth,
                    attackDamage = PlayerAttack.attackDamage,
                    attackRange = PlayerAttack.attackRange,
                    attackSpeed = PlayerAttack.attackSpeed,
                    teeth = PlayerStats.teethCurrency
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
                load1 = (gameData.load1 ??= null),
                load2 = (gameData.load2 ??= null),
                load3 = (gameData.load3 ??= null),
                load4 = (gameData.load4 ??= null)
            };

            string json = JsonUtility.ToJson(data);
            string path = Application.persistentDataPath + "/playerData.json";
            System.IO.File.WriteAllText(path, json);
        }
    }

    public static void LoadInstancePlayerData()
    {
        switch (runningLoad)
        {
            case 1:
                PlayerStats.maxHealth = gameData.load1.healthPoints;
                PlayerAttack.attackDamage = gameData.load1.attackDamage;
                PlayerAttack.attackSpeed = gameData.load1.attackSpeed;
                PlayerStats.teethCurrency = gameData.load1.teeth;
                break;
            case 2:
                PlayerStats.maxHealth = gameData.load2.healthPoints;
                PlayerAttack.attackDamage = gameData.load2.attackDamage;
                PlayerAttack.attackSpeed = gameData.load2.attackSpeed;
                PlayerStats.teethCurrency = gameData.load2.teeth;
                break;
            case 3:
                PlayerStats.maxHealth = gameData.load3.healthPoints;
                PlayerAttack.attackDamage = gameData.load3.attackDamage;
                PlayerAttack.attackSpeed = gameData.load3.attackSpeed;
                PlayerStats.teethCurrency = gameData.load3.teeth;
                break;
            case 4:
                PlayerStats.maxHealth = gameData.load4.healthPoints;
                PlayerAttack.attackDamage = gameData.load4.attackDamage;
                PlayerAttack.attackSpeed = gameData.load4.attackSpeed;
                PlayerStats.teethCurrency = gameData.load4.teeth;
                break;
        }
    }

    public static void LoadPlayerData()
    {
        string path = Application.persistentDataPath + "/playerData.json";
        if (File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            DataLoads loadedData = JsonUtility.FromJson<DataLoads>(json);

            gameData = new DataLoads()
            {
                saves = loadedData.saves,
                load1 = (loadedData.load1 ??= null),
                load2 = (loadedData.load2 ??= null),
                load3 = (loadedData.load3 ??= null),
                load4 = (loadedData.load4 ??= null)
            };

            SavesMenu.Saves = loadedData.saves;
        }
        else
        {
            gameData = new DataLoads();
            SavesMenu.Saves = 0;
        }
    }
}
