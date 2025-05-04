using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    public Camera playerCamera;
 
    public float walkSpeed;
    public float runSpeed;
    public float jumpPower;
    public float gravity;
    public float lookSpeed;
    public float lookXLimit;
    public float defaultHeight;
    public float crouchHeight;
    public float crouchSpeed;

    // Jump timing parameters
    public float jumpDelay; // Time in seconds before actual jump occurs after pressing space


    private Vector3 moveDirection = Vector3.zero;
    private float rotationY = 0;
    private CharacterController characterController;
    private bool canMove = true;
    private bool isJumpingAnimation = false;
    private bool isPerformingJump = false;
    private float jumpTimer = 0f;

    private PlayerInput _playerInput;
    private InputAction _move;
    private InputAction _jump;
    private InputAction _crouch;
    private InputAction _sprint;
    private InputAction _attack;
    private InputAction _interact;
    private InputAction _look;

    private bool isRunning;
    private bool isWalkForward;
    private bool isLStrafe;
    private bool isRStrafe;
    private bool isWalkBack;
    private bool isJumpButtonPressed;
    private bool isCrouching;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        _playerInput = GetComponent<PlayerInput>();

        SetInputActions();
    }

    void Update()
    {
        UpdateActions();

        // Handle jump initiation
        if (isJumpButtonPressed && canMove && characterController.isGrounded && !isJumpingAnimation)
        {
            // Start jump animation
            isJumpingAnimation = true;
            jumpTimer = 0f;

            // Set the jumping animation parameter
            SetAnimationState("isJumping");
        }

        // Handle the jump delay
        if (isJumpingAnimation)
        {
            jumpTimer += Time.deltaTime;

            // Perform the actual jump after the delay
            if (jumpTimer >= jumpDelay && !isPerformingJump)
            {
                isPerformingJump = true;
                moveDirection.y = jumpPower;
            }
        }

        // Reset jump state when player lands
        if (isPerformingJump && characterController.isGrounded)
        {
            isJumpingAnimation = false;
            isPerformingJump = false;
        }

        // Movement controls - don't allow running while jumping
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Allow movement but at reduced speed when jumping
        float movementMultiplier = isJumpingAnimation ? 0.5f : 1.0f;
        float curSpeedX = canMove ? (isRunning && !isJumpingAnimation ? runSpeed : walkSpeed) * _move.ReadValue<Vector2>().y * movementMultiplier : 0;
        float curSpeedY = canMove ? (isRunning && !isJumpingAnimation ? runSpeed : walkSpeed) * _move.ReadValue<Vector2>().x * movementMultiplier : 0;

        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        moveDirection.y = movementDirectionY;

        // Apply gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Crouching
        if (isCrouching && canMove && !isJumpingAnimation)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed * 1.5f;
        }
        else if (!isJumpingAnimation)
        {
            characterController.height = defaultHeight;
            walkSpeed = 6f;
            runSpeed = 12f;
        }

        // Apply movement
        characterController.Move(moveDirection * Time.deltaTime);

        // Looking around
        if (canMove && Cursor.lockState == CursorLockMode.Locked)
        {
            rotationY -= _look.ReadValue<Vector2>().y;
            rotationY = Mathf.Clamp(rotationY, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationY, 0, 0).normalized;
            transform.rotation *= Quaternion.Euler(0, _look.ReadValue<Vector2>().x * lookSpeed, 0);
        }

        AnimationHandler();
    }

    private void AnimationHandler()
    {
        // Handle animations - only if not currently jumping
        if (!isJumpingAnimation)
        {
            if (isRunning)
            {
                SetAnimationState("isRunning");
            }
            else if (isWalkForward)
            {
                SetAnimationState("isWalking");
            }
            else if (isLStrafe)
            {
                SetAnimationState("isLStrafe");
            }
            else if (isRStrafe)
            {
                SetAnimationState("isRStrafe");
            }
            else if (isWalkBack)
            {
                SetAnimationState("isWalk_Back");
            }
            else
            {
                SetAnimationState("isIdle");
            }
        }
    }

    // Helper method to set the input keys
    private void SetInputActions()
    {
        var actions = _playerInput.actions;
        _move = actions["Move"];
        _jump = actions["Jump"];
        _crouch = actions["Crouch"];
        _sprint = actions["Sprint"];
        _attack = actions["Attack"];
        _interact = actions["Interact"];
        _look = actions["Look"];
    }

    // Helper method to set animation state exclusively
    private void SetAnimationState(string activeState)
    {
        animator.SetBool("isRunning", activeState == "isRunning");
        animator.SetBool("isWalking", activeState == "isWalking");
        animator.SetBool("isIdle", activeState == "isIdle");
        animator.SetBool("isLStrafe", activeState == "isLStrafe");
        animator.SetBool("isRStrafe", activeState == "isRStrafe");
        animator.SetBool("isWalk_Back", activeState == "isWalk_Back");
        animator.SetBool("isJumping", activeState == "isJumping");
    }

    private void UpdateActions()
    {
        if (Input.anyKey)
        {
            isRunning = _sprint.IsPressed() && _move.IsPressed();
            isWalkForward = _move.ReadValue<Vector2>().y > 0.1f;
            isLStrafe = _move.ReadValue<Vector2>().x < -0.1f;
            isRStrafe = _move.ReadValue<Vector2>().x > 0.1f;
            isWalkBack = _move.ReadValue<Vector2>().y < -0.1f;
            isJumpButtonPressed = _jump.WasPressedThisFrame();
            isCrouching = _crouch.IsPressed();
        }
        else
        {
            isRunning = false;
            isWalkForward = false;
            isLStrafe = false;
            isRStrafe = false;
            isWalkBack = false;
            isJumpButtonPressed = false;
        }
    }
}