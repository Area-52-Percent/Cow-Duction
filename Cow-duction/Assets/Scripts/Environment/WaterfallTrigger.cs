using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterfallTrigger : MonoBehaviour
{
    public AudioClip SoundToPlay;
    AudioSource sounds;
    void Start()
    {
        sounds = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "UFO")
        {
            sounds.clip = SoundToPlay;
            sounds.loop = true;
            sounds.Play();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        sounds.Stop();
    }

}
