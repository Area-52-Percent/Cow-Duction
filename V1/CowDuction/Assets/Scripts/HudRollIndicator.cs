using UnityEngine;

public class HudRollIndicator : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(transform.localEulerAngles);
        float angle = Camera.main.transform.eulerAngles.z;
        transform.localEulerAngles = Vector3.forward * -angle;
    }
}
