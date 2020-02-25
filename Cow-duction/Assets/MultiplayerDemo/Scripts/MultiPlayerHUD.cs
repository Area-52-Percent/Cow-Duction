using UnityEngine;
using Mirror;

public class MultiPlayerHUD : NetworkBehaviour
{
    [Tooltip("The mesh which should visible to observers, but not visible from the first person perspective")]
    public GameObject[] thirdPersonMeshes;
    [Tooltip("The Canvas component which should only be visible to this object")]
    public Canvas hud;
    public Camera cameraTransform;

    private void OnValidate()
    {
        if (cameraTransform == null)
            cameraTransform = GetComponentInChildren<Camera>();
    }

    // OnStartLocalPlayer is called when the local player object is set up
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        // Parent main camera to cameraTransform
        Camera.main.orthographic = false;
        Camera.main.transform.SetParent(cameraTransform.transform);
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localEulerAngles = Vector3.zero;

        Camera.main.cullingMask = ~(1 << LayerMask.NameToLayer("UI"));
        cameraTransform.enabled = true;
    }

    // OnDisable is called when the object is removed from the server
    private void OnDisable()
    {
        if (isLocalPlayer)
        {
            // Return main camera to starting position
            Camera.main.transform.SetParent(null);
            Camera.main.transform.localPosition = new Vector3(30f, 15f, -10f); // Hard-coded position
            Camera.main.transform.localEulerAngles = Vector3.zero;

            Camera.main.cullingMask = -1;
            cameraTransform.enabled = false;
        }
    }

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
