using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PermUpMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI teethText;

    [Header("Upgrade Buttons")]
    [SerializeField] private Button resurrectButton;
    [SerializeField] private Button moreCoinsButton;
    [SerializeField] private Button stealthButton;
    [SerializeField] private Button healthButton;
    [SerializeField] private Button damageButton;
    [SerializeField] private Button speedButton;

    [Header("Costs")]
    public int resurrectionCost = 50;
    public int coinBoostCost = 20;
    public int stealthCost = 30;
    public int healthCost = 10;
    public int attackCost = 10;
    public int speedCost = 10;

    private void Start()
    {
        UpdateTeethDisplay();

        resurrectButton.onClick.AddListener(BuyResurrection);
        moreCoinsButton.onClick.AddListener(BuyCoinBoost);
        stealthButton.onClick.AddListener(BuyStealth);
        healthButton.onClick.AddListener(BuyHealth);
        damageButton.onClick.AddListener(BuyAttack);
        speedButton.onClick.AddListener(BuySpeed);
    }

    private void UpdateTeethDisplay()
    {
        teethText.text = "Teeth: " + PlayerStats.teethCurrency;
    }

    private bool TrySpendTeeth(int cost)
    {
        if (PlayerStats.teethCurrency >= cost)
        {
            PlayerStats.teethCurrency -= cost;
            UpdateTeethDisplay();
            return true;
        }
        return false;
    }

    private void BuyResurrection()
    {
        if (!PlayerStats.hasResurrection && TrySpendTeeth(resurrectionCost))
        {
            PlayerStats.hasResurrection = true;
            resurrectButton.interactable = false;
        }
    }

    private void BuyCoinBoost()
    {
        if (!PlayerStats.extraCoins && TrySpendTeeth(coinBoostCost))
        {
            PlayerStats.extraCoins = true;
            moreCoinsButton.interactable = false;
        }
    }

    private void BuyStealth()
    {
        if (!PlayerStats.lowHealthStealth && TrySpendTeeth(stealthCost))
        {
            PlayerStats.lowHealthStealth = true;
            stealthButton.interactable = false;
        }
    }

    private void BuyHealth()
    {
        if (TrySpendTeeth(healthCost))
        {
            PlayerStats.permHealthUpgrades++;
            PlayerStats.maxHealth += 10;

        }
    }

    private void BuyAttack()
    {
        if (TrySpendTeeth(attackCost))
        {
            PlayerStats.permAttackUpgrades++;
            PlayerAttack.attackDamage += 5;
        }
    }

    private void BuySpeed()
    {
        if (TrySpendTeeth(speedCost))
        {
            PlayerStats.permSpeedUpgrades++;
            PlayerAttack.attackSpeed += 0.1f;
        }
    }

}
