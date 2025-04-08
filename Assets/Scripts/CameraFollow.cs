using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // Player Transform
    public Vector3 offset = new Vector3(0, 1.5f, -2f);  // Camera offset
    public float smoothSpeed = 10f;
    public float sensitivity = 2f;  // Mouse sensitivity
    public float maxVerticalAngle = 80f;  // Limit vertical rotation
    private float rotationX = 0f;

    void LateUpdate()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // Rotate the camera around the player
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -maxVerticalAngle, maxVerticalAngle);

        transform.RotateAround(target.position, Vector3.up, mouseX);
        transform.localRotation = Quaternion.Euler(rotationX, transform.eulerAngles.y, 0);

        // Position the camera based on the offset
        Vector3 desiredPosition = target.position + transform.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Always look at the player
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
