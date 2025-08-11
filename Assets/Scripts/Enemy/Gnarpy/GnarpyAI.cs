using UnityEngine;

public class GnarpyAI : BaseAIController
{
    public float attackRange = 0.6f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;
    private bool isDead = false;

    PlayerMovement playerMovement;

    public void AssignPlayer(Transform playerTransform)
    {
        player = playerTransform;
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    protected override void Awake()
    {
        base.Awake();
        navMeshAgent.stoppingDistance = attackRange;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
    }

    private void FacePlayer()
    {
        if (player == null) return;

        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0; // Prevent tilting up/down

        if (lookDir.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }

    protected override void HandleAI()
    {
        if (isDead || player == null || PlayerStats.isUndetectable)
        {
            StopMovement();
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", true);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        bool isAttacking = currentState.IsTag("Attack");

        if (isAttacking)
        {
            StopMovement();
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", false);
            return;
        }

        if (distanceToPlayer > attackRange)
        {
            ChasePlayer();
            animator.SetBool("isWalking", true);
            animator.SetBool("isIdle", false);
        }
        else
        {
            StopMovement();
            FacePlayer();

            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", false);

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                animator.ResetTrigger("Attack");
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
            }
        }
    }

    public void DealMeleeDamage()
    {
        if (player == null || playerMovement == null || playerMovement.isInvincible) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackRange + 0.5f)
        {
            player.GetComponent<PlayerStats>()?.TakeDamage(20);
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        StopMovement();
        animator.SetTrigger("Death");

        // Optional: disable collider, navmesh, etc.
        navMeshAgent.enabled = false;
        GetComponent<Collider>().enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
