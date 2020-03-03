/* SC_CowBrain.cs

    Simple AI that randomly selects a destination on a flat plane to travel to.
    The destination updates once the agent is within 1 meter of it.
   
   Assumptions:
     This component is attached to a GameObject with Collider, NavMeshAgent and Rigidbody components.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class SC_CowBrain : MonoBehaviour
{
    // Protected variables
    protected GameObject[] fields; // Tagged as Field    
    protected RigidbodyFirstPersonController rbFpController;
    protected NavMeshAgent m_Agent;
    protected Camera m_Cam;
    protected AudioSource m_AudioSource;
    protected Animator m_Animator;
    protected Vector3 currentDestination; // Keeps track of destination while agent disabled
    protected float wanderTime = 0.0f;
    protected bool wandering = false;
    
    // Serialized protected variables
    [SerializeField] protected float fieldRadius = 5.0f;
    [SerializeField] private bool seekingFood = true;
    [SerializeField] protected Vector3 wanderOrigin = Vector3.zero;
    [SerializeField] protected int wanderRadius = 100;
    [SerializeField] protected float maxWanderTime = 10.0f;
    [SerializeField] protected float idleTime = 3.0f;
    [SerializeField] protected float maxSpeed = 8.0f;
    [SerializeField] protected float recoveryTime = 3.0f;    
    [SerializeField] protected bool tugWhenGrappled = false;
    [SerializeField] protected bool aiControlled = true;

    // Serialized private variables
    [Space]
    [SerializeField] private AudioClip cowMoo = null; // Set up in inspector
    [SerializeField] private AudioClip cowWalking = null; // Set up in inspector
    [SerializeField] private float milk = 10.0f;
    [SerializeField] public string cowType;
    public AudioClip[] distressed;

    // Awake is called after all objects are initialized
    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Cam = GetComponentInChildren<Camera>();
        m_AudioSource = GetComponent<AudioSource>();
        m_Animator = GetComponentInChildren<Animator>();
        rbFpController = GetComponent<RigidbodyFirstPersonController>();
        fields = GameObject.FindGameObjectsWithTag("Field");
        if (aiControlled)
        {
            SetPlayerControlled(false);
            m_Agent.destination = Random.insideUnitSphere * wanderRadius;
            currentDestination = m_Agent.destination;
        }
        else
        {
            SetPlayerControlled(true);
        }
        SeekFood();

    }

    // Update is called once per frame
    private void Update()
    {
        if (m_Agent.enabled && m_Agent.isOnNavMesh && aiControlled)
        {
            if (wanderTime < maxWanderTime && m_Agent.remainingDistance > fieldRadius)
            {
                wanderTime += Time.deltaTime;
            }
            else
            {
                if (seekingFood)
                {
                    Wander();
                    PlayMoo(1f);
                    
                }
                else
                {
                    SeekFood();
                }
            }
        }
        if (m_Animator)
            m_Animator.SetFloat("speed", m_Agent.velocity.magnitude);
       
    }

    // Satisfy hunger on contact with field
    private void OnCollisionStay(Collision collision)
    {
        if (seekingFood && collision.gameObject.tag == "Field")
        {
            StartCoroutine(Idle());
            SatisfyHunger();
        }        
    }

    // Allow agent to be knocked over
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision occured with " + collision.gameObject.name);
        
        if (collision.rigidbody && (collision.rigidbody.velocity.magnitude * collision.rigidbody.mass) > (GetComponent<Rigidbody>().velocity.magnitude * GetComponent<Rigidbody>().mass))
        {
            if (m_Agent && m_Agent.enabled)
            {
                m_Agent.enabled = false;
                if (!GetComponent<NavMeshObstacle>())
                    gameObject.AddComponent<NavMeshObstacle>();
                StartCoroutine(Recover());                                  
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "Probe(Clone)")
        {
            if (m_AudioSource)
            {
                int soundSelect = (int)Random.Range(0, distressed.Length);
              //  Debug.Log("Playing here ProbeClone");
              //  Debug.Log("Playing sound " + soundSelect);
                m_AudioSource.PlayOneShot(distressed[soundSelect]);
            }
        }
    }

    public void PlayMoo(float pitch)
    {
        if (m_AudioSource && cowMoo)
        {
            m_AudioSource.pitch = pitch;
            m_AudioSource.PlayOneShot(cowMoo);
        }
    }

    

    // Re-enable agent after a set period of time
    public IEnumerator Recover()
    {
        if (this != null)
        {
            yield return new WaitForSeconds(recoveryTime);
            
            float distanceToGround = Mathf.Infinity;
            RaycastHit rayHit;
            while ((transform.localEulerAngles.z > 1f && transform.localEulerAngles.z < 359f) || distanceToGround > m_Agent.height)
            {
                if (Physics.Raycast(transform.position, Vector3.down, out rayHit))
                {
                    distanceToGround = rayHit.distance;
                }

                Rigidbody rb = GetComponent<Rigidbody>();
                Quaternion deltaQuat = Quaternion.FromToRotation(transform.up, Vector3.up);

                Vector3 axis;
                float angle;
                deltaQuat.ToAngleAxis(out angle, out axis);

                float dampenFactor = 2f; // this value requires tuning
                rb.AddTorque(-rb.angularVelocity * dampenFactor, ForceMode.Acceleration);

                float adjustFactor = 1f; // this value requires tuning
                rb.AddTorque(axis.normalized * angle * adjustFactor, ForceMode.Acceleration);
                
                yield return null;
            }

            if (GetComponent<NavMeshObstacle>())
                Destroy(GetComponent<NavMeshObstacle>());
            m_Agent.enabled = true;
            if (!m_Agent.hasPath)
                m_Agent.destination = currentDestination;
        }
    }
    
    public float GetMilk()
    {
        return milk;
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }
    
    public float GetMaxWanderTime()
    {
        return maxWanderTime;
    }

    public bool GetTugWhenGrappled()
    {
        return tugWhenGrappled;
    }

    public void SetMilk(float _milk)
    {
        milk = _milk;
    }

    public void SetMaxSpeed(float _maxSpeed)
    {
        maxSpeed = _maxSpeed;
        m_Agent.speed = maxSpeed;
    }

    public void SetMaxWanderTime(float _maxWanderTime)
    {
        maxWanderTime = _maxWanderTime;
    }

    public void SetTugWhenGrappled(bool _tugWhenGrappled)
    {
        tugWhenGrappled = _tugWhenGrappled;
    }

    // Set destination as the closest field
    private void SeekFood()
    {
        if (!m_Agent.enabled)
            return;
        
        seekingFood = true;

        // Find the closest field
        float minDist = Mathf.Infinity;
        Vector3 targetArea = Vector3.zero;
        foreach (GameObject field in fields)
        {
            float dist = Vector3.Distance(field.transform.position, transform.position);
            if (dist < minDist)
            {
                targetArea = field.transform.position;
                minDist = dist;
            }
        }
        m_Agent.destination = targetArea;
        currentDestination = m_Agent.destination;
        m_Agent.stoppingDistance = fieldRadius;
    }

    // Stop seeking food
    private void SatisfyHunger()
    {
        seekingFood = false;
        wandering = true;
        Wander();
    }

    // Choose a random destination
    protected void Wander()
    {
        if (!m_Agent.enabled)
            return;
        
        NavMeshPath navMeshPath = new NavMeshPath();
        Vector3 targetPosition = wanderOrigin + Random.insideUnitSphere * wanderRadius;

        // Keep looking for a path if it can't reach the destination
        while (navMeshPath.status == NavMeshPathStatus.PathPartial) {
            targetPosition = Random.insideUnitSphere * wanderRadius;
            m_Agent.CalculatePath(targetPosition, navMeshPath);
        }

        m_Agent.destination = targetPosition;
        currentDestination = m_Agent.destination;
        m_Agent.stoppingDistance = 0f;
        wanderTime = 0f;
    }

    // Wait for a set amount of seconds before moving again
    protected IEnumerator Idle()
    {
        m_Agent.speed = 1.0f;

        yield return new WaitForSeconds(idleTime);

        m_Agent.speed = maxSpeed;
    }

    // Toggle between AI controlled and Player controlled
    protected void SetPlayerControlled(bool control)
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
