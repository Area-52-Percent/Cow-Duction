using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

[RequireComponent(typeof(SC_SpaceshipMovement))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MultiPlayerCowShooter : NetworkBehaviour
{
    private SC_AlienUIManager uiManager;
    private SC_SpaceshipMovement spaceshipMovement;
    private GameObject cowspawner;
    private float obtainedCows;
    public GameObject crosshair;
    public GameObject grappleOrigin;
    public Camera mainCam;
    public GameObject dehydratedCow;
    public GameObject cow;
    public GameObject ufo;
    public float shotSpeed = 100f;
    public float randomFactor = 0.1f;

    private GameObject cowClone;
    private GameObject fullCow;
    public float shootForce;
    public List<GameObject> cows = new List<GameObject>();

    private void Awake()
    {
        //uiManager = GameObject.FindWithTag("UIManager").GetComponent<SC_AlienUIManager>();
        //spaceshipMovement = GetComponent<SC_SpaceshipMovement>();
        //uiManager = GameObject.FindWithTag("UIManager").GetComponent<SC_AlienUIManager>();
        cowspawner = GameObject.Find("CowSpawner");
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && Time.timeScale > Mathf.Epsilon)
            if (Input.GetMouseButtonDown(1))
            {
                if (obtainedCows > 0)
                {
                    Vector3 reticlePoint = RectTransformUtility.WorldToScreenPoint(null, crosshair.GetComponent<RectTransform>().position);
                    Ray ray = Camera.main.ScreenPointToRay(reticlePoint);
                    RaycastHit hit;

                    int layerMask = ~(1 << gameObject.layer);
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
                    {
                        StartCoroutine(ShootCow(hit));
                    }
                }
                Debug.Log(obtainedCows);
            }
    }
    public void AddCow()
    {
        obtainedCows++;
    }

    private IEnumerator ShootCow(RaycastHit hit)
    {
        Vector3 grappleHitPoint = hit.point;

        Vector3 reticlePoint = RectTransformUtility.WorldToScreenPoint(null, crosshair.GetComponent<RectTransform>().position);
        Ray ray = Camera.main.ScreenPointToRay(reticlePoint);

        if (dehydratedCow)
        {
            cowClone = Instantiate(dehydratedCow, grappleOrigin.transform.position, ufo.transform.rotation);
            cows.Add(cowClone);

            //Vector3 shotLocation = Vector3.Lerp(reticlePoint, grappleHitPoint, 5f);
            cowClone.gameObject.GetComponent<Rigidbody>().AddForce(ray.direction * shotSpeed, ForceMode.Impulse);
            obtainedCows--;
        }

        yield return new WaitForSeconds(5f);
        Transform cowSpawn = cows[0].transform;
        GameObject curCow = cows[0];
        cows.Remove(curCow);
        Destroy(curCow);
        
        fullCow = Instantiate(cow, cowSpawn.position, cowSpawn.rotation, cowspawner.transform);
        NetworkServer.Spawn(fullCow);
        RandomizeWeakCow(fullCow);
    }

    public void RandomizeWeakCow(GameObject cow)
    {
        SC_CowBrain cowBrain = cow.GetComponent<SC_CowBrain>();
        Rigidbody cowRigidbody = cow.GetComponent<Rigidbody>();

        float mass = cowRigidbody.mass;
        float size = cow.transform.localScale.x; // Assume scale is uniform
        float milk = cowBrain.GetMilk();
        float maxSpeed = cowBrain.GetMaxSpeed();
        float maxWanderTime = cowBrain.GetMaxWanderTime();

        size = (Random.Range(size - (size * randomFactor), size + (size * randomFactor)) / 2);
        mass = Random.Range(mass - (mass * randomFactor), mass + (mass * randomFactor)) + size;
        milk = (Random.Range(milk - (milk * randomFactor), milk + (milk * randomFactor)) + size) / 2;
        maxSpeed = Random.Range(maxSpeed - (maxSpeed * randomFactor), maxSpeed + (maxSpeed * randomFactor)) + size;
        maxWanderTime = Random.Range(maxWanderTime - (maxWanderTime * randomFactor), maxWanderTime + (maxWanderTime * randomFactor)) - size;

        cowRigidbody.mass = mass;
        cow.transform.localScale *= size;
        cowBrain.SetMilk(milk);
        cowBrain.SetMaxSpeed(maxSpeed);
        cowBrain.SetMaxWanderTime(maxWanderTime);
    }
}
