using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

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

    [SerializeField] private float attackAnimationDuration = 0.9f; // Set this value to match the attack animation duration

    public static int teethCurrency;

    private float currentHealth;

    // Add an Animator reference
    private Animator animator;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetSliderMax(maxHealth);

        //XP bar (CHANGE WHEN YOU HAVE MULTIPLE LEVELS)
        currentXp = 0;
        xpBar.SetSliderCap(xpCap);

        // Get the Animator component attached to the player
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        if (currentHealth <= 0)
        {
            Die();
        }

        // Gradually update XP
        if (currentXp != targetXp)
        {
            currentXp = Mathf.Lerp(currentXp, targetXp, xpLerpSpeed * Time.deltaTime);

            // Optional: snap to target when close enough
            if (Mathf.Abs(currentXp - targetXp) < 0.01f)
            {
                currentXp = targetXp;
            }

            xpBar.SetSlider(currentXp);
        }
    }


    public void GainXP(float amount)
    {
        targetXp += amount;
        if (targetXp > xpCap)
        {
            targetXp = xpCap;
        }
    }


    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        healthBar.SetSlider(currentHealth);

        // Set 'isHit' to true to trigger the damage animation
        if (animator != null)
        {
            animator.SetBool("isHit", true);

            // Reset 'isHit' after the animation duration
            StartCoroutine(ResetHitAnimation());
        }

        // Play hurt sound without interfering with other sounds
        if (hurtClip != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(hurtClip);
        }
    }

    private IEnumerator ResetHitAnimation()
    {
        // Wait for the duration of the hit animation
        yield return new WaitForSeconds(attackAnimationDuration);

        // Reset 'isHit' to false
        if (animator != null)
        {
            animator.SetBool("isHit", false);
        }
    }


    public void HealPlayer(float amount)
    {
        currentHealth += amount;
        healthBar.SetSlider(currentHealth);
    }

    private void Die()
    {
        Debug.Log("You died!");

        // Play death animation
        if (animator != null)
        {
            animator.SetBool("isDead", true);
        }

        // Pause the game
        Time.timeScale = 0f;


        // Play death sound even after object is destroyed
        if (deathClip != null)
        {
            audioSource.PlayOneShot(deathClip);
        }
        // Activate death screen
        //...
    }
}