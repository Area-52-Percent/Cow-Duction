using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DestructionHandler : NetworkBehaviour
{
    public GameObject myObject;
    public GameObject DestroyedObjectPrefab;
    public GameObject MilkParticle;
    public GameObject bulletHole;
    public GameObject liveParticles;

    //public Mesh destroyedMesh;
    //private Mesh defaultMesh;
    private bool isHit;
    public bool isMilked;
    public float timeToRepair;
    private IEnumerator co;
    private GameObject destroyedObject;

    void Start()
    {
        isHit = false;
        //defaultMesh = myObject.GetComponent<MeshFilter>().mesh;
    }
    //**Mesh Swapping Method**
    //Usable for a static Destroyed Object when physics is NOT wanted on each piece
    //void OnCollisionEnter(Collision col)
    //{
    //    if (!isDestroyed)
    //    {
    //        if (col.gameObject.name == "AlienProjectile")
    //        {
    //            MeshFilter myMesh = myObject.GetComponent<MeshFilter>();
    //            myMesh.mesh = destroyedMesh;
    //        }
    //    }
    //    else
    //    {
    //        if(col.gameObject.tag == "Farmer")
    //        {
    //            MeshFilter myMesh = myObject.GetComponent<MeshFilter>();
    //            myMesh.mesh = defaultMesh;
    //        }
    //    }
    //}
    //**New Object Method**
    //Use when you want to enact physics on the destroyed Object

    public bool HitMilk(RaycastHit hit)
    {
        if (isHit && isMilked)
        {
            return false;
        }
        else if (isHit)
        {
            isMilked = true;
            return true;
        }
        else
        {
            liveParticles = Instantiate(MilkParticle, hit.point - Vector3.forward * 0.05f, Quaternion.FromToRotation(Vector3.forward, hit.normal));
            Instantiate(bulletHole, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
            //destroyedObject = Instantiate(DestroyedObjectPrefab, transform.position, transform.rotation);
            //destroyedObject.transform.parent = gameObject.transform;
            //myObject.SetActive(false);
            isHit = true;
            return false;
        }
    }

    [ClientRpc]
    public void RpcbreakObject()
    {
        destroyedObject = Instantiate(DestroyedObjectPrefab, transform.position, transform.rotation);
        //destroyedObject.transform.parent = gameObject.transform;
        myObject.SetActive(false);
        isHit = true;
    }

    void OnCollisionEnter(Collision col)
    {
        if (!isHit)
        {
            if (col.gameObject.tag == "CowProjectile")
            {
                //destroyedObject = Instantiate(DestroyedObjectPrefab, transform.position, transform.rotation);
                //destroyedObject.transform.parent = gameObject.transform;

                //Instantiate(MilkParticle, col.gameObject.transform.position, Quaternion.FromToRotation(col.gameObject.transform.forward, Vector3.forward));
                //Instantiate(bulletHole, col.gameObject.transform.position, Quaternion.FromToRotation(col.gameObject.transform.forward, Vector3.forward));

                //myObject.SetActive(false);
                isHit = true;
            }
        }
        else
        {
            //if (col.gameObject.tag == "Farmer")
            //{
            //    co = Repair();
            //    StartCoroutine(co);
            //}
        }
    }
    void OnCollisionExit(Collision collision)
    {
        //if(collision.gameObject.tag == "Farmer")
        //{
        //    StopCoroutine(co);
        //}
    }
    protected IEnumerator Repair()
    {
        yield return new WaitForSeconds(timeToRepair);
        Destroy(destroyedObject);
        //Add some Effect
        myObject.SetActive(true);
    }
}
