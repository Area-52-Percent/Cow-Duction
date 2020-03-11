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

    private void OnTriggerEnter()
    {
        if (GameObject.FindWithTag("MainCamera"))
        {
            sounds.PlayOneShot(SoundToPlay);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        sounds.Stop();
    }

}
