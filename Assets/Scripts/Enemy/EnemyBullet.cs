using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float Damage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Bullet hit something: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Bullet hit player");

            PlayerStats player = other.GetComponent<PlayerStats>();
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
    }
}
