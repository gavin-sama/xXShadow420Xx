using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public Animator animator;

    [Header("Movement Settings")]
    public float speedWalk = 2;
    public float speedRun = 5;
    public float stopDistanceFromPlayer = 0.91f;

    [Header("Attack Settings")]
    public float attackRange = 1f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 10;
    public float attackAnimationDuration = 0.9f;

    [Header("Detection Settings")]
    public float viewRadius = 15;
    public float viewAngle = 90;
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    private Transform player;
    private float lastAttackTime;
    private bool isAttacking;
    private bool playerInAttackRange;
    private float attackStartTime;

    private AIHealth aiHealth; // Added to allow damage communication

    void Awake()
    {
        aiHealth = GetComponent<AIHealth>(); // Grab reference to AIHealth
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("PlayerObj").transform;

        navMeshAgent.speed = speedRun;
        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        playerInAttackRange = distanceToPlayer <= attackRange;

        if (!isAttacking || Time.time > attackStartTime + attackAnimationDuration)
        {
            if (playerInAttackRange)
            {
                AttackBehavior();
            }
            else
            {
                ChaseBehavior();
            }
        }

        UpdateAnimations();
    }

    void ChaseBehavior()
    {
        isAttacking = false;
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(player.position);
        navMeshAgent.speed = speedRun;
    }

    void AttackBehavior()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            StartAttack();
        }
    }

    void StartAttack()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0;
        if (directionToPlayer != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToPlayer);
        }

        isAttacking = true;
        attackStartTime = Time.time;
        lastAttackTime = Time.time;
    }

    public void ApplyAttackDamage()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float damageRange = attackRange + 0.7f;

        if (distanceToPlayer <= damageRange)
        {
            PlayerStats playerHealth = player.GetComponent<PlayerStats>();
            DamageIndicator damageIndicator = player.GetComponent<DamageIndicator>();

            if (playerHealth != null)
            {
                damageIndicator.Flash();
                playerHealth.TakeDamage(attackDamage);
            }
        }
        else
        {
            Debug.Log("Player moved out of range at punch impact — attack missed!");
        }
    }

    public void TakeDamage(int damage)
    {
        if (aiHealth != null)
        {
            aiHealth.TakeDamage(damage); // This forwards damage to AIHealth
        }
    }

    void UpdateAnimations()
    {
        bool shouldRun = !isAttacking && navMeshAgent.velocity.magnitude > 0.1f && !playerInAttackRange;
        animator.SetBool("isRunning", shouldRun);

        animator.SetBool("isIdle", navMeshAgent.velocity.magnitude < 0.1f && !isAttacking);
        animator.SetBool("isAttacking", isAttacking);

        if (isAttacking && Time.time - attackStartTime < 0.1f)
        {
            animator.Play("Attack", 0, 0f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange + 0.4f);
    }
}
