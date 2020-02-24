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
    private float obtainedCows;
    public GameObject crosshair;
    public GameObject grappleOrigin;
    public Camera mainCam;
    public float shootForce;
    public GameObject cow;
    private float captureLength;
    public float shotSpeed = 3f;

    private GameObject cowClone;
    private void Awake()
    {
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<SC_AlienUIManager>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && Time.timeScale > Mathf.Epsilon)
        {
            if (obtainedCows >= 0)
            {
                Vector3 reticlePoint = RectTransformUtility.WorldToScreenPoint(null, crosshair.GetComponent<RectTransform>().position);
                Ray ray = Camera.main.ScreenPointToRay(reticlePoint);
                RaycastHit hit;

                // Ignore UFO layer and trigger colliders
                int layerMask = ~(1 << gameObject.layer);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
                {
                    captureLength = Vector3.Distance(grappleOrigin.transform.position, hit.transform.position);

                    StartCoroutine(ShootCow(hit));
                }


                /*
                RaycastHit hit;
                
                if (Physics.Raycast(grappleOrigin.transform.position, grappleOrigin.transform.position, out hit))
                {
                    if (hit.collider)
                    {
                        crosshair.transform.position = mainCam.WorldToScreenPoint(hit.point);
                        GameObject cowspawn = Instantiate(cow, grappleOrigin.transform.position, grappleOrigin.transform.rotation);
                        cowspawn.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 8000);
                    }
                }
                */
            }
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


        if (cow)
        {
            //cowClone = Instantiate(cow, grappleOrigin.transform.position, Quaternion.identity);
            cowClone = Instantiate(cow, grappleOrigin.transform);
            Vector3 shotLocation = Vector3.Lerp(grappleOrigin.transform.position, grappleHitPoint, 1f);
            cowClone.gameObject.GetComponent<Rigidbody>().AddForce(shotLocation*10);
        }

        yield return null;
    }
}
