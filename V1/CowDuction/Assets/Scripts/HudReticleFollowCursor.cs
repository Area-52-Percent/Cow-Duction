using UnityEngine;

public class HudReticleFollowCursor : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
    }
}
