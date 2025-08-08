using UnityEngine;
using System.Collections.Generic;

public class BeastAttack : PlayerAttackBase
{
    [Header("Melee Settings")]
    public int damage = 20;
    public float range = 1.5f;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public Animator animator;
    public GameObject impactEffect;

    [Header("Ultimate Settings")]
    public float ultimateRange = 6f;
    public GameObject ultimateEffectPrefab;

    private GameObject activeUltimateEffect;

    [Header("Ultimate Transformation")]
    public float ultimateDuration = 5f;
    public float damageMultiplierDuringUltimate = 2f;
    public float damageReductionFactor = 0.5f; // 50% damage taken
    public bool isInUltimate;
    private float ultimateEndTime;
    private Vector3 originalScale;


    [Header("Ultimate Audio")]
    public AudioClip ultimateSound;
    private AudioSource audioSource;

    private static readonly int UltimateTrigger = Animator.StringToHash("Ultimate");

        [Header("Charge Settings")]
    public float chargeSpeed = 3f;
    public float chargeDamageMultiplier = 1.5f;

    [Header("Charge Effects")]
    public GameObject groundBreakEffect;
    public Transform groundEffectPoint;

    private bool isCharging;
    private bool wasCharging;

    // Track enemies hit during current charge
    private HashSet<Collider> hitEnemiesDuringCharge = new HashSet<Collider>();
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        originalScale = transform.localScale;
    }

    private void Update()
    {
        bool shouldCharge = Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W);

        // Handle Ultimate input independently
        if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) && canAttack)
        {
            TriggerUltimate();
        }

        // Start charging
        if (shouldCharge && canAttack)
        {
            if (!isCharging)
            {
                isCharging = true;
                animator.SetBool("isCharging", true);
                hitEnemiesDuringCharge.Clear(); // Reset hit tracking
                Debug.Log("Started charging");

                // Spawn ground-breaking effect
                if (groundBreakEffect != null && groundEffectPoint != null)
                {
                    Instantiate(groundBreakEffect, groundEffectPoint.position, Quaternion.identity);
                }
            }

            // Move forward
            transform.position += transform.forward * Time.deltaTime * chargeSpeed;

            Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, 2f, enemyLayers);
            foreach (Collider enemy in hitEnemies)
            {
                if (hitEnemiesDuringCharge.Contains(enemy)) continue;

                hitEnemiesDuringCharge.Add(enemy);

                if (enemy.TryGetComponent(out AIHealth enemyHealth))
                {
                    int totalDamage = Mathf.RoundToInt(damage * chargeDamageMultiplier);
                    enemyHealth.TakeDamage(totalDamage);
                    Debug.Log($"Charged hit: {totalDamage} damage to {enemy.name}");
                }

                if (enemy.TryGetComponent(out Rigidbody rb))
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
                }

                if (enemy.TryGetComponent(out Animator enemyAnimator))
                {
                    enemyAnimator.SetTrigger("Knockback");
                }

                if (isInUltimate && Time.time >= ultimateEndTime)
                {
                    transform.localScale = originalScale;
                    isInUltimate = false;
                    Debug.Log("Beast Ultimate ended, reverting to normal.");
                }

            }
        }
        else if (isCharging)
        {
            // Stop charging
            isCharging = false;
            animator.SetBool("isCharging", false);
            Debug.Log("Stopped charging");
        }

        wasCharging = isCharging;
    }

    public override void PerformAttack()
    {
        PerformAttack(damage);
    }

    public void PerformAttack(int attackDamage)
    {
        lastAttackTime = Time.time;

        if (!isCharging)
        {
            animator.SetTrigger("Attack");
        }

        int finalDamage = isInUltimate ? Mathf.RoundToInt(attackDamage * damageMultiplierDuringUltimate) : attackDamage;

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, range, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out AIHealth enemyHealth))
            {
                enemyHealth.TakeDamage(finalDamage);
                Debug.Log($"Normal hit: {finalDamage} damage to {enemy.name}");
            }
        }

        if (impactEffect != null)
        {
            Instantiate(impactEffect, attackPoint.position, Quaternion.identity);
        }
    }


    private void TriggerUltimate()
    {
        if (animator == null)
        {
            Debug.LogError("Animator is null!");
            return;
        }

        animator.SetTrigger(UltimateTrigger);

        if (ultimateSound != null)
        {
            audioSource.PlayOneShot(ultimateSound);
        }

        lastAttackTime = Time.time;

        // Begin transformation
        transform.localScale = originalScale * 2f;
        isInUltimate = true;
        ultimateEndTime = Time.time + ultimateDuration;

        Debug.Log("Beast has entered Ultimate state!");
    }


    public void CastUltimate()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, ultimateRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out AIHealth health))
            {
                health.Die();
                Debug.Log($"Beast Ultimate hit: {enemy.name}");
            }
        }

        Debug.Log($"Beast Ultimate affected {hitEnemies.Length} enemies!");
    }

    private void SpawnUltimateEffect()
    {
        if (ultimateEffectPrefab == null) return;

        if (activeUltimateEffect != null)
            Destroy(activeUltimateEffect);

        activeUltimateEffect = Instantiate(ultimateEffectPrefab, transform.position, Quaternion.identity, transform);
    }

    public void StopUltimateEffect()
    {
        if (activeUltimateEffect != null)
        {
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

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = isCharging ? Color.yellow : Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, range);
        }
    }
}
