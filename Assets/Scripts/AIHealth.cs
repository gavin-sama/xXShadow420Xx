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
    public GameObject deathEffectPrefab; // Assign a simple particle system in Inspector
    public AudioClip deathSound; // Assign a death sound effect

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
    }

    // For testing only
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            TakeDamage(20);
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"{gameObject.name} taking {damage} damage. Current health before: {currentHealth}");

        currentHealth -= damage;

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        Debug.Log($"{gameObject.name} health after damage: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Add this getter method for debugging
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died.");

        // 1. Disable AI components
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<AIController>().enabled = false; 

        // 2. Trigger death animation (if you have an Animator)
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        // 3. Disable collider to prevent weird physics
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 4. Add slight delay before destruction with particle effect
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        // 5. Optional death sound
        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null)
        {
            audio.PlayOneShot(deathSound); // Add a public AudioClip deathSound variable
        }

        // 6. Basic death particle effect
        GameObject deathParticles = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        // 7. Sink into ground slightly before disappearing
        float sinkSpeed = 0.5f;
        float sinkTime = 1.5f;
        float timer = 0;

        while (timer < sinkTime)
        {
            transform.position -= Vector3.up * sinkSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        // 8. Destroy after everything is done
        Destroy(deathParticles, 2f); // Clean up particles after they play
        Destroy(gameObject);
    }
}