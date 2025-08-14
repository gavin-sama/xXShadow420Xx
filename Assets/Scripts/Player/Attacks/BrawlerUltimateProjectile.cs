using UnityEngine;

public class BrawlerUltimateProjectile : MonoBehaviour
{
    public int damage = 50;            // Damage applied to enemies
    public LayerMask enemyLayers;      // Layers considered enemies
    public float lifetime = 7f;        // Destroy after this time
    public bool canHitMultiple = true; // If true, can damage multiple enemies during lifetime

    private void Start()
    {
        Destroy(gameObject, lifetime); // Auto-destroy after set lifetime
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is in the enemy layer mask
        if (((1 << other.gameObject.layer) & enemyLayers) != 0)
        {
            if (other.TryGetComponent(out AIHealth enemyHealth))
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log($"Ultimate projectile hit {other.name} for {damage} damage");
            }

            // No Destroy() here — projectile stays active until lifetime ends
            if (!canHitMultiple)
            {
                // Optional: if we only want to damage the first enemy, disable collider
                GetComponent<Collider>().enabled = false;
            }
        }
    }
}
