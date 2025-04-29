using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    public Camera playerCamera;

    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 25f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    // Jump timing parameters
    public float jumpDelay = 0f; // Time in seconds before actual jump occurs after pressing space

    private Vector3 moveDirection = Vector3.zero;
    private float rotationY = 0;
    private CharacterController characterController;
    private bool canMove = true;
    private bool isJumpingAnimation = false;
    private bool isPerformingJump = false;
    private float jumpTimer = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isWalking = Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D);
        bool isIdle = !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D);
        bool lStrafe = Input.GetKey(KeyCode.A);
        bool walkBack = Input.GetKey(KeyCode.S);
        bool jumpButtonPressed = Input.GetButtonDown("Jump");

        // Handle jump initiation
        if (jumpButtonPressed && canMove && characterController.isGrounded && !isJumpingAnimation)
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
        float curSpeedX = canMove ? (isRunning && !isJumpingAnimation ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") * movementMultiplier : 0;
        float curSpeedY = canMove ? (isRunning && !isJumpingAnimation ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") * movementMultiplier : 0;

        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        moveDirection.y = movementDirectionY;

        // Apply gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Crouching
        if (Input.GetKey(KeyCode.LeftControl) && canMove && !isJumpingAnimation)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
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
            rotationY -= Input.GetAxis("Mouse Y") * lookSpeed;
            rotationY = Mathf.Clamp(rotationY, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationY, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        // Handle animations - only if not currently jumping
        if (!isJumpingAnimation)
        {
            if (isRunning && Input.GetKey(KeyCode.W))
            {
                SetAnimationState("isRunning");
            }
            else if (isWalking)
            {
                SetAnimationState("isWalking");
            }
            else if (lStrafe)
            {
                SetAnimationState("isLStrafe");
            }
            else if (walkBack)
            {
                SetAnimationState("isWalk_Back");
            }
            else if (isIdle)
            {
                SetAnimationState("isIdle");
            }
        }
    }

    // Helper method to set animation state exclusively
    private void SetAnimationState(string activeState)
    {
        animator.SetBool("isRunning", activeState == "isRunning");
        animator.SetBool("isWalking", activeState == "isWalking");
        animator.SetBool("isIdle", activeState == "isIdle");
        animator.SetBool("isLStrafe", activeState == "isLStrafe");
        animator.SetBool("isWalk_Back", activeState == "isWalk_Back");
        animator.SetBool("isJumping", activeState == "isJumping");
    }
}