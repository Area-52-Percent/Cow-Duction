/*  SC_RadarCamera.cs

    Causes an object to follow another object at a set height difference.

    Assumptions:
        This component belongs to a camera pointing downward.
        There is an object in the scene named "UFO".
 */

using UnityEngine;

public class SC_RadarCamera : MonoBehaviour
{
    // Private variables
    private GameObject followObject;
    private Camera m_Camera;

    // Serialized private variables
    [SerializeField] private float height = 15.0f;
    [SerializeField] private float orthoSizeMultiplier = 2f;

    // Awake is called after all objects are initialized
    void Awake()
    {
        followObject = GameObject.Find("UFO");
        m_Camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (followObject) {

            if (m_Camera.orthographic)
            {
                GetComponent<Camera>().orthographicSize = height * orthoSizeMultiplier;
            }

            Vector3 followPosition = followObject.transform.position;
            
            followPosition.y += height;
            transform.position = followPosition;

            transform.localEulerAngles = new Vector3(90f, 0, -followObject.transform.localEulerAngles.y);
        }
    }
}
