using UnityEngine;

public class RangedEnemy : BaseAIController
{
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float bulletSpeed = 25f;
    public float attackCooldown = 2f;
    public float shootingRange = 10f;

    private float lastShotTime;
    private bool isShooting;

    private bool isAttacking;

    protected override void HandleAI()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Throwing"))
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
            return;
        }

        if (Time.time < lastShotTime + attackCooldown)
        {
            isAttacking = true;
            navMeshAgent.isStopped = true;
            return;
        }

        isAttacking = false;

        if (distanceToPlayer <= shootingRange)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;

            Vector3 lookDir = (player.position + Vector3.up * 1.5f - transform.position).normalized;
            lookDir.y = 0;
            transform.rotation = Quaternion.LookRotation(lookDir);

            animator.SetTrigger("Throwing");
            lastShotTime = Time.time;
        }
        else
        {
            navMeshAgent.isStopped = false;
            ChasePlayer();
        }
    }



    public void ShootAtPlayer()
    {
        if (player == null) return;

        Vector3 aimPoint = player.position + Vector3.up * 1.5f;
        Vector3 direction = (aimPoint - shootPoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.LookRotation(direction));
        bullet.GetComponent<Rigidbody>().linearVelocity = direction * bulletSpeed; // slight correction from 'linearVelocity'
        bullet.GetComponent<EnemyBullet>().InitializeShooter(gameObject); // pass the shooter

        Debug.Log(" Bullet fired by animation event");
    }
}
