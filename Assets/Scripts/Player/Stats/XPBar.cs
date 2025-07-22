using UnityEngine;
using UnityEngine.UI;

public class XPBar : MonoBehaviour
{
    public Slider xpSlider;

    public void SetSlider(float amount)
    {
        xpSlider.value = amount;
    }
    public void SetSliderCap(float amount)
    {
        xpSlider.maxValue = amount;
        SetSlider(0);
    }
}