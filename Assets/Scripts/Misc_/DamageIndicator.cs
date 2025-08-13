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
    private Vector3 _originalCameraPosition;

    void Awake()
    {
        if (damageImage == null)
        {
            Debug.LogError("DamageIndicator: damageImage not assigned!", this);
        }
        else
        {
            // Make sure the image is hidden at start
            damageImage.gameObject.SetActive(false);
            damageImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 1f);
        }
    }

    void Update()
    {
        if (_currentShakeDuration > 0)
        {
            if (_cinemachineCamera != null)
            {
                // Apply shake by modifying the virtual camera's local position
                Vector3 shakeOffset = Random.insideUnitSphere * shakeAmount;
                shakeOffset.z = 0f; // Optional: avoid shaking forward/backward
                _cinemachineCamera.transform.localPosition = _originalCameraPosition + shakeOffset;

                _currentShakeDuration -= Time.deltaTime * shakeDecreaseFactor;

                if (_currentShakeDuration <= 0)
                {
                    // Reset position once shake ends
                    _currentShakeDuration = 0f;
                    _cinemachineCamera.transform.localPosition = _originalCameraPosition;
                }
            }
        }
    }
     
    public void Flash()
    {
        if (damageImage == null) return;

        if (!damageImage.gameObject.activeSelf)
            damageImage.gameObject.SetActive(true);

        if (_currentFadeRoutine != null)
            StopCoroutine(_currentFadeRoutine);

        damageImage.color = flashColor;
        damageImage.enabled = true;

        _currentFadeRoutine = StartCoroutine(FadeRoutine());

        _currentShakeDuration = shakeDuration;
    }

    private IEnumerator FadeRoutine()
    {
        float currentAlpha = 1f;
        while (currentAlpha > 0f)
        {
            currentAlpha -= Time.deltaTime / flashSpeed;
            damageImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, Mathf.Clamp01(currentAlpha));
            yield return null;
        }

        damageImage.enabled = false;
        damageImage.gameObject.SetActive(false);
        _currentFadeRoutine = null;
    }
}
