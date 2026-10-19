using System.Collections;
using UnityEngine;

public class StarPopFX : MonoBehaviour
{
    [Header("Target & Timing")]
    [Tooltip("Target Transform to animate. Leave blank to use this object.")]
    public Transform starTransform;
    public float duration = 0.4f;
    public Vector3 finalScale = Vector3.one;

    [Header("Rotation & Scale Pop")]
    [Tooltip("Total degrees to spin during the pop.")]
    public float totalSpinDegrees = -180f;
    [Tooltip("How much larger the star expands during peak overshoot before settling.")]
    public float overshootFactor = 1.35f;

    private Coroutine animRoutine;

    private void Awake()
    {
        if (starTransform == null)
            starTransform = transform;
    }

    private void OnEnable()
    {
        if (animRoutine != null)
            StopCoroutine(animRoutine);

        animRoutine = StartCoroutine(PlayStarPopRoutine());
    }

    private IEnumerator PlayStarPopRoutine()
    {
        // 1. Reset state
        starTransform.localScale = Vector3.zero;
        starTransform.localRotation = Quaternion.identity;

        float elapsed = 0f;

        // 2. Main Pop & Rotate Loop
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // Unscaled so it works if game is paused
            float t = Mathf.Clamp01(elapsed / duration);

            // Scale with Overshoot (Expands past final scale, then settles back)
            float scaleProgress = EvaluateOvershoot(t, overshootFactor);
            starTransform.localScale = finalScale * scaleProgress;

            // Smooth rotation untwisting back to zero
            float currentAngle = Mathf.Lerp(totalSpinDegrees, 0f, Mathf.SmoothStep(0f, 1f, t));
            starTransform.localRotation = Quaternion.Euler(0f, 0f, currentAngle);

            yield return null;
        }

        // 3. Guarantee exact final transform
        starTransform.localScale = finalScale;
        starTransform.localRotation = Quaternion.identity;
    }

    // Easing curve: Rapid expansion to peak overshoot, then smooth ease into 1.0
    private float EvaluateOvershoot(float t, float overshoot)
    {
        if (t < 0.45f)
        {
            float normalizedT = t / 0.45f;
            return Mathf.Lerp(0f, overshoot, Mathf.Sin(normalizedT * Mathf.PI * 0.5f));
        }
        else
        {
            float normalizedT = (t - 0.45f) / 0.55f;
            return Mathf.Lerp(overshoot, 1.0f, Mathf.SmoothStep(0f, 1f, normalizedT));
        }
    }
}