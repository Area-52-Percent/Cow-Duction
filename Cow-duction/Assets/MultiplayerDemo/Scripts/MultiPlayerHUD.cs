using UnityEngine;
using Mirror;

public class MultiPlayerHUD : NetworkBehaviour
{
    [Tooltip("The mesh which should visible to observers, but not visible from the first person perspective")]
    public GameObject thirdPersonMesh;
    [Tooltip("The Canvas component which should only be visible to this object")]
    public Canvas hud;

    // Start is called before the first frame update
    private void Start()
    {
        hud = GetComponentInChildren<Canvas>();
        if (isLocalPlayer)
        {
            hud.enabled = true;
            if (thirdPersonMesh != null)
                thirdPersonMesh.GetComponent<SkinnedMeshRenderer>().enabled = false;
        }
        else if (hud.enabled)
        {
            hud.enabled = false;
        }
    }
}
