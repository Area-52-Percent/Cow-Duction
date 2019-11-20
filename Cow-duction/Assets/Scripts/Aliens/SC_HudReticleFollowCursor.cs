/*  SC_HudReticleFollowCursor.cs

    Causes an object to follow the cursor position, or move by input from controller 2.

    Assumptions:
        This component belongs to a UI image.
 */

using UnityEngine;

public class SC_HudReticleFollowCursor : MonoBehaviour
{
    public float joystickSensitivity = 5.0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetJoystickNames().Length > 1)
        {
            // Take input from controller 2
            float horizontal = Input.GetAxis("ReticleHorizontal");
            float vertical = Input.GetAxis("ReticleVertical");
            
            if (horizontal > 0 && transform.position.x < Screen.width ||
                horizontal < 0 && transform.position.x > 0)
            {
                transform.position += Vector3.right * horizontal * joystickSensitivity;
            }
            if (vertical < 0 && transform.position.y < Screen.height ||
                vertical > 0 && transform.position.y > 0)
            {
                transform.position += Vector3.down * vertical * joystickSensitivity;
            }
        }
        else
        {
            // Use input mouse position
            transform.position = Input.mousePosition;
        }
    }
}
