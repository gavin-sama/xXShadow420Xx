using UnityEngine;

public class BrawlerAttack : PlayerAttackBase
{
    [Header("Melee Settings")]
    public int damage = 20;
    public float range = 1.5f;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public GameObject impactEffect;

    [Header("Animation")]
    public Animator animator;
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");
    private static readonly int UltimateTrigger = Animator.StringToHash("Ultimate");

    [Header("Ultimate Projectile Settings")]
    public GameObject ultimateProjectilePrefab;
    public Transform ultimateSpawnPoint;
    public float ultimateProjectileSpeed = 30f;
    public AudioClip ultimateSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (attackPoint == null)
            attackPoint = transform;

        if (ultimateSpawnPoint == null)
            ultimateSpawnPoint = transform; // fallback
    }

    private void Update()
    {
        // Normal punch
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            PerformAttack();
        }

        // Ultimate
        if (Input.GetKeyDown(KeyCode.LeftControl) && canAttack)
        {
            PerformUltimate();
        }
    }

    public override void PerformAttack()
    {
        if (animator != null)
            animator.SetTrigger(AttackTrigger);

        lastAttackTime = Time.time;
    }

    public override void PerformUltimate()
    {
        if (animator != null)
            animator.SetTrigger(UltimateTrigger);

        lastAttackTime = Time.time;
    }

    // Called via animation event for melee damage
    public void DealDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, range, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out AIHealth enemyHealth))
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log($"Punch hit: {damage} damage to {enemy.name}");
            }
        }
    }

    public void SpawnImpactEffect()
    {
        if (impactEffect != null && attackPoint != null)
        {
            GameObject impact = Instantiate(impactEffect, attackPoint.position, attackPoint.rotation);
            Destroy(impact, 0.3f);
        }
    }

    // Called via animation event to fire ultimate projectile
    public void CastUltimateProjectile()
    {
        if (ultimateProjectilePrefab == null || ultimateSpawnPoint == null)
        {
            Debug.LogError("Missing ultimate projectile prefab or spawn point.");
            return;
        }

        Vector3 spawnPos = ultimateSpawnPoint.position;
        Vector3 direction = transform.forward; // straight ahead
        direction.Normalize();

        GameObject projectile = Instantiate(ultimateProjectilePrefab, spawnPos, Quaternion.LookRotation(direction));
        projectile.SetActive(true);

        if (projectile.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = direction * ultimateProjectileSpeed;
        }

        if (ultimateSound != null)
        {
            audioSource.PlayOneShot(ultimateSound);
        }

        Debug.Log("Ultimate projectile launched!");
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, range);
        }

        if (ultimateSpawnPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(ultimateSpawnPoint.position, 0.2f);
        }
    }
}
