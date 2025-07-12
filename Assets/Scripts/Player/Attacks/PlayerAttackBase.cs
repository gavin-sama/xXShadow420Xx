using UnityEngine;

public abstract class PlayerAttackBase : MonoBehaviour
{
    [Header("General Settings")]
    public float attackCooldown = 1f;

    protected float lastAttackTime;
    protected bool canAttack => Time.time >= lastAttackTime + attackCooldown;

    public abstract void PerformAttack(); // Used by subclasses if needed
}
