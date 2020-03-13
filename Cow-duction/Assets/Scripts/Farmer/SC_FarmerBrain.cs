/*  SC_FarmerBrain.cs

    Extends Cow Brain to add lock on and firing states.

    Assumptions:
        There is a GameObject in the scene named "UFO".
 */

using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;

public class SC_FarmerBrain : SC_CowBrain
{
    // Private variables
    private Transform targetTransform;
    private float fireCooldown;
    private int ammoCount;
    private bool lockedOn;
    private bool seekingAmmo;
    public bool peaceful;
    public ParticleSystem GunSmoke;
    

    // Serialized private variables
    [SerializeField] private Transform gunShotOrigin = null; // Set up in inspector
    [SerializeField] private GameObject projectile = null; // Set up in inspector
    [SerializeField] private AudioClip shotgunPump = null; // Set up in inspector
    [SerializeField] private AudioClip shotgunShot = null; // Set up in inspector    
    [Space]
    [SerializeField] private float lockOnDistance = 20.0f;
    [SerializeField] private float lockOnSpeed = 5.0f;
    [SerializeField] private float aimSpeed = 5.0f;
    [SerializeField] private float projectileSpeed = 100.0f;
    [SerializeField] private float projectileLife = 5.0f;
    [SerializeField] private float fireRate = 3.0f;
    [SerializeField] private int startingAmmo = 5;
    // [SerializeField] private float peaceTime = 45.0f;

    // Awake is called after all objects are initialized
    void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Cam = GetComponentInChildren<Camera>();
        rbFpController = GetComponent<RigidbodyFirstPersonController>();
        // targetTransform = GameObject.Find("UFO").transform;
        m_Animator = GetComponentInChildren<Animator>();
        m_AudioSource = GetComponent<AudioSource>();
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
    }

    // Start is called before the first frame update
    void Start()
    {
        targetTransform = GameObject.FindGameObjectWithTag("UFO").transform;
        lockedOn = false;
        seekingAmmo = false;
        fireCooldown = fireRate;
        ammoCount = startingAmmo;
        wandering = true;
        Wander();
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_Animator.GetBool("celebrate"))
        {
            m_Agent.speed = 0f;
            return;
        }

        if (m_Agent.enabled && m_Agent.isOnNavMesh && aiControlled)
        {
            if (fireCooldown < fireRate)
            {
                fireCooldown += Time.deltaTime;
            }
            if (!lockedOn)
            {
                m_Cam.transform.rotation = Quaternion.Slerp(m_Cam.transform.rotation, transform.rotation, lockOnSpeed * Time.deltaTime);
                m_Animator.transform.rotation = Quaternion.Slerp(m_Animator.transform.rotation, transform.rotation, lockOnSpeed * Time.deltaTime);

                if (!seekingAmmo)
                {
                    if (wandering && wanderTime < maxWanderTime)
                    {
                        wanderTime += Time.deltaTime;
                    }
                    if (m_Agent.remainingDistance <= 1f || wanderTime >= maxWanderTime)
                    {
                        Wander();
                    }
                    else if (!peaceful && Vector3.Distance(transform.position, targetTransform.position) <= lockOnDistance)
                    {
                        LockOn();
                    }
                }
            }
            else if (!seekingAmmo)
            {
                m_Agent.destination = new Vector3(targetTransform.position.x, 0, targetTransform.position.z);
                currentDestination = m_Agent.destination;
                
                Quaternion targetRotation = Quaternion.LookRotation(targetTransform.position - m_Cam.transform.position);
                m_Cam.transform.rotation = Quaternion.Slerp(m_Cam.transform.rotation, targetRotation, lockOnSpeed * Time.deltaTime);
                
                Vector3 farmerForward = new Vector3(m_Cam.transform.forward.x, 0, m_Cam.transform.forward.z);
                m_Animator.transform.forward = Vector3.Lerp(farmerForward, m_Animator.transform.forward, lockOnSpeed * Time.deltaTime);

                if ((Vector3.Distance(transform.position, targetTransform.position) > lockOnDistance) ||
                    (!targetTransform.GetComponentInChildren<MeshRenderer>().enabled))
                {
                    Disengage();
                }
                else if (fireCooldown >= fireRate)
                {
                    FireWeapon();
                }
            }
        }

        if (m_Animator)
                m_Animator.SetFloat("speed", m_Agent.velocity.magnitude);
    }

    // Refill ammo on collision with field
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Field")
        {
            if (seekingAmmo)
                RefillAmmo();
        }
    }

    // Move towards and aim at target
    private void LockOn()
    {
        if (seekingAmmo)
            return;

        if (!targetTransform.GetComponentInChildren<MeshRenderer>().enabled)
        {
            Disengage();
            return;
        }
        
        Debug.DrawRay(m_Cam.transform.position, (targetTransform.position - m_Cam.transform.position), Color.yellow);
        if (Physics.Raycast(m_Cam.transform.position, (targetTransform.position - m_Cam.transform.position), out RaycastHit hit))
        {
            if (hit.transform != targetTransform)
            {
                Disengage();
                return;
            }
        }

        lockedOn = true;
        wandering = false;
        m_Agent.speed = aimSpeed;
        fireCooldown = 0.0f;
        if (m_Animator)
            m_Animator.SetBool("lockedOn", lockedOn);
        if (m_AudioSource)
            m_AudioSource.PlayOneShot(shotgunPump);
    }

    // Go back to wandering state
    private void Disengage()
    {
        lockedOn = false;
        wandering = true;
        m_Agent.speed = maxSpeed;

        if (m_Animator)
            m_Animator.SetBool("lockedOn", lockedOn);
    }

    // Shoot a projectile from gunShotOrigin
    private void FireWeapon()
    {
        GameObject projectileClone = Instantiate(projectile, gunShotOrigin);
        GunSmoke.Play();
        projectileClone.transform.parent = null;
        projectileClone.GetComponent<Rigidbody>().AddForce(gunShotOrigin.forward * projectileSpeed, ForceMode.Impulse);

        fireCooldown = 0.0f;
        ammoCount--;

        if (ammoCount < 1)
        {
            Disengage();
            float minDist = Mathf.Infinity;
            Vector3 targetArea = targetTransform.position;
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
            seekingAmmo = true;
        }

        if (m_AudioSource)
            m_AudioSource.PlayOneShot(shotgunShot);

        StartCoroutine(DestroyClone(projectileClone));
    }

    // Add ammo to ammoCount
    private void RefillAmmo()
    {
        ammoCount = startingAmmo;
        seekingAmmo = false;
        Wander();
    }

    // Destroy the clone after a set amount of time
    private IEnumerator DestroyClone(GameObject clone)
    {
        yield return new WaitForSeconds(projectileLife);

        if (clone)
            Destroy(clone);
    }
}
