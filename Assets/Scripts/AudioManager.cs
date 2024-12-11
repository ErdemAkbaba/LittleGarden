using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager audioManager;
    public AudioSource audioSource;
    public List<AudioClip> soundFX = new List<AudioClip>();
    private void Awake()
    {
        audioManager = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void TriggerActiveClip()
    {
        audioSource.Play();
    }

    public void ChangeAudioClip(int index)
    {
        audioSource.clip = soundFX[index];
    }
}
