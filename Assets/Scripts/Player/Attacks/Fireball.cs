using UnityEngine;

public class Fireball : MonoBehaviour
{
    public int damage = 20;

    private void Start()
    {
        // Destroy after 10 seconds if it doesn't hit anything
        Destroy(gameObject, 10f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Only react to hitting an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            AIHealth health = collision.gameObject.GetComponent<AIHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }

        // Optional: prevent destroying on hitting terrain or the player
        if (!collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
