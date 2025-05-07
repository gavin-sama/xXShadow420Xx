using System.IO;
using UnityEngine;

public class DataSave
{
    DataLoads gameData;

    public void SavePlayerData()
    {
        DataLoads data = new DataLoads();
        for (int i = 1; i <= SavesMenu.Saves; i++)
        {
            if (i == 1)
            {
                data.load1 = gameData.load1;
            }
            else if (i == 2)
            {
                data.load2 = gameData.load2;
            }
            else if (i == 3)
            {
                data.load3 = gameData.load3;
            }
            else if (i == 4)
            {
                data.load4 = gameData.load4;
            }
            else
            {
                Debug.LogWarning("Data not found to save.");
            }
        }

        string json = JsonUtility.ToJson(data);
        string path = Application.persistentDataPath + "/playerData.json";
        System.IO.File.WriteAllText(path, json);
    }

    public void LoadPlayerData()
    {
        string path = Application.persistentDataPath + "/playerData.json";
        if (File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            DataLoads loadedData = JsonUtility.FromJson<DataLoads>(json);
            
            // logic to load the data to some space
        }
    }
}
