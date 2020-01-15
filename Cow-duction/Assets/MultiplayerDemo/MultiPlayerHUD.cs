using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MultiPlayerHUD : NetworkBehaviour
{
    public Canvas hud; // The local player's HUD

    void Start()
    {
        hud = GetComponentInChildren<Canvas>();
        if (isLocalPlayer)
        {
            hud.enabled = true;
        }
        else if (hud.enabled)
        {
            hud.enabled = false;
        }
    }
}
