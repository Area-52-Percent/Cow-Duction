using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AmmoSack : NetworkBehaviour
{
    public GameObject myProjectile;
    [ServerCallback]
    public void AmmoSwap(GameObject farmer)
    {
        farmer.GetComponent<MultiPlayerFarmerController>().projectile = myProjectile;
    }
}
