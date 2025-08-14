using UnityEngine;

public class AttackPrefabDamage : MonoBehaviour
{
    public int damage = 10;
    public LayerMask enemyLayers;
    public float destroyAfter = 2f;

    private void Start()
    {
        // Auto destroy the prefab
        Destroy(gameObject, destroyAfter);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider is in enemy layer
        if (((1 << other.gameObject.layer) & enemyLayers) != 0)
        {
            if (other.TryGetComponent(out AIHealth enemyHealth))
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log($"Prefab dealt {damage} damage to {other.name}");
            }
        }
    }
}
