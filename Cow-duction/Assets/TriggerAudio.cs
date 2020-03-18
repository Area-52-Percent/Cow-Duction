using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAudio : MonoBehaviour
{
    public AudioClip SoundToPlay;
    AudioSource sounds;
    void Start()
    {
        sounds = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter()
    {
        if (GameObject.FindWithTag("MainCamera"))
        {
            sounds.PlayOneShot(SoundToPlay);
        }
    }

}
