using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public AudioClip SoundToPlay;
    AudioSource sounds;
    public bool alreadyPlayed = false;
    void Start()
    {
        sounds = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter()
    {
        if (GameObject.FindWithTag("MainCamera"))
        {
            if (!alreadyPlayed)
            {
                sounds.PlayOneShot(SoundToPlay);
                alreadyPlayed = true;
            }
        }
    }

}
