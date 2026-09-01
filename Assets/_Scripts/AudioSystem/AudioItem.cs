using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class AudioItem : MonoBehaviour
{
    public AudioClip clip;
    public float volume;

    public AudioSource source;

    public void Play()
    {
        StartCoroutine(PlayAudio());
    }

    IEnumerator PlayAudio()
    {
        source.clip = clip;
        source.volume = volume;
        source.Play();

        yield return new WaitForSecondsRealtime(clip.length);

        AudioFinished();
    }

    void AudioFinished()
    {
        Destroy(gameObject);
    }
}