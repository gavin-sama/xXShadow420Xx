using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerClass", menuName = "Player/PlayerClass")]
public class PlayerClassData : ScriptableObject
{
    public string className;

    // Movement Stats
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float gravity = 20f;
    public float crouchSpeed = 3f;

    // Animation Clips
    public RuntimeAnimatorController baseAnimatorController;
    public AnimationClip runAnim;
    public AnimationClip idleAnim;
    public AnimationClip attackAnim;

    // Abilities
    public GameObject specialAbilityPrefab;
}
