using UnityEngine;
using Mirror;

public class MultiPlayerHUD : NetworkBehaviour
{
    [Tooltip("The mesh which should visible to observers, but not visible from the first person perspective")]
    public GameObject[] thirdPersonMeshes;
    [Tooltip("The Canvas component which should only be visible to this object")]
    public Canvas hud;

    // Start is called before the first frame update
    private void Start()
    {
        hud = GetComponentInChildren<Canvas>();
        if (isLocalPlayer)
        {
            hud.enabled = true;
            if (thirdPersonMeshes.Length > 0)
            {
                foreach(GameObject mesh in thirdPersonMeshes)
                {
                    try {
                        mesh.GetComponent<MeshRenderer>().enabled = false;
                    }
                    catch {
                        try {
                            mesh.GetComponent<SkinnedMeshRenderer>().enabled = false;
                        }
                        catch {
                            Debug.Log ("No MeshRender or SkinnedMeshRenderer on GameObject mesh"); 
                        }
                    }
                }
            }
        }
        else if (hud.enabled)
        {
            hud.enabled = false;
        }
    }
}
