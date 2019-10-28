using UnityEngine;

public class HudRollIndicator : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.localEulerAngles = Vector3.forward * -Camera.main.transform.eulerAngles.z;
    }
}
