using UnityEngine;

public class MobilePerformanceInitializer : MonoBehaviour
{
    void Awake()
    {
        QualitySettings.vSyncCount = 0;

        double nativeRefreshRate = Screen.currentResolution.refreshRateRatio.value;

        if (nativeRefreshRate <= 0)
        {
            Application.targetFrameRate = 60;
        }
        else
        {
            Application.targetFrameRate = Mathf.RoundToInt((float)nativeRefreshRate);
        }

        Debug.Log($"[Performance] Mobile Target FPS set to: {Application.targetFrameRate}");
    }
}