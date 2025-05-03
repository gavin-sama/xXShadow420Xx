using UnityEngine;
using UnityEngine.UI;

public class AIHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("UI References")]
    public Slider healthSlider;

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

        // Optional: destroy object or trigger animation
        // You might want to add a death animation here instead of immediately destroying

        Destroy(gameObject);
    }
}