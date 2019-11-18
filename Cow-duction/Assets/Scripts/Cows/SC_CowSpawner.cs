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
    public GameObject UFOLoc;

    [SerializeField] private int cowAmount = 0;

    // Awake is called after all objects are initialized
    void Awake()
    {
        GameObject.FindWithTag("UIManager").GetComponent<SC_AlienUIManager>().CowSpawner = this.gameObject;
        UFOLoc = GameObject.Find("UFO");
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
        
        if (cowAmount < maxCowAmount)
        {
            //Compare spawnpoints elapsed time
            float[] ElapsedTimes = new float[spawnPoints.Count];
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                // ((1 + (k/n)) ^ n) / e ^ k where n is the distance between the ufo a given spawn
                float fx = ( (Mathf.Pow(1 + (5 / Vector3.Distance(spawnPoints[i].transform.position, UFOLoc.transform.position)), Vector3.Distance(spawnPoints[i].transform.position, UFOLoc.transform.position)) / Mathf.Exp(5)));
                Debug.Log(i + "?" + Vector3.Distance(spawnPoints[i].transform.position, UFOLoc.transform.position) + "-" + fx);
                ElapsedTimes[i] = spawnPoints[i].GetComponent<SpawnPointTimer>().elapsedTime * fx;
            }
            float MaxSinceLastSpawn = Mathf.Max(ElapsedTimes);
            if(MaxSinceLastSpawn > spawnRate)
            {
                int SpawnLoc = System.Array.IndexOf(ElapsedTimes, Mathf.Max(ElapsedTimes));
                spawnPoints[SpawnLoc].GetComponent<SpawnPointTimer>().elapsedTime = 0.0f;
                SpawnCows(spawnPoints[SpawnLoc], 1) ;
            }

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
