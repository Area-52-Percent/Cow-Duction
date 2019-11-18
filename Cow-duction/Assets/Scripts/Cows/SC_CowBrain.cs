/* SC_CowBrain.cs

    Simple AI that randomly selects a destination on a flat plane to travel to.
    The destination updates once the agent is within 1 meter of it.
   
   Assumptions:
     This component is attached to a GameObject with Collider, NavMeshAgent and Rigidbody components.
 */

using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(NavMeshAgent))]
public class SC_CowBrain : MonoBehaviour
{
    protected NavMeshAgent m_Agent;
    protected Camera m_Cam;
    protected AudioSource m_AudioSource = null;
    [SerializeField] private AudioClip cowMoo = null; // Set up in inspector
    protected RigidbodyFirstPersonController rbFpController;
    [SerializeField] protected int wanderRadius = 100;
    protected float wanderTime = 0.0f;
    [SerializeField] protected float maxWanderTime = 10.0f;
    protected bool wandering = true;
    [SerializeField] protected bool aiControlled = true;

    // Start is called before the first frame update
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Cam = GetComponentInChildren<Camera>();
        m_AudioSource = GetComponent<AudioSource>();
        rbFpController = GetComponent<RigidbodyFirstPersonController>();
        if (aiControlled)
        {
            SetPlayerControlled(false);
            m_Agent.destination = Random.insideUnitSphere * wanderRadius;
        }
        else
        {
            SetPlayerControlled(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (aiControlled && wandering)
        {
            if (wanderTime < maxWanderTime)
            {
                wanderTime += Time.deltaTime;
            }
            if (m_Agent.enabled && (m_Agent.remainingDistance < 1f || wanderTime >= maxWanderTime))
            {
                m_Agent.destination = Random.insideUnitSphere * wanderRadius;
                wanderTime = 0f;
                if (m_AudioSource && cowMoo)
                    m_AudioSource.PlayOneShot(cowMoo);
            }
        }
    }

    // Toggle between AI controlled and Player controlled + camera enabled
    public void SetPlayerControlled(bool control)
    {
        if (control)
        {
            aiControlled = false;
            if (m_Cam)
                m_Cam.enabled = true;
            if (rbFpController)
                rbFpController.enabled = true;
        }
        else
        {
            aiControlled = true;
            if (m_Cam)
                m_Cam.enabled = false;
            if (rbFpController)
                rbFpController.enabled = false;
        }
    }
}
