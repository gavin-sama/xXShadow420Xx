using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class AIHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("UI References")]
    public Slider healthSlider;

    [Header("Death Effects")]
    public GameObject deathEffectPrefab;
    public AudioClip deathSound;

    [Header("Fade Settings")]
    public float fadeDelay = 2f;         // Wait time before fading starts
    public float fadeDuration = 3f;      // Duration of the fade out

    private Animator anim;
    private SkinnedMeshRenderer meshRenderer;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        else
        {
            Debug.LogError(gameObject.name + ": Health slider reference is missing!");
        }

        anim = GetComponent<Animator>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (healthSlider != null) healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died.");

        // Disable AI logic
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<AIController>().enabled = false;

        // Set animation bool
        if (anim != null)
        {
            anim.SetBool("isDead", true);
        }

        // Disable collider
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Start death coroutine
        StartCoroutine(DeathRoutine());
    }


    IEnumerator DeathRoutine()
    {
        // Play death sound even after object is destroyed
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        // Play particle effect
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // Wait before fade starts
        yield return new WaitForSeconds(fadeDelay);

        // Destroy the object
        Destroy(gameObject);
    }

}
