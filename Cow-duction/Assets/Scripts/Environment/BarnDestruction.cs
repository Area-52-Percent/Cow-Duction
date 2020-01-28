using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnDestruction : MonoBehaviour
{
    public GameObject myObject;
    public GameObject DestroyedObjectPrefab;
    public Mesh destroyedMesh;
    private Mesh defaultMesh;
    private bool isDestroyed;
    private bool isRepairing;
    public float timeToRepair;
    private float curTime;
    void Start()
    {
        isDestroyed = false;
        isRepairing = false;
        defaultMesh = myObject.GetComponent<MeshFilter>().mesh;
        curTime = 0;
    }
    void Update()
    {
        if(isRepairing)
        {
            if (curTime > timeToRepair)
            {
                Destroy(gameObject.transform.Find("destroyedObject"));
                myObject.SetActive(true);
                curTime = 0;
                isRepairing = false;
                isDestroyed = false;
            }
            else
                curTime += Time.deltaTime;
        }
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
    void OnCollisionEnter(Collision col)
    {
        if (!isDestroyed)
        {
            if (col.gameObject.name == "AlienProjectile")
            {
                GameObject destroyedObject = Instantiate(DestroyedObjectPrefab, transform.position, transform.rotation);
                destroyedObject.name = "destroyedObject";
                destroyedObject.transform.parent = gameObject.transform;
                myObject.SetActive(false);
                isDestroyed = true;
            }
        }
        else
        {
            if (col.gameObject.tag == "Farmer")
            {
                isRepairing = true;
            }
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "Farmer")
        {
            isRepairing = false;
            curTime = 0;
        }
    }
}
