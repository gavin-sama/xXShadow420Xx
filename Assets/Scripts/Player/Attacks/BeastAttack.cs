using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;

public class BeastAttack : PlayerAttackBase
{
    [Header("Melee Settings")]
    public int damage = 20;
    public float range = 1.5f;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public Animator animator;
    public GameObject impactEffect;

    [Header("Ultimate Settings")]
    public float ultimateDuration = 80f;
    public float damageMultiplierDuringUltimate = 2f;
    public float damageReductionFactor = 0.5f;
    public AudioClip ultimateSound;
    private AudioSource audioSource;

    private bool isInUltimate;
    private float ultimateEndTime;
    private Vector3 originalScale;
    private static readonly int UltimateTrigger = Animator.StringToHash("Ultimate");

    [Header("Charge Settings")]
    public float chargeSpeed = 3f;
    public float chargeDamageMultiplier = 1.5f;
    public GameObject groundBreakEffect;
    public Transform groundEffectPoint;

    private bool isCharging;
    private HashSet<Collider> hitEnemiesDuringCharge = new HashSet<Collider>();

    private CinemachineOrbitalFollow cameraOrbital;
    private GameObject freeLookCameraObject;

    private struct OrbitSettings
    {
        public float TopHeight, TopRadius;
        public float CenterHeight, CenterRadius;
        public float BottomHeight, BottomRadius;
    }

    private OrbitSettings originalOrbits;

    private void Start()
    {
        freeLookCameraObject = GameObject.Find("FreeLook Camera");
        if (freeLookCameraObject != null)
        {
            cameraOrbital = freeLookCameraObject.GetComponent<CinemachineOrbitalFollow>();

            if (cameraOrbital != null)
            {
                originalOrbits.TopHeight = cameraOrbital.Orbits.Top.Height;
                originalOrbits.TopRadius = cameraOrbital.Orbits.Top.Radius;

                originalOrbits.CenterHeight = cameraOrbital.Orbits.Center.Height;
                originalOrbits.CenterRadius = cameraOrbital.Orbits.Center.Radius;

                originalOrbits.BottomHeight = cameraOrbital.Orbits.Bottom.Height;
                originalOrbits.BottomRadius = cameraOrbital.Orbits.Bottom.Radius;
            }
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        originalScale = transform.localScale;
    }

    private void Update()
    {
        HandleChargeInput();
        HandleUltimateTimer();

        // Left Mouse Button attack
        if (Input.GetMouseButtonDown(0) && canAttack && !isCharging)
        {
            PerformAttack();
        }
    }

    private void HandleChargeInput()
    {
        bool shouldCharge = Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W);

        if (shouldCharge && canAttack)
        {
            if (!isCharging)
            {
                isCharging = true;
                animator.SetBool("isCharging", true);
                hitEnemiesDuringCharge.Clear();

                if (groundBreakEffect != null && groundEffectPoint != null)
                {
                    Instantiate(groundBreakEffect, groundEffectPoint.position, Quaternion.identity);
                }

                Debug.Log("Started charging");
            }

            transform.position += transform.forward * Time.deltaTime * chargeSpeed;

            Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, 2f, enemyLayers);
            foreach (Collider enemy in hitEnemies)
            {
                if (hitEnemiesDuringCharge.Contains(enemy)) continue;

                hitEnemiesDuringCharge.Add(enemy);

                if (enemy.TryGetComponent(out AIHealth enemyHealth))
                {
                    int totalDamage = Mathf.RoundToInt(damage * chargeDamageMultiplier);
                    if (isInUltimate)
                        totalDamage = Mathf.RoundToInt(totalDamage * damageMultiplierDuringUltimate);

                    enemyHealth.TakeDamage(totalDamage);
                    Debug.Log($"Charged hit: {totalDamage} damage to {enemy.name}");
                }

                if (enemy.TryGetComponent(out Rigidbody rb))
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
                }

                if (enemy.TryGetComponent(out Animator enemyAnimator))
                {
                    enemyAnimator.SetTrigger("Knockback");
                }
            }
        }
        else if (isCharging)
        {
            isCharging = false;
            animator.SetBool("isCharging", false);
            Debug.Log("Stopped charging");
        }
    }

    private void HandleUltimateTimer()
    {
        if (isInUltimate && Time.time >= ultimateEndTime)
        {
            transform.localScale = originalScale;
            isInUltimate = false;

            if (cameraOrbital != null)
            {
                cameraOrbital.Orbits.Top.Height = originalOrbits.TopHeight;
                cameraOrbital.Orbits.Top.Radius = originalOrbits.TopRadius;

                cameraOrbital.Orbits.Center.Height = originalOrbits.CenterHeight;
                cameraOrbital.Orbits.Center.Radius = originalOrbits.CenterRadius;

                cameraOrbital.Orbits.Bottom.Height = originalOrbits.BottomHeight;
                cameraOrbital.Orbits.Bottom.Radius = originalOrbits.BottomRadius;

                Debug.Log("Camera reverted to original settings after Beast Ultimate.");
            }

            Debug.Log("Beast Ultimate ended, reverting to normal.");
        }
    }

    public override void PerformAttack()
    {
        PerformAttack(damage);
    }

    public void PerformAttack(int attackDamage)
    {
        lastAttackTime = Time.time;

        if (!isCharging)
        {
            animator.SetTrigger("Attack");
        }

        int finalDamage = isInUltimate ? Mathf.RoundToInt(attackDamage * damageMultiplierDuringUltimate) : attackDamage;

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, range, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out AIHealth enemyHealth))
            {
                enemyHealth.TakeDamage(finalDamage);
                Debug.Log($"Normal hit: {finalDamage} damage to {enemy.name}");
            }
        }

        if (impactEffect != null)
        {
            GameObject effectInstance = Instantiate(impactEffect, attackPoint.position, Quaternion.identity);
            Destroy(effectInstance, 2f); // Auto destroy after 2 seconds
        }
    }

    public override void PerformUltimate()
    {
        TriggerUltimate();
    }

    private void TriggerUltimate()
    {
        if (animator == null)
        {
            Debug.LogError("Animator is null!");
            return;
        }

        animator.SetTrigger(UltimateTrigger);

        if (ultimateSound != null)
        {
            audioSource.PlayOneShot(ultimateSound);
        }

        lastAttackTime = Time.time;

        transform.localScale = originalScale * 3.2f;
        isInUltimate = true;
        ultimateEndTime = Time.time + ultimateDuration;

        if (cameraOrbital != null)
        {
            cameraOrbital.Orbits.Top.Height = originalOrbits.TopHeight + 10f;
            cameraOrbital.Orbits.Top.Radius = originalOrbits.TopRadius + 5f;

            cameraOrbital.Orbits.Center.Height = originalOrbits.CenterHeight + 6f;
            cameraOrbital.Orbits.Center.Radius = originalOrbits.CenterRadius + 4f;

            cameraOrbital.Orbits.Bottom.Height = originalOrbits.BottomHeight + 3f;
            cameraOrbital.Orbits.Bottom.Radius = originalOrbits.BottomRadius + 3f;

            Debug.Log("Camera zoomed out and elevated for Beast Ultimate.");
        }

        Debug.Log("Beast has entered Ultimate state!");
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = isCharging ? Color.yellow : Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, range);
        }
    }

    public bool IsInUltimate => isInUltimate;
    public float DamageReductionFactor => damageReductionFactor;
}
