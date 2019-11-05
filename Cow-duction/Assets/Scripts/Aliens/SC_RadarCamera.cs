/*  SC_RadarCamera.cs

    Causes an object to follow another object at a set height difference.

    Assumptions:
        This component belongs to a camera pointing downward.
        There is an object in the scene named "UFO".
 */

using UnityEngine;

public class SC_RadarCamera : MonoBehaviour
{
    [SerializeField] private GameObject followObject;
    [SerializeField] private float height = 15.0f;

    // Awake is called after all objects are initialized
    void Awake()
    {
        followObject = GameObject.Find("UFO");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (followObject) {
            Vector3 followPosition = followObject.transform.position;
            
            followPosition.y += height;
            transform.position = followPosition;
        }
    }
}
