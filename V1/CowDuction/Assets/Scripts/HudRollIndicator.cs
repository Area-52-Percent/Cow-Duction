using UnityEngine;

public class HudRollIndicator : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Camera.main && Camera.main.enabled)
            transform.localEulerAngles = Vector3.forward * -Camera.main.transform.eulerAngles.z;
    }
}
