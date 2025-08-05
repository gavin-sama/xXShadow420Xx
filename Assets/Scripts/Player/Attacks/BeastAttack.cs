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

    private void Update()
    {
        bool shouldCharge = Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W);

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

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, range, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out AIHealth enemyHealth))
            {
                enemyHealth.TakeDamage(attackDamage);
                Debug.Log($"Normal hit: {attackDamage} damage to {enemy.name}");
            }
        }

        if (impactEffect != null)
        {
            Instantiate(impactEffect, attackPoint.position, Quaternion.identity);
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
