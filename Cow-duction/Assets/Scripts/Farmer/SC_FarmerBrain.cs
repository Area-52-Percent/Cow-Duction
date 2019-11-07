/*  SC_FarmerBrain.cs

    Extends Cow Brain to add lock on and firing states.

    Assumptions:
        There is a GameObject in the scene named "UFO".
        This component goes with SC_CowBrain on the same object.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SC_FarmerBrain : SC_CowBrain
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform gunShotOrigin;
    [SerializeField] private GameObject projectile;    
    [SerializeField] private float lockOnDistance = 20.0f;
    [SerializeField] private bool lockedOn = false;
    [SerializeField] private float normalSpeed = 10.0f;
    [SerializeField] private float aimSpeed = 5.0f;
    [SerializeField] private float projectileSpeed = 100.0f;
    [SerializeField] private float projectileLife = 5.0f;
    [SerializeField] private float fireCooldown = 3.0f;
    [SerializeField] private float fireRate = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        cowAgent = GetComponent<NavMeshAgent>();
        cowCam = GetComponentInChildren<Camera>();
        targetTransform = GameObject.Find("UFO").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (fireCooldown < fireRate)
        {
            fireCooldown += Time.deltaTime;
        }
        if (!lockedOn)
        {
            if (Vector3.Distance(transform.position, targetTransform.position) <= lockOnDistance)
                LockOn();
        }
        else 
        {
            cowAgent.destination = targetTransform.position;
            cowCam.transform.LookAt(targetTransform);
            if (Vector3.Distance(transform.position, targetTransform.position) > lockOnDistance)
                Disengage();
            else if (fireCooldown >= fireRate)
                FireWeapon();
        }
    }

    // Move towards and aim at target
    private void LockOn()
    {
        lockedOn = true;
        wandering = false;
        cowAgent.speed = aimSpeed;
        fireCooldown = 0.0f;     
    }

    // Go back to wandering state
    private void Disengage()
    {
        lockedOn = false;
        wandering = true;
        cowAgent.speed = normalSpeed;
    }

    // Shoot a projectile from gunShotOrigin
    private void FireWeapon()
    {
        GameObject projectileClone = Instantiate(projectile, gunShotOrigin);
        projectileClone.GetComponent<Rigidbody>().AddForce(gunShotOrigin.forward * projectileSpeed, ForceMode.Impulse);

        fireCooldown = 0.0f;

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
