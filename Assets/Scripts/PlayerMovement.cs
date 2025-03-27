using System.Collections;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;



[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour

{
    public Animator animator;

    //BLEND TREE PARAMS 
    /*
    public float velocity = 0.0f;
    public float deceleration = 0.5f;
    public float acceleration = 0.1f;
    int VelocityHash;
    */

    public Camera playerCamera;


    public float walkSpeed = 6f;

    public float runSpeed = 12f;

    public float jumpPower = 7f;

    public float gravity = 10f;

    public float lookSpeed = 2f;

    public float lookXLimit = 45f;

    public float defaultHeight = 2f;

    public float crouchHeight = 1f;

    public float crouchSpeed = 3f;



    private Vector3 moveDirection = Vector3.zero;

    private float rotationX = 0;

    private CharacterController characterController;

    private bool canMove = true;



    void Start()
    {
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;

        //BLEND TREE START
        //VelocityHash = Animator.StringToHash("Velocity");

        // Get the Animator component attached to the player object
        if (animator == null)
        {
            animator = GetComponent<Animator>(); // Automatically gets the Animator component from the current GameObject
        }
    }



    void Update()
    {

        bool isWalking = Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isIdle = !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D);
        bool isJumping = Input.GetKey(KeyCode.Space);
        bool lStrafe = Input.GetKey(KeyCode.A);
        bool walkBack = Input.GetKey(KeyCode.S);

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;

        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // Jumping
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Crouching
        if (Input.GetKey(KeyCode.C) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 6f;
            runSpeed = 12f;
        }

        // Apply movement
        characterController.Move(moveDirection * Time.deltaTime);

        // Looking around
        if (canMove)
        {
            // Get the mouse input for vertical movement (up and down).
            rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;

            // Apply vertical limits (clamping the up and down view).
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            // Apply the clamped vertical rotation to the camera's local rotation.
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

            // Apply horizontal rotation (left and right).
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }



        if (isRunning && (Input.GetKey(KeyCode.W)))
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", false);
            animator.SetBool("isLStrafe", false); // Stop strafing when running
        }
        // Walking
        else if (isWalking)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", true);
            animator.SetBool("isIdle", false);
            animator.SetBool("isLStrafe", false);
        }
        // Left Strafing
        else if (lStrafe)
        {
            animator.SetBool("isLStrafe", true);
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdle", false);
        }
        // Walking Backward
        else if (walkBack)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", false);
            animator.SetBool("isLStrafe", false);
            animator.SetBool("isWalk_Back", true);
        }
        else if (isIdle)
        {
            animator.SetBool("isIdle", true);
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isLStrafe", false);
            animator.SetBool("isWalk_Back", false);
        }
        else if (isJumping)
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isLStrafe", false);
            animator.SetBool("isWalk_Back", false);
            animator.SetBool("isJumping", true);
        }





        //BLEND TREE LOGIC
        /*
        if (isWalking && velocity < 1.0f)
        {
            velocity += Time.deltaTime * acceleration;
        }

        if (!isWalking && velocity > 0.0f)
        {
            velocity -= Time.deltaTime * deceleration;
        }

        if (!isWalking && velocity < 0.0f)
        { 
        velocity = 0.0f;
        }

        animator.SetFloat(VelocityHash, velocity);
        */
    }
}