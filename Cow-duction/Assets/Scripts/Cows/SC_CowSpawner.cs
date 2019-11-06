/*  SC_CowSpawner.cs

    Spawns cows in regular intervals at input spawn locations.

    Assumptions:
        There is a GameObject in the scene with the "UIManager" tag and SC_AlienUIManager component.
        A cow prefab and spawn point transforms are set up before use.
 */

using UnityEngine;
using System.Collections.Generic;

public class SC_CowSpawner : MonoBehaviour
{
    public GameObject cowPrefab;
    public int maxCowAmount = 10;
    public float radius = 10.0f;
    public float spawnRate = 5f;
    public int intialSpawnAmount = 9;
    public List<GameObject> spawnPoints;

    [SerializeField] private int cowAmount = 0;
    [SerializeField] private float elapsedTime = 0.0f;
    [SerializeField] private int nextSpawnLoc = 0;

    // Awake is called after all objects are initialized
    void Awake()
    {
        GameObject.FindWithTag("UIManager").GetComponent<SC_AlienUIManager>().CowSpawner = this.gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {           
        // Error checking
        if (cowPrefab == null)
            Debug.LogError("Cow prefab not assigned");
        if (maxCowAmount < 1)
            maxCowAmount = 10;
        if (radius < 1)
            radius = 10;
        
        foreach (GameObject spawnPoint in spawnPoints)
        {
            SpawnCows(spawnPoint, intialSpawnAmount / spawnPoints.Count);
        }
    }

    // Update is called once per frame
    void Update()
    {
        cowAmount = GameObject.FindGameObjectsWithTag("Cow").Length;
        elapsedTime += Time.deltaTime;
        //Debug.Log(cowAmount + "/" + maxCowAmount + "/" + elapsedTime + "/" + spawnRate);
        if(cowAmount < maxCowAmount && elapsedTime > spawnRate)
        {
            SpawnCows(spawnPoints[nextSpawnLoc], 1);
            nextSpawnLoc = ((nextSpawnLoc + 1) % spawnPoints.Count);
            elapsedTime = 0.0f;
        }
    }

    // Spawn <Amount> number of cows at <spawnPoint> position
    private void SpawnCows(GameObject spawnPoint, int Amount)
    {
        for (int i = 0; i < Amount; i++)
        {
            float xPos = spawnPoint.transform.position.x + Random.Range(-radius, radius);
            float zPos = spawnPoint.transform.position.z + Random.Range(-radius, radius);
            GameObject cowClone = Instantiate(cowPrefab, new Vector3(xPos, spawnPoint.transform.position.y, zPos), Quaternion.identity);
            cowClone.transform.parent = this.transform;
        }
    }
}
