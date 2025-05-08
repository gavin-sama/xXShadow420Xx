using UnityEngine;
using TMPro; // Use TextMeshPro instead of UnityEngine.UI

public class FPSDisplay : MonoBehaviour
{
    public int avgFrameRate;
    public TextMeshProUGUI display_Text;

    void Update()
    {
        float current = 1f / Time.unscaledDeltaTime;
        avgFrameRate = (int)current;
        display_Text.text = avgFrameRate.ToString() + " FPS";
    }
}
