using UnityEngine;

public abstract class PlayerAttackBase : MonoBehaviour
{
    [Header("General Settings")]
    public float attackCooldown = 1f;

    protected float lastAttackTime;

    public virtual void PerformAttack() { }
    public virtual void PerformUltimate() { }
    protected bool canAttack => Time.time >= lastAttackTime + attackCooldown;
}
