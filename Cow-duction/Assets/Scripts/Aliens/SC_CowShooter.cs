using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SC_SpaceshipMovement))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class SC_CowShooter : MonoBehaviour
{
    private SC_AlienUIManager uiManager;
    private SC_SpaceshipMovement spaceshipMovement;
    private float obtainedCows;

<<<<<<< HEAD
    public float shootForce;
    public GameObject cow;
=======
    private GameObject cowClone;
    private GameObject fullCow;
>>>>>>> parent of 137dbb4... Current Update with teleporting and partial cow-shooter mechanics
    private void Awake()
    {
        spaceshipMovement = GetComponent<SC_SpaceshipMovement>();
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<SC_AlienUIManager>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
<<<<<<< HEAD
            Debug.Log(obtainedCows);
=======
            if (obtainedCows >= 0)
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
>>>>>>> parent of 137dbb4... Current Update with teleporting and partial cow-shooter mechanics
        }
    }
    public void AddCow()
    {
        obtainedCows++;
<<<<<<< HEAD
=======
    }

    private IEnumerator ShootCow(RaycastHit hit)
    {
        Vector3 grappleHitPoint = hit.point;

        Vector3 reticlePoint = RectTransformUtility.WorldToScreenPoint(null, crosshair.GetComponent<RectTransform>().position);
        Ray ray = Camera.main.ScreenPointToRay(reticlePoint);

        if (dehydratedCow)
        {
            cowClone = Instantiate(dehydratedCow, grappleOrigin.transform.position, ufo.transform.rotation);
            
            //Vector3 shotLocation = Vector3.Lerp(reticlePoint, grappleHitPoint, 5f);
            cowClone.gameObject.GetComponent<Rigidbody>().AddForce(ray.direction * shotSpeed, ForceMode.Impulse);
            //obtainedCows--;
        }

        yield return new WaitForSeconds(3f);
        //Transform cowSpawn = cowClone.transform;
        //fullCow = Instantiate(cow, cowSpawn);
        //Destroy(cowClone);
>>>>>>> parent of 137dbb4... Current Update with teleporting and partial cow-shooter mechanics
    }
}
