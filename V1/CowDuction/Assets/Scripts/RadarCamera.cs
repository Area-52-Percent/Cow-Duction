using UnityEngine;

public class RadarCamera : MonoBehaviour
{
    [SerializeField] private GameObject followObject;
    public float height = 15.0f;

    // Awake is called after all objects are initialized
    void Awake()
    {
        followObject = GameObject.Find("UFO");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 followPosition = followObject.transform.position;
        
        followPosition.y += height;
        transform.position = followPosition;
    }
}
