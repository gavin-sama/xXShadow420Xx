using UnityEngine;

public class RangedAttack : PlayerAttackBase
{
    private Animator animator;
    private AudioSource audioSource;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float projectileSpeed = 30f;
    public LayerMask aimLayerMask;

    [Header("Ultimate Settings")]
    public float ultimateRange = 6f;
    public LayerMask enemyLayers;
    private static readonly int UltimateTrigger = Animator.StringToHash("Ultimate");

    [Header("Ultimate Visual Effect")]
    public GameObject ultimateEffectPrefab; // assign your particle system prefab in inspector

    private GameObject activeUltimateEffect;

    [Header("Audio Feedback")]
    public AudioClip castSound;
    public AudioClip ultimateSound;

    private static readonly int AttackTrigger = Animator.StringToHash("Attack");

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (shootPoint == null)
        {
            Debug.LogWarning("ShootPoint not assigned. Creating default.");
            GameObject shootObj = new GameObject("ShootPoint");
            shootObj.transform.parent = transform;
            shootObj.transform.localPosition = new Vector3(0, 1.5f, 1f);
            shootPoint = shootObj.transform;
        }

        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab not assigned to RangedAttack!");
        }
    }

    private void Update()
    {
        // Handle ultimate (CTRL key)
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            Debug.Log("CTRL key detected");
            TriggerUltimate();
        }

        // Handle normal attack (LMB)
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            Debug.Log("LMB clicked, performing regular attack");
            PerformAttack();  // This triggers the normal attack
        }
    }



    // Called via Animation Event for normal attack
    public void CastProjectile()
    {
        if (projectilePrefab == null || shootPoint == null)
        {
            Debug.LogError("Missing projectile prefab or shoot point.");
            return;
        }

        // Spawn exactly at shootPoint
        Vector3 spawnPosition = shootPoint.position;

        // Direction tilted slightly right
        Vector3 direction = transform.forward + transform.right * 0.2f;
        direction.Normalize(); // Make sure it's a unit vector

        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.LookRotation(direction));
        projectile.SetActive(true);

        if (projectile.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        if (castSound != null)
        {
            audioSource.PlayOneShot(castSound);
        }

        Debug.Log("Projectile fired!");
    }




    // Triggers the ultimate attack animation
    private void TriggerUltimate()
    {
        Debug.Log("Entered TriggerUltimate");

        if (animator == null)
        {
            Debug.LogError("Animator is null!");
            return;
        }

        Debug.Log("Animator found, setting Ultimate trigger");
        animator.SetTrigger(UltimateTrigger);

        // Play ultimate sound
        if (ultimateSound != null)
        {
            audioSource.PlayOneShot(ultimateSound);
        }

        lastAttackTime = Time.time;
    }



    // Called via Animation Event at the right moment
    public void CastUltimate()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, ultimateRange, enemyLayers);

        if (hitEnemies.Length == 0)
        {
            Debug.Log("No enemies in range for ultimate.");
            return;
        }

        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log($"Ultimate hit: {enemy.name}");

            AIHealth health = enemy.GetComponent<AIHealth>();
            if (health != null)
            {
                health.Die();  // Call the Die method instead of Destroy
            }
            else
            {
                Debug.LogWarning($"Enemy {enemy.name} has no AIHealth component!");
            }
        }

        Debug.Log($"Ultimate affected {hitEnemies.Length} enemies!");
    }

    public override void PerformAttack()
    {
        animator.SetTrigger(AttackTrigger);
        lastAttackTime = Time.time;
    }

    public override void PerformUltimate()
    {
        TriggerUltimate(); // External call support
    }

    // Spawn the particle effect prefab during ultimate start
    private void SpawnUltimateEffect()
    {
        if (ultimateEffectPrefab == null)
        {
            Debug.LogWarning("Ultimate effect prefab not assigned!");
            return;
        }

        if (activeUltimateEffect != null)
        {
            Destroy(activeUltimateEffect);
        }

        activeUltimateEffect = Instantiate(ultimateEffectPrefab, transform.position, Quaternion.identity, transform);
    }

    // Call this at the end of your ultimate animation (Animation Event or EndAttack method)
    public void StopUltimateEffect()
    {
        if (activeUltimateEffect != null)
        {
            // If it has a ParticleSystem component, stop it gracefully
            ParticleSystem ps = activeUltimateEffect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                Destroy(activeUltimateEffect, ps.main.duration);
            }
            else
            {
                Destroy(activeUltimateEffect);
            }
            activeUltimateEffect = null;
        }
    }

    // Optional animation end
    public void EndAttack()
    {
        animator.ResetTrigger(AttackTrigger);
        animator.ResetTrigger(UltimateTrigger);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, ultimateRange);
    }
}
