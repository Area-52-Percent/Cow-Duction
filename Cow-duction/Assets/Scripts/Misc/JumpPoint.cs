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
        GameObject.Find("UFO").gameObject.transform.position = jumpLocations.transform.Find("UFO Loc").transform.position;
        for (int j = 0; j < GameObject.FindGameObjectsWithTag("Farmer").Length; j++)
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
                curfarmer = "Farmer " + (j+1) + " Loc";
            }
            if (j < 3)
            {
                GameObject.Find(farmer).gameObject.transform.position = jumpLocations.transform.Find(curfarmer).transform.position;
            }
            else
            {
                GameObject.Find(farmer).gameObject.transform.position = jumpLocations.transform.Find("Farmer 1 Loc").transform.position;
            }
        }
    }
}
