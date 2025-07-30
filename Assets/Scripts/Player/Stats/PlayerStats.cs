using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public static float maxHealth = 100;
    [SerializeField] private float xpCap;
    [SerializeField] public float currentXp;

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

    public static int teethCurrency;
    public static int attackUpgrades = 0;
    public static int healthUpgrades = 0;
    public static int speedUpgrades = 0;

    private float currentHealth;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetSliderMax(maxHealth);

        currentXp = 0;
        xpBar.SetSliderCap(xpCap);

        // Ensure fade overlay starts transparent
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
    }

    public void GainXP(float amount)
    {
        if (isDead) return;

        targetXp += amount;
        if (targetXp > xpCap)
        {
            targetXp = xpCap;
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

        if (damageIndicator != null)
            damageIndicator.Flash();

        if (currentHealth <= 0)
            Die();
    }

    public void HealPlayer(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        healthBar.SetSlider(currentHealth);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("You died!");

        if (deathClip != null)
            audioSource.PlayOneShot(deathClip);

        if (playerAnimator != null)
            playerAnimator.SetTrigger("Die");

        StartCoroutine(FadeScreen());
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
