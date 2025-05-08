using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavesMenu : MonoBehaviour
{
    public static int Saves = 0;
    public static GameObject savesCanvas;

    public void CloneNewSave()
    {
        // Create a new save from the base scene with load
    }

    public void LoadSave(int saveNum)
    {
        // Load one of the 4 saves if 
        string saveScene = $"Load{saveNum}";

        SceneManager.LoadScene(saveScene);
    }
}
