using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float Damage = 10f;
    private GameObject shooter;

    public void InitializeShooter(GameObject shooterObj)
    {
        shooter = shooterObj;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject hitObject = collision.gameObject;

        // Prevent hitting the shooter
        if (hitObject == shooter) return;

        Debug.Log("Bullet hit something: " + hitObject.name);

        if (hitObject.CompareTag("Player"))
        {
            Debug.Log("Bullet hit player");

            PlayerStats playerStats = hitObject.GetComponent<PlayerStats>();
            PlayerMovement playerMovement = hitObject.GetComponent<PlayerMovement>();

            if (playerStats != null && playerMovement != null)
            {
                if (!playerMovement.isInvincible)
                {
                    Debug.Log("Player is vulnerable. Applying damage.");
                    playerStats.TakeDamage(Damage);
                }
                else
                {
                    Debug.Log("Player is invincible. No damage applied.");
                }
            }
            else
            {
                Debug.Log("PlayerStats or PlayerMovement not found.");
            }
        }

        // Destroy the bullet on any collision
        Destroy(gameObject);
    }
}
