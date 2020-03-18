using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAudioTunnel : MonoBehaviour
{
    public AudioClip SoundToPlay;
    AudioSource sounds;
    void Start()
    {
        sounds = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter()
    {
        sounds.PlayOneShot(SoundToPlay);
    }

}
