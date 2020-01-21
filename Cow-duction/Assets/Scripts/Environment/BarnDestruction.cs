using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnDestruction : MonoBehaviour
{
    public GameObject DestroyedPrefab;

    //void OnCollisionEnter(Collision col)
    void Start()
    {
        //if (col.gameObject.name == "AlienProjectile")
        //{
        Debug.Log("L");
            Instantiate(DestroyedPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        //}
    }
}
