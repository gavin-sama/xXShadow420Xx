using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    public GameObject wizardCrosshair;
    public GameObject brawlerCrosshair;
    public GameObject beastCrosshair;

    public void ActivateCrosshair(string className)
    {
        wizardCrosshair.SetActive(false);
        brawlerCrosshair.SetActive(false);
        beastCrosshair.SetActive(false);

        switch (className)
        {
            case "Wizard":
                wizardCrosshair.SetActive(true);
                break;
            case "Brawler":
                brawlerCrosshair.SetActive(true);
                break;
            case "Dino": 
                beastCrosshair.SetActive(true);
                break;
        }
    }
}
