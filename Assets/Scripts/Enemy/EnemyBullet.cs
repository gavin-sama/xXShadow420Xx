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

            PlayerStats player = hitObject.GetComponent<PlayerStats>();
            if (player != null)
            {
                Debug.Log("PlayerStats found");
                player.TakeDamage(Damage);
            }
            else
            {
                Debug.Log("PlayerStats not found");
            }
        }

        // Destroy the bullet on any collision
        Destroy(gameObject);
    }
}
