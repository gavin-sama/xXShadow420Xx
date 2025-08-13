using UnityEngine;

public class BrawlerAttack : PlayerAttackBase
{
    [Header("Melee Settings")]
    public int damage = 20;
    public float range = 1.5f;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public GameObject impactEffect;

    [Header("Knockback Settings (Punch 2 Only)")]
    [Range(0f, 50f)]
    public float knockbackForce = 10f; // Adjustable force

    [Header("Combo Settings")]
    public Animator animator;
    public float comboResetTime = 1.2f;

    private int comboStep = 0;
    private float lastComboTime;
    private bool inputBuffered = false;
    private bool isAttacking = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            inputBuffered = true;

            if (!isAttacking)
            {
                StartCombo();
            }
        }

        if (Time.time - lastComboTime > comboResetTime)
        {
            ResetCombo();
        }
    }

    private void StartCombo()
    {
        comboStep = 1;
        lastComboTime = Time.time;
        isAttacking = true;

        animator.CrossFade("PunchCombo", 0.1f);
        animator.SetFloat("PunchIndex", comboStep - 1);

        DealDamage();
    }

    public void TryContinueCombo()
    {
        if (inputBuffered && comboStep < 2) // Only allow up to Punch 2 now
        {
            comboStep++;
            lastComboTime = Time.time;
            inputBuffered = false;

            animator.CrossFade("PunchCombo", 0.1f);
            animator.SetFloat("PunchIndex", comboStep - 1);

            DealDamage();
        }
        else
        {
            ResetCombo();
        }
    }

    private void DealDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, range, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out AIHealth enemyHealth))
            {
                enemyHealth.TakeDamage(damage);

                if (comboStep == 2 && enemy.TryGetComponent(out BaseAIController aiController))
                {
                    Vector3 knockbackDir = (enemy.transform.position - transform.position).normalized;
                    knockbackDir.y = 0; // keep it horizontal
                    aiController.ApplyKnockback(knockbackDir, knockbackForce, 0.2f);
                }


                Debug.Log($"Combo hit {comboStep}: {damage} damage to {enemy.name}");
            }
        }

        if (impactEffect != null)
        {
            Instantiate(impactEffect, attackPoint.position, Quaternion.identity);
        }
    }

    private void ResetCombo()
    {
        comboStep = 0;
        inputBuffered = false;
        isAttacking = false;
        animator.SetFloat("PunchIndex", -1);
    }
}