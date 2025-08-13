using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerClassData playerClassData;
    private Animator animator;
    public static Transform Instance;

    private PlayerInput playerInput; // Declare the missing variable

    void Awake()
    {
        Instance = transform;
        playerInput = GetComponent<PlayerInput>(); // Assign it here
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        // Re-enable input actions safely
        if (playerInput != null)
        {
            playerInput.actions?.Enable();
        }
    }
}
