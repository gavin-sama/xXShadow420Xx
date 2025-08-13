using UnityEngine;
using UnityEngine.AI;

public class GnarpyAI : BaseAIController
{
    public float attackRange = 0.6f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;
    private bool hasExploded = false;

    PlayerMovement playerMovement;

    [Header("Explosion Settings")]
    public GameObject explosionPrefab;
    public AudioClip explosionClip;

    public void AssignPlayer(Transform playerTransform)
    {
        player = playerTransform;
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    protected override void Awake()
    {
        base.Awake();

        navMeshAgent.stoppingDistance = attackRange;
        stopDistanceFromPlayer = attackRange;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
    }

    protected override void UpdateAnimations()
    {
        base.UpdateAnimations(); // No attack animation anymore
    }

    protected override void HandleAI()
    {
        if (player == null || hasExploded)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            ChasePlayer();
        }
        else
        {
            StopMovement();
            FacePlayer();

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                hasExploded = true;

                Debug.Log("GnarpyAI: Suicide range reached — triggering self-destruct.");
                SelfDestruct();
            }
        }
    }

    protected override void ChasePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            StopMovement();
            return;
        }

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(player.position);
        navMeshAgent.speed = speedRun;

        if (runClip != null && !audioSource.isPlaying && navMeshAgent.velocity.magnitude > 0.1f)
        {
            audioSource.clip = runClip;
            audioSource.loop = true;
            audioSource.Play();
        }

        if (navMeshAgent.velocity.magnitude < 0.1f && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    protected override void StopMovement()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector3.zero;

        if (navMeshAgent.updatePosition)
            transform.position = navMeshAgent.nextPosition;
    }

    private void FacePlayer()
    {
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0;
        if (lookDir.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }

    public void SelfDestruct()
    {
        Debug.Log("GnarpyAI: Self-destruct triggered.");
        animator.SetTrigger("Die"); // Explosion happens via animation event
        navMeshAgent.isStopped = true;
        enabled = false; // Disable AI logic
    }

    public void ExplodeDamage()
    {
        if (player == null) return;

        GameObject playerGO = player.gameObject;

        PlayerStats playerStats = playerGO.GetComponent<PlayerStats>();
        PlayerMovement playerMovement = playerGO.GetComponent<PlayerMovement>();

        if (playerStats == null || playerMovement == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange + 0.5f && !playerMovement.isInvincible)
        {
            Debug.Log("Player took damage");
            playerStats.TakeDamage(20);
            Debug.Log("GnarpyAI: Explosion damage dealt.");
        }
        else
        {
            Debug.Log("GnarpyAI: Explosion missed — player out of range or invincible.");
        }

        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        if (explosionClip != null)
            audioSource.PlayOneShot(explosionClip);

        AIHealth health = GetComponent<AIHealth>();
        if (health != null)
        {
            health.Die();
        }
        else
        {
            Debug.LogWarning("GnarpyAI: AIHealth component missing — fallback destroy.");
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
