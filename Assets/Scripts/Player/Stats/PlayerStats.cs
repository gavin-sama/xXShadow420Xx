using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.LowLevel;
using Unity.Cinemachine;

public class PlayerStats : MonoBehaviour
{
    public static float maxHealth = 100;
    [SerializeField] private float xpCap;
    [SerializeField] public float currentXp;
    private float baseXpCap;

    private float targetXp;
    [SerializeField] private float xpLerpSpeed = 3f;
    
    public HealthBar healthBar;
    public XPBar xpBar;
    public DamageIndicator damageIndicator;

    public AudioSource audioSource;
    public AudioClip hurtClip;
    public AudioClip deathClip;

    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Image fadeOverlay;
    [SerializeField] private float fadeSpeed = 1f;

    public static int teethCurrency = 100;
    public static int attackUpgrades = 0;
    public static int healthUpgrades = 0;
    public static int speedUpgrades = 0;

    public static int permAttackUpgrades = 0;
    public static int permHealthUpgrades = 0;
    public static int permSpeedUpgrades = 0;

    public static bool hasResurrection = false;
    public static bool extraCoins = false;
    public static bool lowHealthStealth = false;

    private bool isStealthed = false;
    private float stealthDuration = 10f;
    private float stealthCooldown = 30f;
    private float stealthTimer = 0f;
    private float stealthCooldownTimer = 0f;

    public static bool isUndetectable = false;


    private float currentHealth;
    private bool isDead = false;

    private void Start()
    {
        baseXpCap = xpCap;

        maxHealth = 100 + (permHealthUpgrades * 10);
        currentHealth = maxHealth;
        healthBar.SetSliderMax(maxHealth);

        PlayerAttack.attackDamage = 20 + (permAttackUpgrades * 5);
        PlayerAttack.attackSpeed = 0.6f + (permSpeedUpgrades * 0.1f);

        currentXp = 0;
        xpBar.SetSliderCap(xpCap);

        if (fadeOverlay != null)
            fadeOverlay.color = new Color(0, 0, 0, 0);
    }



    private void Update()
    {
        if (isDead) return;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        if (currentXp != targetXp)
        {
            currentXp = Mathf.Lerp(currentXp, targetXp, xpLerpSpeed * Time.deltaTime);

            if (Mathf.Abs(currentXp - targetXp) < 0.01f)
                currentXp = targetXp;

            xpBar.SetSlider(currentXp);
        }

        if (lowHealthStealth)
        {
            float healthPercent = currentHealth / maxHealth;

            if (!isStealthed && stealthCooldownTimer <= 0f && healthPercent <= 0.25f)
            {
                ActivateStealth();
            }

            if (isStealthed)
            {
                stealthTimer -= Time.deltaTime;
                if (stealthTimer <= 0f)
                {
                    DeactivateStealth();
                    stealthCooldownTimer = stealthCooldown;
                }
            }

            if (stealthCooldownTimer > 0f && !isStealthed)
            {
                stealthCooldownTimer -= Time.deltaTime;
            }
        }

    }

    public void GainXP(float amount)
    {
        if (isDead) return;

        targetXp += amount;

        if (targetXp >= xpCap)
        {
            targetXp = 0;
            xpCap *= 1.2f;
            xpBar.SetSliderCap(xpCap);

            OpenUpgradeMenu();
        }
    }


    private void OpenUpgradeMenu()
    {
        Time.timeScale = 0f;
        UpgradeUI.instance.Show();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        healthBar.SetSlider(currentHealth);

        if (hurtClip != null && !audioSource.isPlaying)
            audioSource.PlayOneShot(hurtClip);

        FindFirstObjectByType<DamageIndicator>()?.Flash();

        if (currentHealth <= 0)
            Die();
    }

    public void HealPlayer(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        healthBar.SetSlider(currentHealth);
    }

    public static void SetActivePerk(string perkName)
    {
        hasResurrection = false;
        extraCoins = false;
        lowHealthStealth = false;

        switch (perkName)
        {
            case "Resurrection":
                hasResurrection = true;
                break;
            case "ExtraCoins":
                extraCoins = true;
                break;
            case "LowHealthStealth":
                lowHealthStealth = true;
                break;
        }
    }

    private void ActivateStealth()
    {
        isStealthed = true;
        isUndetectable = true;
        stealthTimer = stealthDuration;

        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
            r.enabled = false;

        Debug.Log("Stealth activated!");
    }

    private void DeactivateStealth()
    {
        isStealthed = false;
        isUndetectable = false;

        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
            r.enabled = true;

        Debug.Log("Stealth ended.");
    }


    private void Die()
    {
        if (isDead) return;

        if (hasResurrection)
        {
            hasResurrection = false;
            currentHealth = maxHealth * 0.5f;
            healthBar.SetSlider(currentHealth);
            Debug.Log("Resurrected at 50% HP");
            return;
        }
        isDead = true;

        Debug.Log("You died!");

        if (deathClip != null)
            audioSource.PlayOneShot(deathClip);

        if (playerAnimator != null)
            playerAnimator.SetTrigger("Die");

        StartCoroutine(FadeScreen());

        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<CharacterController>().enabled = false; // if used
        GetComponent<CinemachineCamera>().enabled = false;

    }

    public void OnDeathAnimationFinished()
    {
        StartCoroutine(HandleDeathSequence());
    }

    private IEnumerator HandleDeathSequence()
    {

        // Wait for death animation duration (e.g., length of deathClip)
        float waitTime = deathClip != null ? deathClip.length : 2f;
        yield return new WaitForSeconds(waitTime);

        // Reset player stats
        PlayerStats.attackUpgrades = 0;
        PlayerStats.healthUpgrades = 0;
        PlayerStats.speedUpgrades = 0;

        PlayerAttack.attackDamage = 20;
        PlayerAttack.attackSpeed = 0.6f;
        maxHealth = 100;

        xpCap = baseXpCap;
        currentXp = 0;
        targetXp = 0;
        xpBar.SetSliderCap(xpCap);

        // Load scene
        SceneManager.LoadScene("Hub");
    }

    private IEnumerator FadeScreen()
    {
        if (fadeOverlay != null)
        {
            Color fadeColor = fadeOverlay.color;
            fadeColor.a = 0;
            fadeOverlay.color = fadeColor;

            while (fadeColor.a < 1f)
            {
                fadeColor.a += Time.deltaTime * fadeSpeed;
                fadeOverlay.color = fadeColor;
                yield return null;
            }
        }
    }

}
