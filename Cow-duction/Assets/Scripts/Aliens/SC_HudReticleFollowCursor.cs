/*  SC_HudReticleFollowCursor.cs

    Causes an object to follow the cursor position. 

    Assumptions:
        This component belongs to a UI image.
 */

using UnityEngine;

public class SC_HudReticleFollowCursor : MonoBehaviour
{
    [SerializeField] private float joystickSensitivity = 1.0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetJoystickNames().Length > 1)
        {
            float horizontal = Input.GetAxis("ReticleHorizontal");
            float vertical = Input.GetAxis("ReticleVertical");
            if (Mathf.Abs(horizontal) > 0f)
            {
                transform.position += Vector3.right * horizontal * joystickSensitivity;
            }
            if (Mathf.Abs(vertical) > 0f)
            {
                transform.position += Vector3.up * vertical * joystickSensitivity;
            }
        }
        transform.position = Input.mousePosition;
    }
}
