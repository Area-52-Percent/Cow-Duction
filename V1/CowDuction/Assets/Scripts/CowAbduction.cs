using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowAbduction : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Clicked!");
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Debug.Log("Ray cast hit " + hit.transform.gameObject.name);
                if (hit.collider.tag == "Cow")
                {
                    Debug.Log("Cow has been picked up");
                    Destroy(hit.transform.gameObject);
                }
            }
        }
    }
}
