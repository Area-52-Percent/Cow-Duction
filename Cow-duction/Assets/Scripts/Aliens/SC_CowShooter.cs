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
    public GameObject dehydratedCow;
    public GameObject cow;
    public GameObject ufo;
    public float shotSpeed = 20f;

    private GameObject cowClone;
    private GameObject fullCow;
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

                int layerMask = ~(1 << gameObject.layer);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
                {
                    StartCoroutine(ShootCow(hit));
                }
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

        if (dehydratedCow)
        {
            cowClone = Instantiate(dehydratedCow, grappleOrigin.transform.position, ufo.transform.rotation);
            Vector3 shotLocation = Vector3.Lerp(grappleOrigin.transform.position, grappleHitPoint, 1f);
            cowClone.gameObject.GetComponent<Rigidbody>().AddForce(shotLocation * shotSpeed);
            //obtainedCows--;
        }

        yield return new WaitForSeconds(3f);
        //Transform cowSpawn = cowClone.transform;
        //fullCow = Instantiate(cow, cowSpawn);
        //Destroy(cowClone);
    }
}
