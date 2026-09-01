using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance {get; private set;}
    public float master;
    public float music;
    public AudioSource source;

    [Header("Play GameObject")]
    public GameObject playPrefab;

    [Header("Vilagers")]
    public AudioClip villagerAssign;
    public AudioClip villagerRevoke;
    public AudioClip villagerCome;
    public AudioClip villagerLeave;
    public AudioClip villagerDie;
    public AudioClip villagerSelect;
    public AudioClip villagerDeselect;

    [Header("UI")]
    public AudioClip[] buttonClicks;
    public AudioClip slider;
    public AudioClip moneyIncrease;
    public AudioClip moneyDecrease;
    public AudioClip popupText;

    [Header("Laboratory")]
    public AudioClip cureFailure;
    public AudioClip cureSuccess;
    public AudioClip vaccineBuy;
    public AudioClip stats;

    [Header("Building")]
    public AudioClip roadPut;
    public AudioClip roadShovel;
    public AudioClip shovel;
    public AudioClip pickup;
    public AudioClip place;
    public AudioClip building;
    public AudioClip select;

    [Header("Building - Market")]
    public AudioClip sellMarket;

    [Header("Camera")]
    public AudioClip zoomIn;
    public AudioClip zoomOut;

    void Awake()
    {
        instance = this;
    }

    public void Play(AudioClip clip, float amplification = 1f, bool music = false)
    {
        if((music && Settings.instance.muteMusic) || (!music && Settings.instance.muteSfx)) return;

        float volume = music ? this.music : master;
        volume *= amplification;
        source.PlayOneShot(clip, volume);
    }

    public GameObject PlayGameObject(AudioClip clip, float amplification = 1f, bool music = false)
    {
        if((music && Settings.instance.muteMusic) || (!music && Settings.instance.muteSfx)) return null;

        float volume = music ? this.music : master;
        volume *= amplification;

        GameObject go = Instantiate(playPrefab, Vector3.zero, Quaternion.identity);
        if(go.TryGetComponent(out AudioItem goScript))
        {
            goScript.clip = clip;
            goScript.volume = volume;
            goScript.Play();
        }

        return go;
    }
    
    public void UpdateVolume()
    {
        music = Settings.instance.sfxValue;
        master = Settings.instance.musicValue;
    }
}