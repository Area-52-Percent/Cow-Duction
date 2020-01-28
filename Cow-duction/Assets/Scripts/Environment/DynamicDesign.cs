using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicDesign : MonoBehaviour
{
    [SerializeField] private Material[] barnMaterials = null; // Set up in inspector
    [SerializeField] private Material[] barnWindowMaterials = null; // Set up in inspector
    [SerializeField] private GameObject[] trees = null; // Set up in inspector
    [SerializeField] private GameObject[] barns = null; // Set up in inspector
    [SerializeField] private GameObject[] environments = null; // Set up in inspector
    private List<GameObject> barnSpawnPoints;
    private List<GameObject> treeSpawnPoints;
    private int styleSelector;

    void Start()
    {
        styleSelector = Mathf.RoundToInt(Random.Range(0.0f, 2.0f));
        Debug.Log("Dynamic Design started");
        if (Random.Range(0.0f, 1000.0f) <= 500.0f)
        {
            environments[0].gameObject.SetActive(true);
            environments[1].gameObject.SetActive(false);
        }
        else
        {
            environments[0].gameObject.SetActive(false);
            environments[1].gameObject.SetActive(true);
        }

        barnSpawnPoints = new List<GameObject>();
        barnSpawnPoints.AddRange(GameObject.FindGameObjectsWithTag("Barn"));
        treeSpawnPoints = new List<GameObject>();
        treeSpawnPoints.AddRange(GameObject.FindGameObjectsWithTag("Tree"));

        Debug.Log(barnSpawnPoints.Count);

        foreach (GameObject spawnPoint in barnSpawnPoints)
        {
            placeBarn(spawnPoint);
        }
        foreach (GameObject spawnPoint in treeSpawnPoints)
        {
            RandomizeTree(spawnPoint);
        }
    }

    private void placeBarn(GameObject spawnPoint)
    {
        spawnPoint.transform.GetChild(0).GetComponent<Renderer>().material = barnMaterials[styleSelector];
        spawnPoint.transform.GetChild(2).GetChild(0).GetComponent<Renderer>().material = barnMaterials[styleSelector];
        spawnPoint.transform.GetChild(3).GetChild(0).GetComponent<Renderer>().material = barnMaterials[styleSelector];
        spawnPoint.transform.GetChild(4).GetChild(0).GetComponent<Renderer>().material = barnWindowMaterials[styleSelector];
    }

    private void RandomizeTree(GameObject tree)
    {
        float xPos = tree.transform.position.x;
        float yPos = tree.transform.position.y;
        float zPos = tree.transform.position.z;
        tree.SetActive(false);
        int randomTreeType = Mathf.RoundToInt(Random.Range(0.0f, 1.0f));
        if (randomTreeType == 0)
        {
            yPos = yPos + 3f;
        }
        GameObject treeClone = Instantiate(trees[randomTreeType], new Vector3(xPos, yPos, zPos), Quaternion.identity);
        treeClone.transform.SetParent(GameObject.Find("Trees").transform);
    }
}
