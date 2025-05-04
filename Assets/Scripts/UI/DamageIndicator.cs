using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    [Header("Flash Settings")]
    public Image damageImage;
    public float flashSpeed = 0.5f;
    public Color flashColor = Color.white;

    [Header("Shake Settings")]
    [Tooltip("Maximum distance the camera can shake")]
    public float shakeAmount = 0.1f;
    [Tooltip("How quickly the shake settles down")]
    public float shakeDecreaseFactor = 1.0f;
    [Tooltip("Duration of the shake effect in seconds")]
    public float shakeDuration = 0.3f;

    private Coroutine _currentFadeRoutine;
    private Vector3 _originalCameraPos;
    private float _currentShakeDuration;
    private Transform _cameraTransform;

    private void Awake()
    {
        // Initialize flash
        if (damageImage != null)
        {
            damageImage.enabled = false;
            damageImage.color = flashColor;
        }
        else
        {
            Debug.LogError("DamageIndicator: No Image assigned!", this);
        }

        // Initialize shake
        _cameraTransform = Camera.main.transform;
        _originalCameraPos = _cameraTransform.localPosition;
    }

    private void Update()
    {
        // Handle shake effect in Update
        if (_currentShakeDuration > 0)
        {
            _cameraTransform.localPosition = _originalCameraPos + Random.insideUnitSphere * shakeAmount;
            _currentShakeDuration -= Time.deltaTime * shakeDecreaseFactor;
        }
        else
        {
            _currentShakeDuration = 0f;
            _cameraTransform.localPosition = _originalCameraPos;
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