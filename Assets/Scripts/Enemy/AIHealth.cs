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
    public GameObject coinPrefab; //need coin prefab so the enemy drops coin 
    private bool isDead = false;
    public bool skipXPOnDeath = false;

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
        BeastAttack beast = GetComponent<BeastAttack>();
        if (beast != null && beast.IsInUltimate)
        {
            damage = Mathf.RoundToInt(damage * beast.damageReductionFactor);
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // avoid negatives

        // Update slider here
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        Debug.Log($"{gameObject.name} took {damage} damage. Remaining: {currentHealth}");

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
            AudioSource.PlayClipAtPoint(deathSound, transform.position);

        // Spawn death particles
        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        // Drop coins
        int coinDropCount = PlayerStats.extraCoins ? 2 : 1;
        for (int i = 0; i < coinDropCount; i++)
        {
            if (coinPrefab != null)
            {
                Vector3 dropPosition = transform.position + Random.insideUnitSphere * 0.5f;
                dropPosition.y = transform.position.y;
                Instantiate(coinPrefab, dropPosition, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Coin prefab is missing on " + gameObject.name);
            }
        }

        // Delay XP & destruction until next frame
        StartCoroutine(FinishDeath());
    }

    private IEnumerator FinishDeath()
{
    if (!skipXPOnDeath)
        GiveXPToPlayer();
    else
        Debug.Log($"{gameObject.name} died without giving XP because skipXPOnDeath is true.");

    FindFirstObjectByType<UltimateChargeUI>()?.AddChargeFromKill();

    Destroy(gameObject); // Immediate removal
    yield break;
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
