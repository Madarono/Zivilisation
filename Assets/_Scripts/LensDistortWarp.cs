using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LensDistortWarp : MonoBehaviour
{
    public static LensDistortWarp instance {get; private set;}

    [Header("Post Processing")]
    public Volume ppVolume;
    private LensDistortion lensDistortion;

    [Header("Values")]
    public float currentLensDistortion = 0.15f;
    public float minDistort = -0.15f;
    public float maxDistort = 0.25f;
    public float timeDistort = 1f;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (ppVolume != null && ppVolume.profile != null)
        {
            ppVolume.profile.TryGet(out lensDistortion);
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

    IEnumerator Distort(float targetDistort)
    {
        float distort = currentLensDistortion;
        float halfTime = timeDistort / 2f;
        float t = 0;

        while(t < timeDistort)
        {
            t += Time.deltaTime;
            if(t < halfTime)
            {
                lensDistortion.intensity.value = Mathf.Lerp(distort, targetDistort, t / halfTime);
            }
            else
            {
                lensDistortion.intensity.value = Mathf.Lerp(targetDistort, distort, t / timeDistort);
            }
            yield return null;
        }

        lensDistortion.intensity.value = currentLensDistortion;
    }
}