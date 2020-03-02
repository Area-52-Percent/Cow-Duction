using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource footAudioSource;
    public AudioClip[] footsteps;
    public float stepInterval = 0.25f;
    private float nextStep = 0f;



    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Ground" && Time.time > nextStep)
        {
            nextStep = Time.time + stepInterval;
            footAudioSource.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
            return;
        }
        
    }
}
