using UnityEngine.AI;
using UnityEngine;

public class ShortRangeEnemy : BaseAIController
{
    [Header("Short Range Attack")]
    public float attackRange = 1f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 10;
    public float attackAnimationDuration = 0.9f;
    public AudioClip attackClip;

    private float lastAttackTime;
    private bool isAttacking;
    private float attackStartTime;

    protected override void HandleAI()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool inAttackRange = distanceToPlayer <= attackRange;

        if (!isAttacking || Time.time > attackStartTime + attackAnimationDuration)
        {
            if (inAttackRange)
                TryAttack();
            else
                ChasePlayer();
        }
    }

    void TryAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;

            Vector3 lookDir = (player.position - transform.position).normalized;
            lookDir.y = 0;
            transform.rotation = Quaternion.LookRotation(lookDir);

            isAttacking = true;
            attackStartTime = Time.time;
            lastAttackTime = Time.time;

            if (attackClip != null)
                audioSource.PlayOneShot(attackClip);

            animator.SetTrigger("Attack"); // Use Trigger for animation
        }
    }

    public void ApplyAttackDamage()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange + 0.5f)
        {
            PlayerStats playerHealth = player.GetComponent<PlayerStats>();
            playerHealth?.TakeDamage(attackDamage);
        }
    }

    protected override void UpdateAnimations()
    {
        base.UpdateAnimations();
        animator.SetBool("isAttacking", isAttacking);

        if (isAttacking && Time.time - attackStartTime < 0.1f)
        {
            animator.Play("Attack", 0, 0f);
        }
    }
}
