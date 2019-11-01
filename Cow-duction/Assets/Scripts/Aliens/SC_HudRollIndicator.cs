/*  SC_HudRollIndicator.cs

    Updates the rotation of the roll indicator.

    Assumptions:
        This component belongs to a UI image.
 */

using UnityEngine;

public class SC_HudRollIndicator : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Camera.main && Camera.main.enabled)
            transform.localEulerAngles = Vector3.forward * -Camera.main.transform.eulerAngles.z;
    }
}
