using UnityEngine;

public class RangedAttack : PlayerAttackBase
{
    private Animator animator;
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float projectileSpeed = 30f;

    [Header("Audio Feedback")]
    public AudioClip castSound;
    private AudioSource audioSource;


    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (shootPoint == null)
        {
            Debug.LogWarning("ShootPoint not assigned. Creating default.");
            GameObject shootObj = new GameObject("ShootPoint");
            shootObj.transform.parent = transform;
            shootObj.transform.localPosition = new Vector3(0, 1.5f, 1f);
            shootPoint = shootObj.transform;
        }

        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab not assigned to RangedAttack!");
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            animator.SetTrigger(AttackTrigger); // Play attack animation
            lastAttackTime = Time.time;
        }
    }

    // Animation Event calls this during the animation when casting completes
    public void CastProjectile()
    {
        if (projectilePrefab == null || shootPoint == null)
        {
            Debug.LogError("Missing projectile prefab or shoot point.");
            return;
        }

        // Aim from center of screen
        Vector3 direction = transform.forward;
        if (Camera.main != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                direction = (hit.point - shootPoint.position).normalized;
            }
        }
        else
        {
            Debug.LogWarning("Main camera not found. Defaulting to forward.");
        }

        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.LookRotation(direction));
        projectile.SetActive(true); // Just in case it's off
        if (projectile.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        if (castSound != null)
        {
            audioSource.PlayOneShot(castSound);
        }

        Debug.Log("Fireball spawned: " + projectile.name);
    }

    public override void PerformAttack()
    {
        // Animation event will handle actual casting
    }

    // Optional: Animation Event at end of attack animation
    public void EndAttack()
    {
        animator.ResetTrigger(AttackTrigger);
    }
}
