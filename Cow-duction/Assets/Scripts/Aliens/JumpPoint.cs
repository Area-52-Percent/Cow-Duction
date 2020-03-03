using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPoint : MonoBehaviour
{
    public GameObject[] jumpPoints;

    public void JumpAround(int jumpPoint)
    {
        string jumpGroup = "Location " + jumpPoint;
        GameObject jumpLocations = GameObject.Find(jumpGroup);
        Debug.Log(jumpGroup);
        GameObject.Find("Spaceship(Clone)").gameObject.transform.position = jumpLocations.transform.Find("UFO Loc").transform.position;
        GameObject.Find("Spaceship(Clone)").gameObject.transform.rotation = jumpLocations.transform.Find("UFO Loc").transform.rotation;
        Debug.Log(GameObject.Find("Spaceship(Clone)").gameObject.transform.position);
        Debug.Log(GameObject.Find("Spaceship(Clone)").gameObject.transform.rotation);
        for (int j = 0; j < GameObject.FindGameObjectsWithTag("FarmerPlayer").Length; j++)
        {
            string farmer;
            string curfarmer;
            if (j == 0)
            {
                farmer = "Farmer";
                curfarmer = "Farmer 1 Loc";
            }
            else
            {
                farmer = "Farmer (" + j + ")";
                curfarmer = "Farmer " + (j + 1) + " Loc";
            }

            if (j < 3)
            {
                Debug.Log("Current Farmer = " + farmer);
                Debug.Log("Current Farmer Loc Variable = " + curfarmer);
                Debug.Log("Current Farmer Location = " + GameObject.Find(farmer).gameObject.transform.position);
                Debug.Log("Farmer going to = " + jumpLocations.transform.Find(curfarmer).transform.position);
                GameObject.Find(farmer).gameObject.transform.position = new Vector3(0, 0, 0);
                GameObject.Find(farmer).gameObject.transform.position = jumpLocations.transform.Find(curfarmer).transform.position;
                GameObject.Find(farmer).gameObject.transform.rotation = jumpLocations.transform.Find(curfarmer).transform.rotation;
                Debug.Log("Farmer at = " + GameObject.Find(farmer).gameObject.transform.position);
            }
        }
    }
}