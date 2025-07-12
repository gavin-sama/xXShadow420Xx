using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
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
    public float gravity;
    public float lookSpeed;
    public float lookXLimit;
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
        lookInput = _look.ReadValue<Vector2>();
        isRunning = _sprint.IsPressed() && moveInput.y > 0.1f;
    }

    private void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float movementMultiplier = 1.0f;
        float speed = isRunning ? runSpeed : walkSpeed;

        float curSpeedX = canMove ? speed * moveInput.y * movementMultiplier : 0;
        float curSpeedY = canMove ? speed * moveInput.x * movementMultiplier : 0;

        float verticalVelocity = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
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
        if (isRunning)
        {
            PlaySound(runClip);
            SetAnimationState("isRunning");
        }
        else if (moveInput.y > 0.1f)
        {
            PlaySound(runClip);
            SetAnimationState("isWalking");
        }
        else
        {
            PlaySound(idleClip);
            SetAnimationState("isIdle");
        }
    }

    private void SetInputActions()
    {
        var actions = _playerInput.actions;
        _move = actions["Move"];
        _sprint = actions["Sprint"];
        _look = actions["Look"];
    }

    private void SetAnimationState(string activeState)
    {
        animator.SetBool("isRunning", activeState == "isRunning");
        animator.SetBool("isWalking", activeState == "isWalking");
        animator.SetBool("isIdle", activeState == "isIdle");
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
}
