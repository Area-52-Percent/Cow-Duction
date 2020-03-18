using UnityEngine;
using System.Collections;

public class GunScript : MonoBehaviour
{

    public GameObject bulletHole;
    public GameObject MilkParticle;
    public float shootDistance;

    void Update()
    {

        //Left click
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray bullet = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(bullet, out hit, shootDistance))
            {
                //If the raycast hits wall tag
                if (hit.collider.tag == "Wall")
                {
                    Instantiate(MilkParticle, hit.point- Vector3.forward * 0.05f, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                    Instantiate(bulletHole, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));

                }
            }
        }
    }
}