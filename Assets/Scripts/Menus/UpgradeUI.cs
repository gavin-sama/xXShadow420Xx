using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    public static UpgradeUI instance;

    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button healthButton;
    [SerializeField] private Button speedButton;

    private void Awake()
    {
        instance = this;
        upgradePanel.SetActive(false);
    }

    public void Show()
    {
        upgradePanel.SetActive(true);

        // Pause the game if needed
        Time.timeScale = 0f;

        // Show and unlock the cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


    private void Hide()
    {
        upgradePanel.SetActive(false);
        Time.timeScale = 1f;

        // Hide and lock the cursor again
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UpgradeAttack()
    {
        Debug.Log($"[UpgradeAttack] Before: Damage={PlayerAttack.attackDamage}, AttackUpgrades={PlayerStats.attackUpgrades}");
        PlayerAttack.attackDamage += 5;
        PlayerStats.attackUpgrades++;
        Debug.Log($"[UpgradeAttack] After: Damage={PlayerAttack.attackDamage}, AttackUpgrades={PlayerStats.attackUpgrades}");
        Hide();
    }

    public void UpgradeHealth()
    {
        Debug.Log($"[UpgradeHealth] Before: MaxHealth={PlayerStats.maxHealth}, HealthUpgrades={PlayerStats.healthUpgrades}");

        PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerStats != null)
        {
            // Store % of health before upgrade
            float healthPercent = playerStats.GetCurrentHealth() / PlayerStats.maxHealth;

            // Upgrade health
            PlayerStats.maxHealth += 20;
            PlayerStats.healthUpgrades++;

            // Keep same % health after upgrade
            playerStats.SetCurrentHealth(PlayerStats.maxHealth * healthPercent);

            // Update UI so slider scales correctly
            playerStats.healthBar.SetSliderMax(PlayerStats.maxHealth);
            playerStats.healthBar.SetSlider(playerStats.GetCurrentHealth());
        }

        Debug.Log($"[UpgradeHealth] After: MaxHealth={PlayerStats.maxHealth}, HealthUpgrades={PlayerStats.healthUpgrades}");
        Hide();
    }



    //doesnt work + there is no attack speed. who the fuck does attack speed instead of movement speed
    public void UpgradeSpeed()
    {
        Debug.Log($"[UpgradeSpeed] Before: AttackSpeed={PlayerAttack.attackSpeed}, SpeedUpgrades={PlayerStats.speedUpgrades}");
        PlayerAttack.attackSpeed *= 0.9f;
        PlayerStats.speedUpgrades++;
        Debug.Log($"[UpgradeSpeed] After: AttackSpeed={PlayerAttack.attackSpeed}, SpeedUpgrades={PlayerStats.speedUpgrades}");
        Hide();
    }
}

