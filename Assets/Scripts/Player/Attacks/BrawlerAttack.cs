using UnityEngine;

public class BrawlerAttack : PlayerAttackBase
{
    [Header("Melee Settings")]
    public int damage = 20;
    public float range = 1.5f;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public GameObject impactEffect;

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

            // Start combo if not attacking
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
        if (inputBuffered && comboStep < 3)
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

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, range);
        }
    }
}
