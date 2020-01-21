using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MultiPlayerHUD : NetworkBehaviour
{
    public GameObject thirdPersonMesh;
    public Canvas hud; // The local player's HUD

    void Start()
    {
        hud = GetComponentInChildren<Canvas>();
        if (isLocalPlayer)
        {
            hud.enabled = true;
            if (thirdPersonMesh != null)
                thirdPersonMesh.SetActive(false);
        }
        else if (hud.enabled)
        {
            hud.enabled = false;
        }
    }
}
