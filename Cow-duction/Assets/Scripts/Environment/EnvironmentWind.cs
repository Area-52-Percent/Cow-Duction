/*  EnvironmentWind.cs

    Pushes rigidbodies within the trigger zone in the forward direction of the transform.
*/

using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class EnvironmentWind : MonoBehaviour
{
    [SerializeField] private float pushForce = 10.0f;
    [SerializeField] private float maxWindVolume = 0.5f;
    [SerializeField] private bool pushForward;
    private AudioSource m_AudioSource;
    private bool triggered = false;

    // Start is called before the first frame update
    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.volume = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!triggered)
        {
            if (m_AudioSource.volume > 0f)
            {
                m_AudioSource.volume -= Time.deltaTime;
            }
            else
            {
                m_AudioSource.Stop();
            }
        }
    }

    // Pushes rigidbody in forward direction while inside trigger zone
    void OnTriggerStay(Collider other)
    {
        Rigidbody otherRigidbody = other.gameObject.GetComponent<Rigidbody>();
        if (otherRigidbody)
        {
            if (!triggered && other.name == "UFO")
                triggered = true;
            else
            {
                if (!m_AudioSource.isPlaying)
                {
                    m_AudioSource.Play();
                }
                else if (m_AudioSource.volume < maxWindVolume)
                {
                    m_AudioSource.volume += Time.deltaTime * 0.5f;
                }
            }
            otherRigidbody.AddForce(pushForce * (pushForward ? transform.forward : (other.transform.position - transform.position).normalized), ForceMode.Acceleration);
        }
    }

    // Allow update to lower audio source volume
    void OnTriggerExit(Collider other)
    {
        if (m_AudioSource.isPlaying)
        {
            triggered = false;
        }
    }
}
