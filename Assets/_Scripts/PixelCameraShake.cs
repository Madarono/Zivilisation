using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PixelCameraShake : MonoBehaviour
{
    public static PixelCameraShake instance { get; private set; }

    [Header("Settings")]
    public float ppu = 8f;

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    void Awake()
    {
        instance = this;
    }

    public void Shake(float duration, float magnitude)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        originalPosition = transform.position;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            Vector3 randomOffset = Random.insideUnitCircle * magnitude;
            randomOffset.z = 0;

            randomOffset.x = Mathf.Round(randomOffset.x * ppu) / ppu;
            randomOffset.y = Mathf.Round(randomOffset.y * ppu) / ppu;

            transform.position = originalPosition + randomOffset;

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.position = originalPosition;
        shakeCoroutine = null;
    }
}