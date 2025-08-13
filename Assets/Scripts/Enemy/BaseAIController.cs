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

    protected AIHealth aiHealth;

    protected virtual void Awake()
    {
        aiHealth = GetComponent<AIHealth>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        // Optional pause check, replace with your own pause manager if needed
        if (player == null || Time.timeScale == 0f) return;

        HandleAI();
        UpdateAnimations();
    }

    protected abstract void HandleAI(); // Child scripts define this

    protected void ChasePlayer()
    {
        if (PlayerStats.isUndetectable)
        {
            StopMovement();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check stopping distance
        if (distanceToPlayer <= stopDistanceFromPlayer)
        {
            StopMovement();
            Debug.Log("BaseAIController: Stopping chase due to proximity.");
            return;
        }

        // Chase logic
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

    protected void StopMovement()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        transform.position = navMeshAgent.nextPosition;
    }

    public virtual void TakeDamage(int damage)
    {
        if (aiHealth != null)
            aiHealth.TakeDamage(damage);

        if (hurtClip != null)
            audioSource.PlayOneShot(hurtClip);
    }

    protected virtual void UpdateAnimations()
    {
        animator.SetBool("isWalking", navMeshAgent.velocity.magnitude > 0.1f);
        animator.SetBool("isIdle", navMeshAgent.velocity.magnitude < 0.1f);
    }
}
