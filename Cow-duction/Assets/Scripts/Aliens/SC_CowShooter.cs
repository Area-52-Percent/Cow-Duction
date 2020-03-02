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
    public float shotSpeed = 100f;

    private GameObject cowClone;
    private List<GameObject> cowClones;
    private GameObject fullCow;
    private void Awake()
    {
        //uiManager = GameObject.FindWithTag("UIManager").GetComponent<SC_AlienUIManager>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && Time.timeScale > Mathf.Epsilon)
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
            else
            {
                Debug.Log("Missing cows!");
            }
        }
    }
    public void AddCow()
    {
        obtainedCows++;
        Debug.Log("Gimme cow");
    }

    private IEnumerator ShootCow(RaycastHit hit)
    {
        Vector3 grappleHitPoint = hit.point;

        Vector3 reticlePoint = RectTransformUtility.WorldToScreenPoint(null, crosshair.GetComponent<RectTransform>().position);
        Ray ray = Camera.main.ScreenPointToRay(reticlePoint);

        if (dehydratedCow)
        {
            cowClone = Instantiate(dehydratedCow, grappleOrigin.transform.position, ufo.transform.rotation);
            cowClones.Add(cowClone);
            cowClone.gameObject.GetComponent<Rigidbody>().AddForce(ray.direction * shotSpeed, ForceMode.Impulse);
            obtainedCows--;
        }

        yield return new WaitForSeconds(3f);
        Transform cowSpawn = cowClones[0].transform;
        fullCow = Instantiate(cow, cowSpawn);
        fullCow.GetComponent<SC_CowBrain>().setDehydrated();
        Destroy(cowClones[0]);
        cowClones.RemoveAt(0);
    }
}
