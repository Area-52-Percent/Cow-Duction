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
    private Transform targetTransform;
    [SerializeField] private AudioClip shotgunPump = null; // Set up in inspector
    [SerializeField] private AudioClip shotgunShot = null; // Set up in inspector
    [SerializeField] private Transform gunShotOrigin = null; // Set up in inspector
    [SerializeField] private GameObject projectile = null; // Set up in inspector
    [SerializeField] private float lockOnDistance = 20.0f;
    private bool lockedOn = false;
    [SerializeField] private float normalSpeed = 10.0f;
    [SerializeField] private float aimSpeed = 5.0f;
    [SerializeField] private float projectileSpeed = 100.0f;
    [SerializeField] private float projectileLife = 5.0f;
    [SerializeField] private float fireCooldown = 3.0f;
    [SerializeField] private float fireRate = 3.0f;

    private Animator farmerAnimator;

    // Start is called before the first frame update
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Cam = GetComponentInChildren<Camera>();
        rbFpController = GetComponent<RigidbodyFirstPersonController>();
        targetTransform = GameObject.Find("UFO").transform;
        farmerAnimator = GetComponentInChildren<Animator>();
        m_AudioSource = GetComponent<AudioSource>();
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
        if (m_Agent.enabled && aiControlled)
        {
            if (fireCooldown < fireRate)
            {
                fireCooldown += Time.deltaTime;
            }
            if (!lockedOn)
            {
                if (wanderTime < maxWanderTime)
                {
                    wanderTime += Time.deltaTime;
                }
                if (m_Agent.enabled && (m_Agent.remainingDistance < 1f || wanderTime >= maxWanderTime))
                {
                    m_Agent.destination = Random.insideUnitSphere * wanderRadius;
                    wanderTime = 0f;
                }
                if (Vector3.Distance(transform.position, targetTransform.position) <= lockOnDistance)
                {
                    LockOn();
                }
            }
            else 
            {
                m_Agent.destination = new Vector3(targetTransform.position.x, 0, targetTransform.position.z);                                
                m_Cam.transform.LookAt(targetTransform);
                farmerAnimator.transform.forward = new Vector3(m_Cam.transform.forward.x, 0, m_Cam.transform.forward.z);
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
    }

    // Move towards and aim at target
    private void LockOn()
    {
        if (!targetTransform.GetComponentInChildren<MeshRenderer>().enabled)
            return;
        lockedOn = true;
        wandering = false;
        m_Agent.speed = aimSpeed;
        fireCooldown = 0.0f;
        if (farmerAnimator)
            farmerAnimator.SetBool("lockedOn", lockedOn);
        if (m_AudioSource)
            m_AudioSource.PlayOneShot(shotgunPump);
    }

    // Go back to wandering state
    private void Disengage()
    {
        lockedOn = false;
        wandering = true;
        m_Agent.speed = normalSpeed;
        m_Cam.transform.forward = transform.forward;
        farmerAnimator.transform.forward = transform.forward;
        if (farmerAnimator)
            farmerAnimator.SetBool("lockedOn", lockedOn);
    }

    // Shoot a projectile from gunShotOrigin
    private void FireWeapon()
    {
        GameObject projectileClone = Instantiate(projectile, gunShotOrigin);
        projectileClone.transform.parent = null;
        projectileClone.GetComponent<Rigidbody>().AddForce(gunShotOrigin.forward * projectileSpeed, ForceMode.Impulse);

        fireCooldown = 0.0f;

        if (m_AudioSource)
            m_AudioSource.PlayOneShot(shotgunShot);

        StartCoroutine(DestroyClone(projectileClone));
    }

    // Destroy the clone after a set amount of time
    private IEnumerator DestroyClone(GameObject clone)
    {
        yield return new WaitForSeconds(projectileLife);

        if (clone)
            Destroy(clone);
    }
}
