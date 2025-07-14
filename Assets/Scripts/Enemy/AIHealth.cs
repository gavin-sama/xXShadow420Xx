using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class AIHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    private int Xp = 20;

    [Header("UI References")]
    public Slider healthSlider;

    [Header("Death Effects")]
    public GameObject deathEffectPrefab;
    public AudioClip deathSound;

    [Header("Fade Settings")]
    public float fadeDelay = 0.5f;         // Wait before starting fade
    public float fadeDuration = 2f;        // Duration of fade out

    private NavMeshAgent navAgent;
    private BaseAIController aiController;
    private Collider enemyCollider;
    private SkinnedMeshRenderer meshRenderer;
    private Material[] materials;
    private bool isDead = false;

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
            Debug.LogWarning(gameObject.name + ": Health slider reference is missing!");
        }

        navAgent = GetComponent<NavMeshAgent>();
        aiController = GetComponent<BaseAIController>();
        enemyCollider = GetComponent<Collider>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        if (meshRenderer != null)
        {
            // Cache all materials to fade them later
            materials = meshRenderer.materials;
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (healthSlider != null) healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " died.");

        // Disable AI logic and collider immediately
        if (navAgent != null) navAgent.enabled = false;
        if (aiController != null) aiController.enabled = false;
        if (enemyCollider != null) enemyCollider.enabled = false;

        // Play death sound
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        // Spawn death particles immediately
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // Give XP to player
        GiveXPToPlayer();

        // Destroy the enemy immediately — no fade delay or coroutine
        Destroy(gameObject);
    }


    void GiveXPToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.GainXP(Xp);
                Debug.Log("Granting XP: " + Xp);
            }
        }
    }
}
