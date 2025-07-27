using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavesMenu : MonoBehaviour
{
    public static int Saves = 0;
    public static GameObject savesCanvas;
    public static bool newGameMode = false;

    public void SelectSlot(int slotNum)
    {
        if (newGameMode)
        {
            CloneNewSave(slotNum);
        }
        else
        {
            LoadSave(slotNum);
        }
    }

    private void CloneNewSave(int slotNum)
    {
        string path = Application.persistentDataPath + "/save" + slotNum + ".json";
        File.WriteAllText(path, "{\"created\":true}");

        Debug.Log($"New save created at slot {slotNum}, loading Hub...");
        StartCoroutine(AsyncLoadScene("Hub"));
    }

    private void LoadSave(int saveNum)
    {
        string path = Application.persistentDataPath + "/save" + saveNum + ".json";
        if (File.Exists(path))
        {
            string sceneName = $"Load{saveNum}";
            Debug.Log($"Loading save from slot {saveNum} -> scene {sceneName}");
            StartCoroutine(AsyncLoadScene(sceneName));
        }
        else
        {
            Debug.LogWarning($"No save found in slot {saveNum}");
        }
    }

    private System.Collections.IEnumerator AsyncLoadScene(string sceneName)
    {
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        while (!load.isDone)
        {
            yield return null;
        }
    }
}
