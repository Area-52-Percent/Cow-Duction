using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cornrotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in transform)
        {
            child.RotateAround(Vector3.up, Random.Range(-180.0f, 180.0f));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            KnockDown();
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Collided with "+ this.ToString());
        if (collision.gameObject.name != "UFO")
        {
            KnockDown();
        }
        
    }
    void KnockDown()
    {
        transform.localEulerAngles = new Vector3(90f, 0, 0);
        /*
        foreach (Transform child in transform)
        {
            //child.Rotate(90f,0f,0f,Space.World);
        }*/

    }
}
