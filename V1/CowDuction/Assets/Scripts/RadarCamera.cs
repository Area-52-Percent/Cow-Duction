using UnityEngine;

public class RadarCamera : MonoBehaviour
{
    public GameObject followObject;
    public float height = 15.0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 followPosition = followObject.transform.position;
        
        followPosition.y = height;
        transform.position = followPosition;        
    }
}
