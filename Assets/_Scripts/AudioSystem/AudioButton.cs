using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AudioButton : MonoBehaviour
{
    [SerializeField] private Button button;
    
    [Header("Audio")]
    public AudioClip overrideAudio;

    private void OnEnable()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        bool overrideAudioClip = overrideAudio != null;

        AudioClip clip = overrideAudioClip ? overrideAudio : AudioManager.instance.buttonClicks[Random.Range(0, AudioManager.instance.buttonClicks.Length)];

        AudioManager.instance.Play(clip);
    }
}