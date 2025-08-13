using UnityEngine;
using UnityEngine.AI;

public abstract class BaseAIController : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public Animator animator;
    public AudioSource audioSource;
    public Transform player;

    [Header("Movement Settings")]
    public float speedWalk = 2;
    public float speedRun = 5;
    public float stopDistanceFromPlayer = 0.91f;

    [Header("Detection Settings")]
    public float viewRadius = 15;
    public float viewAngle = 90;
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    [Header("Audio Settings")]
    public AudioClip runClip;
    public AudioClip hurtClip;
    public AudioClip attackClip;

    private AIHealth aiHealth;

    // NavMeshObstacle component for better local avoidance when stopped
    private NavMeshObstacle obstacle;

    protected virtual void Awake()
    {
        aiHealth = GetComponent<AIHealth>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (navMeshAgent != null)
        {
            // Set agent radius to approximate AI size
            navMeshAgent.radius = 0.6f;

            // Use high quality obstacle avoidance for better local steering
            navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

            // Randomize avoidance priority (0-99), lower value = higher priority
            navMeshAgent.avoidancePriority = Random.Range(30, 70);
        }

        // Setup NavMeshObstacle for carving (avoid overlap when stopped)
        obstacle = GetComponent<NavMeshObstacle>();
        if (obstacle != null)
        {
            obstacle.carving = true;
            obstacle.enabled = false; // Start disabled, enabled only when stopped
        }
    }

    protected virtual void Update()
    {
        // Optional pause check, replace with your own pause manager if needed
        if (player == null || Time.timeScale == 0f) return;

        HandleAI();
        UpdateAnimations();
    }

    protected abstract void HandleAI(); // Child scripts define this

    protected virtual void ChasePlayer()
    {
        if (PlayerStats.isUndetectable)
        {
            StopMovement();
            return;
        }

        // Disable obstacle and enable agent movement
        if (obstacle != null && obstacle.enabled)
        {
            obstacle.enabled = false;
            navMeshAgent.enabled = true;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= stopDistanceFromPlayer)
        {
            StopMovement();
            Debug.Log("BaseAIController: Stopping chase due to proximity.");
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

    protected virtual void StopMovement()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        transform.position = navMeshAgent.nextPosition;

        // Enable NavMeshObstacle and disable agent movement to avoid overlap
        if (obstacle != null && !obstacle.enabled)
        {
            navMeshAgent.enabled = false;
            obstacle.enabled = true;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        if (aiHealth != null)
            aiHealth.TakeDamage(damage);

        if (hurtClip != null)
            audioSource.PlayOneShot(hurtClip);
    }

    public void ApplyKnockback(Vector3 direction, float force, float duration)
    {
        StartCoroutine(KnockbackRoutine(direction, force, duration));
    }

    private System.Collections.IEnumerator KnockbackRoutine(Vector3 direction, float force, float duration)
    {
        navMeshAgent.isStopped = true; // stop AI movement

        float timer = 0f;
        while (timer < duration)
        {
            transform.position += direction * force * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        navMeshAgent.isStopped = false; // resume AI movement
    }

    protected virtual void UpdateAnimations()
    {
        animator.SetBool("isWalking", navMeshAgent.velocity.magnitude > 0.1f);
        animator.SetBool("isIdle", navMeshAgent.velocity.magnitude < 0.1f);
    }
}
