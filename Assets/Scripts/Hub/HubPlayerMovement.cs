using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class HubPlayerMovement : MonoBehaviour
{
    public PlayerClassData playerClassData;

    public Animator animator;
    public Camera playerCamera;

    // AUDIO
    public AudioClip runClip;
    public AudioClip idleClip;
    private AudioSource audioSource;

    // STATS
    public PlayerStats playerStats;
    public float walkSpeed;
    public float runSpeed;
    public float lookSpeed;
    public float lookXLimit;
    public float gravity;
    public float defaultHeight;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationY = 0;
    private CharacterController characterController;

    private bool canMove = true;

    private PlayerInput _playerInput;
    private InputAction _move;
    private InputAction _sprint;
    private InputAction _look;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private bool isRunning;

    // Omnimovement variables
    private Vector2 smoothedMoveInput;
    public float inputSmoothTime = 0.1f;
    private Vector2 inputVelocity;

    // Movement direction enum for cleaner animation handling
    public enum MovementDirection
    {
        Idle,
        Forward,
        Backward,
        Left,
        Right,
        ForwardLeft,
        ForwardRight,
        BackwardLeft,
        BackwardRight
    }

    private MovementDirection currentMovementDirection = MovementDirection.Idle;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = animator ?? GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        SetInputActions();

        playerStats = playerStats ?? GetComponent<PlayerStats>();
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        if (playerClassData != null)
        {
            ApplyClassData();
        }

        
    }

    void Update()
    {
        UpdateInputs();
        HandleMovement();
        HandleLooking();
        HandleAnimation();
    }

    void ApplyClassData()
    {
        walkSpeed = playerClassData.walkSpeed;
        runSpeed = playerClassData.runSpeed;
        gravity = playerClassData.gravity;
    }

    private void UpdateInputs()
    {
        moveInput = _move.ReadValue<Vector2>();

        // Smooth the input for more natural movement
        smoothedMoveInput = Vector2.SmoothDamp(smoothedMoveInput, moveInput, ref inputVelocity, inputSmoothTime);

        lookInput = _look.ReadValue<Vector2>();
        isRunning = _sprint.IsPressed() && smoothedMoveInput.magnitude > 0.1f;

        // Determine movement direction based on input
        currentMovementDirection = GetMovementDirection(smoothedMoveInput);
    }

    private MovementDirection GetMovementDirection(Vector2 input)
    {
        const float threshold = 0.1f;

        if (input.magnitude < threshold)
            return MovementDirection.Idle;

        float x = input.x;
        float y = input.y;

        // Ignore diagonals: pick the dominant axis only
        if (Mathf.Abs(y) > Mathf.Abs(x))
        {
            return y > 0 ? MovementDirection.Forward : MovementDirection.Backward;
        }
        else
        {
            return x > 0 ? MovementDirection.Right : MovementDirection.Left;
        }
    }


    private void HandleMovement()
    {
        if (!canMove)
        {
            moveDirection.y -= gravity * Time.deltaTime;
            characterController.Move(moveDirection * Time.deltaTime);
            return;
        }

        // Calculate movement based on camera-relative directions
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;

        // Remove y component for ground movement
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        float speed = isRunning ? runSpeed : walkSpeed;

        // Use smoothed input for movement calculation
        Vector3 desiredMove = forward * smoothedMoveInput.y + right * smoothedMoveInput.x;

        // Normalize diagonal movement to prevent speed boost
        if (desiredMove.magnitude > 1f)
            desiredMove.Normalize();

        float verticalVelocity = moveDirection.y;
        moveDirection = desiredMove * speed;
        moveDirection.y = verticalVelocity;

        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleLooking()
    {
        if (!canMove || Cursor.lockState != CursorLockMode.Locked) return;

        rotationY -= lookInput.y;
        rotationY = Mathf.Clamp(rotationY, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationY, 0, 0);
        transform.rotation *= Quaternion.Euler(0, lookInput.x * lookSpeed, 0);
    }

    private void HandleAnimation()
    {
        // Set movement speed for blend trees
        float movementMagnitude = smoothedMoveInput.magnitude;
        float speedMultiplier = isRunning ? 2f : 1f;

        // Set animator parameters for omnidirectional movement
        //animator.SetFloat("Speed", movementMagnitude * speedMultiplier);
        //animator.SetFloat("InputX", smoothedMoveInput.x);
        //animator.SetFloat("InputY", smoothedMoveInput.y);

        // Set boolean states
       // animator.SetBool("isMoving", movementMagnitude > 0.1f);
        //animator.SetBool("isRunning", isRunning);
        animator.SetBool("isWalking", movementMagnitude > 0.1f && !isRunning);
        animator.SetBool("isIdle", movementMagnitude <= 0.1f);


        // Handle audio
        if (movementMagnitude > 0.1f)
        {
            PlaySound(runClip);
        }
        else
        {
            PlaySound(idleClip);
        }
    }


    private void SetInputActions()
    {
        var actions = _playerInput.actions;
        _move = actions["Move"];
        _sprint = actions["Sprint"];
        _look = actions["Look"];
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource.clip != clip)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void PlayOneShot(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    // Debug method to visualize current movement direction
    void OnGUI()
    {
        if (Application.isPlaying)
        {
            GUI.Label(new Rect(10, 10, 200, 20), $"Direction: {currentMovementDirection}");
            GUI.Label(new Rect(10, 30, 200, 20), $"Input: {smoothedMoveInput}");
            GUI.Label(new Rect(10, 50, 200, 20), $"Running: {isRunning}");
        }
    }
    
    //Ian added this to stop Hub Character from moving in the menus
    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;

        if (!enabled)
        {
            // Optional: reset velocity so player stops immediately
            moveDirection = Vector3.zero;
            
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", true);
        }
    }
}