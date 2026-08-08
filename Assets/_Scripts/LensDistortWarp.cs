using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LensDistortWarp : MonoBehaviour
{
    public static LensDistortWarp instance { get; private set; }

    [Header("Post Processing Volume")]
    public Volume ppVolume;
    private LensDistortion lensDistortion;
    private ChromaticAberration chromaticAberration;
    private Vignette vignette;

    [Header("Warp Settings")]
    public float currentLensDistortion = 0.15f;
    public float minDistort = -0.15f;
    public float maxDistort = 0.25f;
    public float timeDistort = 1f;

    [Header("Lose Sequence Settings")]
    public float currentChromaticAberration = 0.15f;
    public float currentVignette = 0.15f;

    public float caValue;
    public float ldValue;
    public float vValue;
    public float timeLose = 2f;

    [Header("Time Slowdown Settings")]
    public float minTimeScale = 0.2f;

    [Header("Animation Curves")]
    [Tooltip("Control the distortion profile over time (0 = start values, 1 = peak lose values)")]
    public AnimationCurve loseWarpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Tooltip("Control game speed scale over time (0 = normal time, 1 = maximum slowdown)")]
    public AnimationCurve timeSlowCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("Point along the curve (0.0 to 1.0) where the UI window pops open")]
    [Range(0f, 1f)] public float uiTriggerPoint = 0.5f;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (ppVolume != null && ppVolume.profile != null)
        {
            ppVolume.profile.TryGet(out lensDistortion);
            ppVolume.profile.TryGet(out chromaticAberration);
            ppVolume.profile.TryGet(out vignette);
        }
    }

    public void Enlarge()
    {
        StartCoroutine(Distort(maxDistort));
    }

    public void Small()
    {
        StartCoroutine(Distort(minDistort));
    }

    public void LoseSequence()
    {
        StartCoroutine(LoseScreen());
    }

    IEnumerator Distort(float targetDistort)
    {
        if (lensDistortion == null) yield break;

        float distort = currentLensDistortion;
        float halfTime = timeDistort / 2f;
        float t = 0;

        while (t < timeDistort)
        {
            t += Time.deltaTime;

            if (t < halfTime)
            {
                lensDistortion.intensity.value = Mathf.Lerp(distort, targetDistort, t / halfTime);
            }
            else
            {
                float progress = (t - halfTime) / halfTime;
                lensDistortion.intensity.value = Mathf.Lerp(targetDistort, distort, progress);
            }

            yield return null;
        }

        lensDistortion.intensity.value = currentLensDistortion;
    }

    IEnumerator LoseScreen()
    {
        float t = 0f;
        bool windowActivated = false;
        float defaultFixedDelta = Time.fixedDeltaTime;

        while (t < timeLose)
        {
            t += Time.unscaledDeltaTime;
            float normalizedTime = Mathf.Clamp01(t / timeLose);

            float curveValue = loseWarpCurve.Evaluate(normalizedTime);
            float slowValue = timeSlowCurve.Evaluate(normalizedTime);

            Time.timeScale = Mathf.Lerp(1.0f, minTimeScale, slowValue);
            Time.fixedDeltaTime = defaultFixedDelta * Time.timeScale;

            lensDistortion.intensity.value = Mathf.Lerp(currentLensDistortion, ldValue, curveValue);
            chromaticAberration.intensity.value = Mathf.Lerp(currentChromaticAberration, caValue, curveValue);
            vignette.intensity.value = Mathf.Lerp(currentVignette, vValue, curveValue);

            if (!windowActivated && normalizedTime >= uiTriggerPoint)
            {
                if (LoseCondition.instance != null && LoseCondition.instance.loseWindow != null)
                {
                    LoseCondition.instance.loseWindow.SetActive(true);
                }
                windowActivated = true;
            }

            yield return null;
        }

        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = defaultFixedDelta;

        lensDistortion.intensity.value = currentLensDistortion;
        chromaticAberration.intensity.value = currentChromaticAberration;
        vignette.intensity.value = currentVignette;
    }
}