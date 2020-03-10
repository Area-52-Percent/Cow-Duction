using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class FarmerTeleport : NetworkBehaviour
{
    public GameObject Map;
    public List<Button> Buttons;
    public List<Transform> Location;

    void Awake()
    {
        for (int i = 0; i < Buttons.Count; i++)
        {
            Buttons[i].onClick.AddListener(delegate { Teleport(Location[i].position); });
        }
    }
    void Update()
    {

    }

    public void Teleport(Vector3 loc)
    {
        gameObject.GetComponent<MultiPlayerFarmerController>().MoveFarmer(loc);
    }
}
