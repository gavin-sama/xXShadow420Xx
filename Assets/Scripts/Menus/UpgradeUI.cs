using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    public static UpgradeUI instance;

    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button healthButton;
    [SerializeField] private Button speedButton;

    [SerializeField] private PlayerClassData playerClassData;

    [SerializeField] private float speedIncreaseAmount = 1f;

    private void Awake()
    {
        instance = this;
        upgradePanel.SetActive(false);
    }

    public void Show()
    {
        upgradePanel.SetActive(true);
    }

    private void Hide()
    {
        upgradePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void UpgradeAttack()
    {
        PlayerAttack.attackDamage += 5;
        PlayerStats.attackUpgrades++;
        Hide();
    }

    public void UpgradeHealth()
    {
        PlayerStats.maxHealth += 20;
        PlayerStats.healthUpgrades++;
        Hide();
    }

    public void UpgradeSpeed()
    {
        playerClassData.walkSpeed += speedIncreaseAmount;
        playerClassData.runSpeed += speedIncreaseAmount;

        PlayerStats.speedUpgrades++;
        Hide();
    }
}
