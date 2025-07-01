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

    protected AIHealth aiHealth;

    protected virtual void Awake()
    {
        aiHealth = GetComponent<AIHealth>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if (player == null) return;

        HandleAI();
        UpdateAnimations();
    }

    protected abstract void HandleAI(); // Child scripts define this

    protected void ChasePlayer()
    {
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
