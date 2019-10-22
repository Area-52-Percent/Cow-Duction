using UnityEngine;

public class HudRollIndicator : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        float angle = Camera.main.transform.eulerAngles.z;
        transform.localEulerAngles = Vector3.forward * -angle;
    }
}
