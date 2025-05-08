using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float xpCap;
    [SerializeField] public float currentXp;
    public HealthBar healthBar;
    public XPBar xpBar;
    public DamageIndicator damageIndicator;

    [SerializeField] private float attackAnimationDuration = 0.9f; // Set this value to match the attack animation duration



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
    }

    public void GainXP(float amount)
    {
        currentXp += amount;
        if (currentXp > xpCap)
        {
            currentXp = xpCap;
        }
        
        xpBar.SetSlider(currentXp);
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

        // Activate death screen
        //...
    }
}