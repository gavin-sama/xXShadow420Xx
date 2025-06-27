using UnityEngine;
using System.Collections;

public class RangedEnemy : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float bulletSpeed = 25f;
    public float attackCooldown = 1.5f;
    private bool canShoot = true;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && canShoot)
        {
            Debug.Log("Player in range");
            StartCoroutine(Shoot(other.transform));
        }
    }

    IEnumerator Shoot(Transform target)
    {
        canShoot = false;

        // Aim toward player's chest (1.5 units above feet)
        Vector3 aimPoint = target.position + Vector3.up * 1.5f;
        Vector3 direction = (aimPoint - shootPoint.position).normalized;

        // Rotate enemy to look at player horizontally only
        Vector3 flatLook = new Vector3(aimPoint.x, transform.position.y, aimPoint.z);
        transform.LookAt(flatLook);

        // Spawn and shoot bullet
        GameObject bullet = Instantiate(
            bulletPrefab,
            shootPoint.position,
            Quaternion.LookRotation(direction)
        );

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = direction * bulletSpeed;

        Debug.Log("Bullet fired");

        yield return new WaitForSeconds(attackCooldown);
        canShoot = true;
    }
}
