using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class Hub : MonoBehaviour
{
	//Modified from PlayerMovement
	[Header("Movement Settings")]
	public float walkSpeed = 6f;
	public float runSpeed = 12f;
	public float gravity = 25f;

	private Vector3 moveDirection = Vector3.zero;
	private CharacterController characterController;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		characterController = GetComponent<CharacterController>();
	}

	// Update is called once per frame
	void Update()
    {
		HandleMovement();
    }
	void HandleMovement()
	{
		// Get player input
		float horizontalInput = Input.GetAxis("Horizontal");
		float verticalInput = Input.GetAxis("Vertical");
		bool isRunning = Input.GetKey(KeyCode.LeftShift);

		if (characterController.isGrounded)
		{
			// Calculate movement direction based on player's orientation
			Vector3 forward = transform.forward;
			Vector3 right = transform.right;

			moveDirection = (forward * verticalInput + right * horizontalInput).normalized;

			// Set speed based on if we're running or not
			float currentSpeed = isRunning ? runSpeed : walkSpeed;
			moveDirection *= currentSpeed;
		}


		// Apply gravity
		if (!characterController.isGrounded)
		{
			moveDirection.y -= gravity * Time.deltaTime;
		}
		else
		{
			moveDirection.y = -0.5f; // Small downward force to keep grounded
		}

		// Move the player
		characterController.Move(moveDirection * Time.deltaTime);
	}
}
