using UnityEngine;
using System.Collections.Generic;

public class BrawlerAttack : PlayerAttackBase
{
    [Header("Melee Settings")]
    public int damage = 20;
    public float range = 1.5f;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public Animator animator;
    public GameObject impactEffect;

    [Header("Combo Settings")]
    public float comboResetTime = 1.2f; // Time before combo resets
    private int comboStep = 0;
    private float lastComboTime;

    private void Update()
    {
        // Detect attack input
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            PerformComboAttack();
        }

        // Reset combo if too much time has passed
        if (Time.time - lastComboTime > comboResetTime)
        {
            comboStep = 0;
        }
    }

    public void PerformComboAttack()
    {
        lastAttackTime = Time.time;
        lastComboTime = Time.time;

        // Cycle through combo steps: 1 2, 3
        comboStep = (comboStep % 3) + 1;

        // Trigger corresponding animation
        switch (comboStep)
        {
            case 1:
                animator.SetTrigger("Punch1");
                break;
            case 2:
                animator.SetTrigger("Punch2");
                break;
            case 3:
                animator.SetTrigger("Punch3");
                break;
        }

        // Detect enemies in range
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, range, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out AIHealth enemyHealth))
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log($"Combo hit {comboStep}: {damage} damage to {enemy.name}");
            }
        }

        // Spawn impact effect
        if (impactEffect != null)
        {
            Instantiate(impactEffect, attackPoint.position, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, range);
        }
    }
}
