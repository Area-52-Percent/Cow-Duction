using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Mirror;

public class JumpPoint : NetworkBehaviour
{
    public GameObject[] jumpPoints;

    [ClientRpc]
    public void RpcJumpAroundSpaceship(int jumpPoint)
    {
        string jumpGroup = "Location " + jumpPoint;
        GameObject[] jumpLocations = GameObject.FindGameObjectsWithTag("JumpLocation");
        GameObject.Find("Spaceship(Clone)").gameObject.transform.position = jumpLocations[0 + (4 * (jumpPoint - 1))].transform.position;
        GameObject.Find("Spaceship(Clone)").gameObject.transform.rotation = jumpLocations[0 + (4 * (jumpPoint - 1))].transform.rotation;
    }

    [ClientRpc]
    public void RpcJumpAroundFarmer(int jumpPoint, GameObject currentFarmer, int curJump)
    {
        string jumpGroup = "Location " + jumpPoint;
        GameObject[] jumpLocations = GameObject.FindGameObjectsWithTag("JumpLocation");

        for (int i = 0; i < 10; i++)
        {
            currentFarmer.gameObject.transform.position = new Vector3(0, 0, 0);
            currentFarmer.gameObject.transform.position = jumpLocations[curJump + (4 * (jumpPoint - 1))].transform.position;
            currentFarmer.gameObject.transform.rotation = jumpLocations[curJump + (4 * (jumpPoint - 1))].transform.rotation;
        }

        /*GameObject[] farmers = GameObject.FindGameObjectsWithTag("Farmer");
        Debug.Log(jumpGroup);
        for (int j = 0; j < farmers.Length; j++)
        {
            string curfarmer;
            if (j == 0)
                curfarmer = "Farmer 1 Loc";
            else
                curfarmer = "Farmer " + (j + 1) + " Loc";

            if (j < 3)
            {
                Debug.Log("Current Farmer Loc Variable = " + curfarmer);
                Debug.Log("Current Farmer Location = " + farmers[j].gameObject.transform.position);
                Debug.Log("Farmer going to = " + jumpLocations[j + (4 * (jumpPoint - 1))].transform.position);
                for (int i = 0; i < 10; i++)
                {
                    farmers[j].gameObject.transform.position = new Vector3(0, 0, 0);
                    farmers[j].gameObject.transform.position = jumpLocations[j + (4 * (jumpPoint - 1))].transform.position;
                    farmers[j].gameObject.transform.rotation = jumpLocations[j + (4 * (jumpPoint - 1))].transform.rotation;
                }
                //GameObject.FindGameObjectsWithTag("Farmer")[j].gameObject.GetComponent<NetworkTransform>().transform.position = jumpLocations.transform.Find(curfarmer).transform.position;
                //GameObject.FindGameObjectsWithTag("Farmer")[j].gameObject.GetComponent<NetworkTransform>().transform.rotation = jumpLocations.transform.Find(curfarmer).transform.rotation;
                Debug.Log("Farmer at = " + farmers[j].gameObject.transform.position);
            }
            else
            {
                curfarmer = "Farmer 1 Loc";
                Debug.Log("Farmer going to = " + jumpLocations[j + (4 * (jumpPoint - 1))].transform.position);
                for (int i = 0; i < 10; i++)
                {
                    farmers[j].gameObject.transform.position = new Vector3(0, 0, 0);
                    farmers[j].gameObject.transform.position = jumpLocations[1 + (4 * (jumpPoint - 1))].transform.position;
                    farmers[j].gameObject.transform.rotation = jumpLocations[1 + (4 * (jumpPoint - 1))].transform.rotation;
                }
            }
        }
        */
    }
}