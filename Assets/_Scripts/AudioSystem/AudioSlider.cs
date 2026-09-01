using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    public Slider slider;
    public float amplification = 1f;
    public float stepPercent = 0.01f;
    public bool isMusic = false;
    
    private float stepInterval = 1;
    private float lastPlayedValue;

    private void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();
            
        CalculateStepInterval();
    }

    private void Start()
    {
        if (slider != null) lastPlayedValue = slider.value;
    }

    private void OnEnable()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    public void CalculateStepInterval()
    {
        if (slider == null) return;

        float range = slider.maxValue - slider.minValue;
        stepInterval = Mathf.Max(1f, range * stepPercent); 
    }

    private void OnSliderValueChanged(float rawValue)
    {
        int currentValue = Mathf.RoundToInt(rawValue);

        if (Mathf.Abs(currentValue - lastPlayedValue) >= stepInterval)
        {
            AudioManager.instance.Play(AudioManager.instance.slider, amplification, isMusic);
            lastPlayedValue = currentValue;
        }
    }

    
}