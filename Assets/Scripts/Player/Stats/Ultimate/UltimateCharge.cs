using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UltimateChargeUI : MonoBehaviour
{
    [Header("UI References")]
    public Button ultButton;
    public TextMeshProUGUI chargeText;
    public Image ultIcon;

    [Header("Charge Settings")]
    public float chargeRate = 5f; // % per second
    public float killBonus = 20f;
    public KeyCode ultKey = KeyCode.LeftControl;

    [Header("Ultimate Logic")]
    [Header("Ultimate Logic")]
    public BeastAttack beastAttack;
    public RangedAttack rangedAttack;
    public BrawlerAttack brawlerAttack;



    private float currentCharge = 0f;
    private bool isReady = false;


    void Awake()
    {
    }


    void Start()
    {

        Debug.Log($"Using chargeText: {chargeText.name}", chargeText);

        ultButton.onClick.AddListener(TriggerUltimate);
        ultButton.interactable = false;
        chargeText.gameObject.SetActive(true);
        UpdateUI();
    }

    void Update()
    {
        if (!isReady)
        {
            currentCharge += chargeRate * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0f, 100f);

            if (currentCharge >= 100f)
            {
                currentCharge = 100f;
                isReady = true;
                ultButton.interactable = true;
                chargeText.gameObject.SetActive(false);
            }
            else
            {
                chargeText.text = $"{Mathf.FloorToInt(currentCharge)}%";
            }
        }

        if (isReady && Input.GetKeyDown(ultKey))
        {
            TriggerUltimate();
        }
    }

    public void AddChargeFromKill()
    {
        if (isReady) return;

        currentCharge += killBonus;
        currentCharge = Mathf.Clamp(currentCharge, 0f, 100f);
        UpdateUI();
    }

    private void TriggerUltimate()
    {
        if (!isReady) return;

        if (brawlerAttack != null)
            brawlerAttack.PerformUltimate();

        if (beastAttack != null)
            beastAttack.PerformUltimate();

        if (rangedAttack != null)
            rangedAttack.PerformUltimate();

        currentCharge = 0f;
        isReady = false;
        ultButton.interactable = false;
        chargeText.gameObject.SetActive(true);
        UpdateUI();
    }


    private void UpdateUI()
    {
        if (!isReady)
        {
            chargeText.text = $"{Mathf.FloorToInt(currentCharge)}%";

            // Darken icon based on charge
            float brightness = Mathf.Lerp(0.4f, 1f, currentCharge / 100f);
            SetIconBrightness(brightness);
        }
        else
        {
            SetIconBrightness(1f); // Full brightness
        }
    }

    private void SetIconBrightness(float brightness)
    {
        if (ultIcon != null)
        {
            Color baseColor = Color.white; // Or use ultIcon.color if you want to preserve tint
            ultIcon.color = baseColor * brightness;
        }
    }
}
