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
    public float timeToRepair;
    private IEnumerator co;
    private GameObject destroyedObject;
    void Start()
    {
        isDestroyed = false;
        defaultMesh = myObject.GetComponent<MeshFilter>().mesh;
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
                destroyedObject = Instantiate(DestroyedObjectPrefab, transform.position, transform.rotation);
                destroyedObject.transform.parent = gameObject.transform;
                myObject.SetActive(false);
                isDestroyed = true;
            }
        }
        else
        {
            if (col.gameObject.tag == "Farmer")
            {
                co = Repair();
                StartCoroutine(co);
            }
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "Farmer")
        {
            StopCoroutine(co);
        }
    }
    protected IEnumerator Repair()
    {
        yield return new WaitForSeconds(timeToRepair);
        Destroy(destroyedObject);
        //Add some Effect
        myObject.SetActive(true);
    }
}
