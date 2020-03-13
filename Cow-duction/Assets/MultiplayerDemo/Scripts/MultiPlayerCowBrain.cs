﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

/// <summary>
/// Simple AI that randomly selects a destination on a flat plane to travel to.
/// The destination updates once the agent is within 1 meter of it.
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(NavMeshAgent))]
public class MultiPlayerCowBrain : NetworkBehaviour
{
    protected GameObject[] fields; // Tagged as Field
    protected NavMeshAgent m_Agent;
    protected AudioSource m_AudioSource;
    protected Animator m_Animator;
    protected Vector3 currentDestination; // Keeps track of destination while agent disabled
    protected float wanderTime = 0f;
    protected bool wandering = false;
    private bool followingWaypoints = false;
    private bool seekingFood = true;
    private bool recovering = false;
    private float mooTimer = 0f;
    private float timeBetweenMoos = 0f;
    [SerializeField] private float maxTimeBetweenMoos = 20f;
    [SerializeField] private float minTimeBetweenMoos = 10f;

    [Header("Parameters")]
    public float fieldRadius = 5f;
    public float wanderRadius = 20f;
    public float maxWanderTime = 10f;
    public float maxSpeed = 8f;
    public float idleTime = 3f;
    public float recoveryTime = 3f;
    public float milk = 10f;
    public float mass = 10f;
    public bool tugWhenGrappled = false;

    [Header("SFX")]
    public AudioClip cowMoo = null;

    public void SetMaxSpeed(float speed)
    {
        maxSpeed = speed;
        m_Agent.speed = maxSpeed;
    }

    // Awake is called after all objects are initialized
    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_AudioSource = GetComponent<AudioSource>();
        m_Animator = GetComponentInChildren<Animator>();
        fields = GameObject.FindGameObjectsWithTag("Field");

        timeBetweenMoos = Random.Range(minTimeBetweenMoos, maxTimeBetweenMoos);
        followingWaypoints = GetComponent<WaypointFollower>();

        Wander();
    }

    // Update is called once per frame
    private void Update()
    {
        // Play moo sfx in set intervals
        if (mooTimer >= timeBetweenMoos)
        {
            PlayMoo();
        }
        else
        {
            mooTimer += Time.deltaTime;
        }

        // Set animator speed parameter for walking animation
        if (m_Animator)
            m_Animator.SetFloat("speed", m_Agent.velocity.magnitude);

        if (!NetworkServer.active) return;

        if (followingWaypoints) return;

        // Wander for set amount of time or until reaching destination
        if (m_Agent.enabled && m_Agent.isOnNavMesh)
        {
            if (wanderTime < maxWanderTime && m_Agent.remainingDistance >= m_Agent.stoppingDistance)
            {
                wanderTime += Time.deltaTime;
            }
            else
            {
                if (seekingFood)
                {
                    Wander();
                }
                else
                {
                    SeekFood();
                }
            }
        }
        else if (!recovering)
        {
            Rigidbody cowRb = GetComponent<Rigidbody>();
            if (cowRb && cowRb.useGravity)
            {
                StartCoroutine(Recover());
            }
        }
    }

    // Satisfy hunger on contact with field
    private void OnCollisionStay(Collision collision)
    {
        if (seekingFood && collision.gameObject.tag == "Field")
        {
            SatisfyHunger();
        }
    }

    // Allow agent to be knocked over
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody && (collision.rigidbody.velocity.magnitude * collision.rigidbody.mass > maxSpeed * mass))
        {
            if (m_Agent && m_Agent.enabled)
            {
                m_Agent.enabled = false;
                if (!GetComponent<Rigidbody>())
                {
                    gameObject.AddComponent<Rigidbody>().mass = mass;
                }

                // Become an obstacle that other agents avoid
                if (!GetComponent<NavMeshObstacle>())
                {
                    gameObject.AddComponent<NavMeshObstacle>();
                    GetComponent<NavMeshObstacle>().radius = m_Agent.radius;
                    GetComponent<NavMeshObstacle>().height = m_Agent.height;
                }
                StartCoroutine(Recover());
            }
        }
    }

    // Choose a random destination
    protected void Wander()
    {
        if (!m_Agent.enabled)
            return;
        
        if (seekingFood) seekingFood = false;
        wandering = true;

        NavMeshPath navMeshPath = new NavMeshPath();
        Vector3 targetPosition = transform.position + Random.insideUnitSphere * wanderRadius;

        // Keep looking for a path if it can't reach the destination
        while (navMeshPath.status == NavMeshPathStatus.PathPartial) {
            targetPosition = Random.insideUnitSphere * wanderRadius;
            m_Agent.CalculatePath(targetPosition, navMeshPath);
        }

        m_Agent.destination = targetPosition;
        currentDestination = m_Agent.destination;

        wanderTime = 0f;
    }

    // Wait for a set amount of seconds before moving again
    protected IEnumerator Idle()
    {
        m_Agent.speed = 1.0f;

        yield return new WaitForSeconds(idleTime);

        m_Agent.speed = maxSpeed;
    }

    // Re-enable agent after a set period of time
    protected IEnumerator Recover()
    {
        yield return new WaitForSeconds(recoveryTime);
            
        recovering = true;

        float distanceToGround = Mathf.Infinity;
        Rigidbody rb = GetComponent<Rigidbody>();
        RaycastHit rayHit;

        while ((transform.localEulerAngles.z > 1f && transform.localEulerAngles.z < 359f) || distanceToGround > m_Agent.height)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out rayHit))
            {
                distanceToGround = rayHit.distance;
            }

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

        if (rb)
            Destroy(rb);

        if (GetComponent<NavMeshObstacle>())
            Destroy(GetComponent<NavMeshObstacle>());

        m_Agent.enabled = true;
        if (!m_Agent.hasPath)
            m_Agent.destination = currentDestination;

        recovering = false;
    }

    // Play cow moo sfx at the specified pitch
    public void PlayMoo(float pitch = 1f)
    {
        if (m_AudioSource && cowMoo)
        {
            m_AudioSource.pitch = pitch;
            m_AudioSource.PlayOneShot(cowMoo);
        }   
        mooTimer = 0f;
    }

    // Set destination as the closest field
    private void SeekFood()
    {
        if (!m_Agent.enabled)
            return;
        
        if (wandering) wandering = false;
        seekingFood = true;

        // Prevent getting stuck looking for food if there are no fields
        if (fields.Length < 1)
            return;

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
    }

    // Stop seeking food
    private void SatisfyHunger()
    {
        StartCoroutine(Idle());
        Wander();
    }
}
