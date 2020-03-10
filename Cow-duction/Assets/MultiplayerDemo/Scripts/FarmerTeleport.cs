using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class FarmerTeleport : NetworkBehaviour
{
    public GameObject Map;
    public float radius = 3f;
    public List<Button> Buttons;
    private List<Vector3> locations;

    private void Awake()
    {
        locations = new List<Vector3>();
        locations.Add(GameObject.Find("TeleportPoint1").transform.position);
        locations.Add(GameObject.Find("TeleportPoint3").transform.position);
        locations.Add(GameObject.Find("TeleportPoint2").transform.position);
    }
    private void Teleport(Vector3 pos)
    {
        gameObject.GetComponent<MultiPlayerFarmerController>().MoveFarmer(pos);
        Map.SetActive(false);
    }

    public void OpenMenu()
    {
        if (Map.activeSelf)
            Map.SetActive(false);
        else
            Map.SetActive(true);
    }
    public void Teleport1()
    {
        Vector3 pos = new Vector3(locations[0].x + Random.Range(-radius, radius), locations[0].y, locations[0].z + Random.Range(-radius, radius));
    }
    public void Teleport2()
    {
        Vector3 pos = new Vector3(locations[1].x + Random.Range(-radius, radius), locations[1].y, locations[1].z + Random.Range(-radius, radius));
        Teleport(pos);
    }
    public void Teleport3()
    {
        Vector3 pos = new Vector3(locations[2].x + Random.Range(-radius, radius), locations[2].y, locations[2].z + Random.Range(-radius, radius));
        Teleport(pos);
    }
}
