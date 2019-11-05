/*  SC_HudReticleFollowCursor.cs

    Causes an object to follow the cursor position. 

    Assumptions:
        This component belongs to a UI image.
 */


using UnityEngine;

public class SC_HudReticleFollowCursor : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
    }
}
