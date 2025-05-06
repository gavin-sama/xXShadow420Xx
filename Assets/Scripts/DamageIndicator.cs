using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class DamageIndicator : MonoBehaviour
{
    [Header("Flash Settings")]
    public Image damageImage;
    public float flashSpeed = 0.5f;
    public Color flashColor = Color.white;

    [Header("Shake Settings")]
    [Tooltip("Maximum distance the camera can shake")]
    public float shakeAmount = 0.5f;
    [Tooltip("How quickly the shake will settle down")]
    public float shakeDecreaseFactor = 1.0f;
    [Tooltip("Duration of the shake effect in seconds")]
    public float shakeDuration = 0.3f;

    private Coroutine _currentFadeRoutine;
    private float _currentShakeDuration;
    private CinemachineCamera _cinemachineCamera;
    private Vector3 _originalCameraOffset;

    private void Awake()
    {
        // Initialize flash effect
        if (damageImage != null)
        {
            damageImage.enabled = false;
            damageImage.color = flashColor;
        }
        else
        {
            Debug.LogError("DamageIndicator: No Image assigned!", this);
        }

        // Find the Cinemachine Camera
        _cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
        if (_cinemachineCamera == null)
        {
            Debug.LogError("DamageIndicator: No CinemachineCamera found in scene!", this);
        }
        else
        {
            // Store the original offset
            _originalCameraOffset = _cinemachineCamera.transform.localPosition;
        }
    }

    private void Update()
    {
        // Handle shake effect in Update
        if (_currentShakeDuration > 0 && _cinemachineCamera != null)
        {
            // Apply shake to the camera's position
            Vector3 shakeOffset = Random.insideUnitSphere * shakeAmount;

            // Only apply shake to main camera, not the virtual camera
            Camera.main.transform.position += shakeOffset;

            // Decrease shake duration
            _currentShakeDuration -= Time.deltaTime * shakeDecreaseFactor;
        }
        else if (_currentShakeDuration <= 0)
        {
            // Reset shake
            _currentShakeDuration = 0f;

            // We don't need to reset the camera position as we're only temporarily
            // offsetting it each frame rather than permanently changing its position
        }
    }

    public void Flash()
    {
        // Flash effect
        if (_currentFadeRoutine != null)
        {
            StopCoroutine(_currentFadeRoutine);
        }

        damageImage.color = flashColor;
        damageImage.enabled = true;
        _currentFadeRoutine = StartCoroutine(FadeRoutine());

        // Shake effect
        _currentShakeDuration = shakeDuration;
    }

    private IEnumerator FadeRoutine()
    {
        float currentAlpha = 1.0f;
        while (currentAlpha > 0f)
        {
            currentAlpha -= Time.deltaTime / flashSpeed;
            damageImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, currentAlpha);
            yield return null;
        }

        damageImage.enabled = false;
        _currentFadeRoutine = null;
    }
}