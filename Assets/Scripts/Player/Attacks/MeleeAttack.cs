using UnityEngine;

public class MeleeAttack : PlayerAttackBase
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
    public float chargeDamageMultiplier = 1.5f; // Optional: increase damage when charged

    private bool isCharging;
    private bool wasCharging; // Track previous charging state

    private void Update()
    {
        bool shouldCharge = Input.GetMouseButton(0) &&
                           (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.LeftShift));

        // Start charging attack
        if (shouldCharge && canAttack)
        {
            if (!isCharging)
            {
                isCharging = true;
                animator.SetBool("isCharging", true);
                Debug.Log("Started charging attack");
            }

            // Move forward during charge
            transform.position += transform.forward * Time.deltaTime * chargeSpeed;
        }
        // Release charge when buttons are released
        else if (isCharging && !shouldCharge)
        {
            // Release charge
            isCharging = false;
            lastAttackTime = Time.time;
            animator.SetBool("isCharging", false);

            // Calculate damage based on charge
            int finalDamage = Mathf.RoundToInt(damage * chargeDamageMultiplier);
            PerformAttack(finalDamage);

            Debug.Log("Released charge attack");
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

        // Only trigger attack animation if not charging
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