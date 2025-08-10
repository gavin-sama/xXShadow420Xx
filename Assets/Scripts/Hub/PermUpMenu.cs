using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;


public class PermUpMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuUI;
    [SerializeField] private TextMeshProUGUI teethText;
    [SerializeField] private GameObject teethIconPrefab; // prefab of the currency icon
    [SerializeField] private RectTransform teethCounterUI; // where teeth start
    [SerializeField] private RectTransform centerScreen;   // where teeth fly to

    [Header("Upgrade Buttons")]
    [SerializeField] private Button resurrectButton;
    [SerializeField] private Button moreCoinsButton;
    [SerializeField] private Button stealthButton;
    [SerializeField] private Button healthButton;
    [SerializeField] private Button damageButton;
    [SerializeField] private Button speedButton;
    [SerializeField] private Button confirmPurchaseButton;
    [SerializeField] private TextMeshProUGUI confirmPurchasePriceText;

    [SerializeField] private TextMeshProUGUI upgradeDescriptionText;
    [SerializeField] private string defaultDescription = " ";

    private int selectedUpgradeCost = 0;
    private System.Action selectedUpgradeAction = null;


    [Header("Costs")]
    public int resurrectionCost = 50;
    public int coinBoostCost = 20;
    public int stealthCost = 30;
    public int healthCost = 10;
    public int attackCost = 10;
    public int speedCost = 10;

    private void Start()
    {
        //Save File Testing
        //C:\Users\heath\AppData\LocalLow\DefaultCompany\xXShadow420Xx
        //DataSave.runningLoad = 1;
        //DataSave.LoadPlayerData();
        //DataSave.LoadInstancePlayerData();
        UpdateTeethDisplay();

        confirmPurchaseButton.onClick.AddListener(OnConfirmPurchase);
        confirmPurchaseButton.interactable = false;
        confirmPurchasePriceText.text = "";


        resurrectButton.onClick.AddListener(() => SelectUpgrade(resurrectionCost, () =>
        {
            PlayerStats.hasResurrection = true;
            resurrectButton.interactable = false;
        }));

        moreCoinsButton.onClick.AddListener(() => SelectUpgrade(coinBoostCost, () =>
        {
            PlayerStats.extraCoins = true;
            moreCoinsButton.interactable = false;
        }));

        stealthButton.onClick.AddListener(() => SelectUpgrade(stealthCost, () =>
        {
            PlayerStats.lowHealthStealth = true;
            stealthButton.interactable = false;
        }));

        healthButton.onClick.AddListener(() => SelectUpgrade(healthCost, () =>
        {
            PlayerStats.permHealthUpgrades++;
            PlayerStats.maxHealth += 10;
        }));

        damageButton.onClick.AddListener(() => SelectUpgrade(attackCost, () =>
        {
            PlayerStats.permAttackUpgrades++;
            PlayerAttack.attackDamage += 5;
        }));

        speedButton.onClick.AddListener(() => SelectUpgrade(speedCost, () =>
        {
            PlayerStats.permSpeedUpgrades++;
            PlayerAttack.attackSpeed += 0.1f;
        }));

        SetDefaultButtonStates();
    }

    private void SetDefaultButtonStates()
    {
        if (PlayerStats.hasResurrection)
            resurrectButton.interactable = false;

        if (PlayerStats.extraCoins)
            moreCoinsButton.interactable = false;

        if (PlayerStats.lowHealthStealth)
            stealthButton.interactable = false;

        if (PlayerStats.permHealthUpgrades > 0)
            healthButton.interactable = false;

        if (PlayerStats.permAttackUpgrades > 0)
            damageButton.interactable = false;

        if (PlayerStats.permSpeedUpgrades > 0)
            speedButton.interactable = false;
    }

    public void OpenMenu()
    {
        menuUI.SetActive(true);
        UpdateTeethDisplay();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f; // pause game if you want like in ClassMenuController
    }

    public void CloseMenu()
    {
        Debug.Log("CloseMenu called");

        // Hide the menu using the CanvasGroup
        menuUI.SetActive(false);

        // Lock & hide cursor again to resume gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;

        // Save player data after purchase (NOT WORKING)
        DataSave.SavePlayerData();
    }

    private void UpdateTeethDisplay()
    {
        teethText.text = "" + PlayerStats.teethCurrency;
    }

    private void BuyUpgrade(int cost, System.Action onPurchase)
    {
        if (PlayerStats.teethCurrency >= cost)
        {
            // Disable confirm button while animating (optional)
            confirmPurchaseButton.interactable = false;

            // Start coroutine to animate teeth one by one and subtract gradually
            StartCoroutine(AnimateTeethSequentially(cost, onPurchase));
        }
    }

    public void SelectUpgrade(int cost, System.Action onPurchase)
    {
        selectedUpgradeCost = cost;
        selectedUpgradeAction = onPurchase;
        UpdateConfirmButton();
    }

    private void UpdateConfirmButton()
    {
        confirmPurchasePriceText.text = $"{selectedUpgradeCost}";
        confirmPurchaseButton.interactable = PlayerStats.teethCurrency >= selectedUpgradeCost;
    }

    private void OnConfirmPurchase()
    {
        if (PlayerStats.teethCurrency >= selectedUpgradeCost && selectedUpgradeAction != null)
        {
            confirmPurchaseButton.interactable = false;
            StartCoroutine(AnimateTeethSequentially(selectedUpgradeCost, selectedUpgradeAction));
        }
    }


    private IEnumerator AnimateTeethSequentially(int amount, System.Action onPurchase)
    {
        for (int i = 0; i < amount; i++)
        {
            // Instantiate one tooth icon
            GameObject icon = Instantiate(teethIconPrefab, teethCounterUI.position, Quaternion.identity, menuUI.transform);
            RectTransform rt = icon.GetComponent<RectTransform>();

            // Animate it moving to center and wait until done
            yield return StartCoroutine(MoveTeethIcon(rt));

            // After animation completes, subtract one tooth and update UI
            PlayerStats.teethCurrency--;
            UpdateTeethDisplay();

        }

        // Invoke the upgrade effect after animation completes
        onPurchase?.Invoke();

        // Reset confirm button UI
        selectedUpgradeAction = null;
        selectedUpgradeCost = 0;
        confirmPurchaseButton.interactable = false;
        confirmPurchasePriceText.text = "";

        // Save player data after purchase (NOT WORKING)
        DataSave.SaveInstancePlayerData();
    }

    private System.Collections.IEnumerator MoveTeethIcon(RectTransform icon)
    {
        float t = 0;
        Vector3 startPos = icon.position;
        Vector3 endPos = centerScreen.position;

        while (t < 1)
        {
            t += Time.unscaledDeltaTime * 10f; // use unscaledDeltaTime for animation during pause
            icon.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        Destroy(icon.gameObject);
    }

    
    public void ShowUpgradeDescription(string desc)
    {
        upgradeDescriptionText.text = desc;
    }

    public void ClearUpgradeDescription()
    {
        upgradeDescriptionText.text = defaultDescription;
    }

    
}
