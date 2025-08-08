using UnityEngine;

public class ShortRangeEnemy : BaseAIController
{
    public float attackRange = 0.6f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;
    private bool isAttacking;

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

    protected override void HandleAI()
    {
        if (player == null)
        {
            Debug.LogWarning("ShortRangeEnemy: No player found.");
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);

        isAttacking = currentState.IsTag("Attack");

        // Debug current animation state
        Debug.Log($"ShortRangeEnemy: Current animation state is '{currentState.fullPathHash}', isAttacking={isAttacking}");

        if (isAttacking)
        {
            StopMovement();
            return;
        }

        if (distanceToPlayer > attackRange)
        {
            Debug.Log("ShortRangeEnemy: Chasing player...");
            ChasePlayer();
        }
        else
        {
            Debug.Log("ShortRangeEnemy: Within attack range.");

            StopMovement();
            FacePlayer();

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                animator.ResetTrigger("Attack"); // Optional: prevent overlap
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
                Debug.Log("ShortRangeEnemy: Attack triggered.");
            }
        }
    }

    private void FacePlayer()
    {
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0;
        if (lookDir.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(lookDir);
            Debug.Log("ShortRangeEnemy: Facing player.");
        }
    }

    public void DealMeleeDamage()
    {
        if (player == null)
        {
            Debug.LogWarning("ShortRangeEnemy: No player to damage.");
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackRange + 0.5f && !playerMovement.isInvincible)
        {
            player.GetComponent<PlayerStats>()?.TakeDamage(20);
            Debug.Log("ShortRangeEnemy: Melee attack hit the player.");
        }
        else
        {
            Debug.Log("ShortRangeEnemy: Player out of melee range — attack missed.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
