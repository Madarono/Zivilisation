using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public enum Graphics
{
    Low = 0,
    Medium = 1,
    High = 2
}

[System.Serializable]
public class ButtonVisual
{
    public Color textColor;
    public string visual;
    public Sprite sprite;
}

public class Settings : MonoBehaviour
{
    public static Settings instance {get; private set;}
    public GameObject window;
    private TimeForward timeForward;
    public bool isOpen;
    private CameraPinch camera;

    [Header("Settings")]
    public float sfxValue;
    public bool muteSfx;
    public float musicValue;
    public bool muteMusic;
    public float audioDivider = 100f;
    public Graphics graphics;
    public int graphicsIndex;
    public bool canScreenShake;
    public int fps;
    public int fpsIndex;

    [Header("Settings Result")]
    public int[] wanderCountGraphics;

    [Header("Settings Visual")]
    public Slider sfxSlider;
    public Slider musicSlider;
    public Image graphicsButton;
    public Image fpsButton;
    public Image shakeButton;
    public Image muteSfxButton;
    public Image muteMusicButton;
    public TextMeshProUGUI graphicsText;
    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI shakeText;
    public ButtonVisual[] graphicsVisual;
    public ButtonVisual[] fpsVisual;
    public ButtonVisual[] shakeVisual;
    public Sprite[] muteButton;

    [Header("Post Processing")]
    public Volume ppVolume;
    public ScrollingScanlines scanlines;
    private Bloom bloom;
    private Vignette vignette;
    private LensDistortion lensDistortion;
    private ColorAdjustments colorAdjustments;
    private ChromaticAberration chromaticAberration;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        isOpen = false;
        window.SetActive(false);
        timeForward = TimeForward.instance;
        camera = CameraPinch.Instance;

        if (ppVolume != null && ppVolume.profile != null)
        {
            ppVolume.profile.TryGet(out bloom);
            ppVolume.profile.TryGet(out vignette);
            ppVolume.profile.TryGet(out lensDistortion);
            ppVolume.profile.TryGet(out colorAdjustments);
            ppVolume.profile.TryGet(out chromaticAberration);
        }
    }

    public void BothWindow()
    {
        isOpen = !isOpen;

        if(isOpen)
        {
            OpenWindow();
        }
        else
        {
            CloseWindow();
        }
    }

    public void OpenWindow()
    {
        window.SetActive(true);
        Time.timeScale = 0;
        camera.enabled = false;
        isOpen = true;
        UpdateVisual();
    }

    public void CloseWindow()
    {
        window.SetActive(false);
        timeForward.UpdateTimeScale();
        camera.enabled = true;
        isOpen = false;
    }

    public void ChangeFPS()
    {
        fpsIndex++;
        if(fpsIndex >= fpsVisual.Length)
        {
            fpsIndex = 0;
        }
        fps = int.Parse(fpsVisual[fpsIndex].visual);

        UpdateVisual();
    }

    public void ChangeShake()
    {
        canScreenShake = !canScreenShake;
        UpdateVisual();
    }

    public void ChangeGraphics()
    {
        graphicsIndex++;
        if(graphicsIndex >= graphicsVisual.Length)
        {
            graphicsIndex = 0;
        }
        graphics = (Graphics)graphicsIndex;
        ApplyChanges();
        UpdateVisual();
    }

    public void ChangeAudioSlider()
    {
        musicValue = musicSlider.value / audioDivider;
        sfxValue = sfxSlider.value / audioDivider;
    }

    public void MuteSFX()
    {
        muteSfx = !muteSfx;
        UpdateVisual();
    }

    public void MuteMusic()
    {
        muteMusic = !muteMusic;
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        graphicsButton.sprite = graphicsVisual[graphicsIndex].sprite;
        graphicsText.text = graphicsVisual[graphicsIndex].visual;
        graphicsText.color = graphicsVisual[graphicsIndex].textColor;

        fpsButton.sprite = fpsVisual[fpsIndex].sprite;
        fpsText.text = fpsVisual[fpsIndex].visual;
        fpsText.color = fpsVisual[fpsIndex].textColor;
        SetFPS();

        shakeButton.sprite = shakeVisual[canScreenShake ? 1 : 0].sprite;
        shakeText.text = shakeVisual[canScreenShake ? 1 : 0].visual;
        shakeText.color = shakeVisual[canScreenShake ? 1 : 0].textColor;

        musicSlider.value = musicValue * audioDivider;
        sfxSlider.value = sfxValue * audioDivider;

        muteSfxButton.sprite = muteButton[muteSfx ? 1 : 0];
        muteMusicButton.sprite = muteButton[muteMusic ? 1 : 0];
    }

    public void UpdateValues()
    {
        graphics = (Graphics)graphicsIndex;
        fps = int.Parse(fpsVisual[fpsIndex].visual);
    }

    public void ApplyChanges()
    {
        TownManager.instance.maxSimultaneousWanderers = wanderCountGraphics[graphicsIndex];
        ApplyGraphicsTier(graphics);
    }

    public void SetFPS()
    {
        Application.targetFrameRate = fps;
    }

    public void ApplyGraphicsTier(Graphics tier)
    {
        switch (tier)
        {
            case Graphics.Low:
                ppVolume.gameObject.SetActive(false);
                scanlines.enabled = false;
                break;

            case Graphics.Medium:
                ppVolume.gameObject.SetActive(true);
                scanlines.enabled = true;
                
                if (bloom != null) bloom.active = true;
                if (vignette != null) vignette.active = true;
                if (colorAdjustments != null) colorAdjustments.active = true;
                
                // Turn Off
                if (lensDistortion != null) lensDistortion.active = false;
                if (chromaticAberration != null) chromaticAberration.active = false;
                break;

            case Graphics.High:
                ppVolume.gameObject.SetActive(true);
                scanlines.enabled = true;

                if (bloom != null) bloom.active = true;
                if (vignette != null) vignette.active = true;
                if (colorAdjustments != null) colorAdjustments.active = true;
                if (lensDistortion != null) lensDistortion.active = true;
                if (chromaticAberration != null) chromaticAberration.active = true;
                break;
        }
    }
}