using UnityEngine;
using System.Collections;
public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 0.6f;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private Transform attackPoint;

    [Header("Feedback")]
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private ParticleSystem hitEffect;

    private Animator animator;
    private bool canAttack = true;
    private bool isAttacking = false;
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");

    private void Start()
    {
        animator = GetComponent<Animator>();

        // Ensure we have an attack point
        if (attackPoint == null)
        {
            Debug.LogError("No attack point assigned to PlayerAttack script. Creating one.");
            GameObject newAttackPoint = new GameObject("AttackPoint");
            newAttackPoint.transform.parent = transform;
            newAttackPoint.transform.localPosition = new Vector3(0, 0, 1); // Adjust as needed
            attackPoint = newAttackPoint.transform;
        }

        // Verify enemy layers are set
        if (enemyLayers.value == 0)
        {
            Debug.LogError("Enemy layers not set in PlayerAttack script!");
        }
    }

    private void Update()
    {
        // Only allow attack input when not currently attacking and cooldown is complete
        if (Input.GetMouseButtonDown(0) && canAttack && !isAttacking)
        {
            StartAttack();
        }

        // Debug info to monitor states
        if (Input.anyKeyDown)
        {
            Debug.Log($"Attack state - canAttack: {canAttack}, isAttacking: {isAttacking}");
        }

        if (Input.GetKeyDown(KeyCode.Z)) // Simulate attack
        {
            Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward, 1.2f, enemyLayers);
            foreach (Collider hit in hits)
            {
                Debug.Log("Trying to damage: " + hit.name);
                AIController ai = hit.GetComponent<AIController>();
                if (ai != null) ai.TakeDamage(10);
            }
        }

    }

    private void StartAttack()
    {
        // Safety check - if somehow we're already attacking, don't start again
        if (isAttacking)
        {
            Debug.LogWarning("StartAttack called while already attacking!");
            return;
        }

        isAttacking = true;
        canAttack = false;

        // Reset any lingering triggers before setting a new one
        animator.ResetTrigger(AttackTrigger);

        // Trigger the attack animation
        animator.SetTrigger(AttackTrigger);

        // Start cooldown coroutine
        StopAllCoroutines(); // Stop any existing cooldown coroutines
        StartCoroutine(AttackCooldown());

        // Play attack sound if available
        if (attackSound != null)
        {
            attackSound.Play();
        }

        Debug.Log("Attack started");
    }

    // This should be called via Animation Event at the exact frame when the attack hits
    public void OnAttackPoint()
    {
        // Prevent duplicate attack calls
        if (!isAttacking)
        {
            Debug.LogWarning("OnAttackPoint called but isAttacking is false - Animation Event might be misconfigured");
            return;
        }

        Debug.Log("OnAttackPoint called - Animation Event triggered");
        PerformDamage();
    }

    private void PerformDamage()
    {
        if (attackPoint == null)
        {
            Debug.LogError("Attack point is null when trying to perform damage!");
            return;
        }

        // Debug the attack point position
        Debug.Log($"Attacking at position: {attackPoint.position}, range: {attackRange}");

        // Use OverlapSphere for more reliable collision detection
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        Debug.Log($"Found {hitEnemies.Length} potential targets in range");

        bool hitAny = false;

        // Apply damage to each enemy
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy == null)
            {
                Debug.LogWarning("Null enemy collider detected!");
                continue;
            }

            Debug.Log($"Processing potential target: {enemy.name}, Layer: {LayerMask.LayerToName(enemy.gameObject.layer)}");

            AIHealth enemyHealth = enemy.GetComponent<AIHealth>();
            if (enemyHealth != null)
            {
                // Apply damage immediately
                enemyHealth.TakeDamage(attackDamage);
                hitAny = true;

                // Spawn hit effect at the enemy's position if available
                if (hitEffect != null)
                {
                    Instantiate(hitEffect, enemy.transform.position, Quaternion.identity);
                }

                Debug.Log($"Hit enemy: {enemy.name} for {attackDamage} damage. Current health: {enemyHealth.GetCurrentHealth()}");
            }
            else
            {
                Debug.LogWarning($"Enemy {enemy.name} has no AIHealth component!");
            }
        }

        if (!hitAny)
        {
            Debug.Log("No enemies hit");
        }
    }

    // Called by Animation Event at the end of attack animation
    public void EndAttack()
    {
        if (!isAttacking)
        {
            Debug.LogWarning("EndAttack called but isAttacking was already false!");
        }

        isAttacking = false;
        Debug.Log("Attack animation completed");

        // Make sure the trigger is reset
        animator.ResetTrigger(AttackTrigger);
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        // Draw a small sphere at the attack point to make it more visible
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(attackPoint.position, 0.1f);
    }
}