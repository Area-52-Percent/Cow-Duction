using UnityEngine;
using System.Collections.Generic;
using Mirror;

/// <summary>
/// Spawns cows in regular intervals at input spawn locations.
/// </summary>
/// <remarks>
/// <para>A cow prefab and spawn point transforms should be set up before use.</para>
/// </remarks>
public class MultiPlayerCowSpawner : NetworkBehaviour
{
    private List<GameObject> spawnPoints;
    private GameObject UFOLoc;
    private int cowAmount = 0;

    [Tooltip("Place cow prefabs in this array")]
    public GameObject[] cows = null;
    [Tooltip("Spawn ratios of the above array, as a decimal between 0 and 1 (where 1 is a 100% spawn chance)")]
    public float[] cowSpawnRatios = null;
    
    [Header("Parameters")]
    public int maxCowAmount = 10;
    public float radius = 10.0f;
    public float spawnRate = 5f;
    public int intialSpawnAmount = 9;
    public float randomFactor = 0.1f;

    // OnDisable is called when the object is removed from the server
    private void OnDisable()
    {
        foreach(Transform cow in transform)
        {
            Destroy(cow.gameObject);
        }
    }

    // Awake is called after all objects are initialized
    private void Awake()
    {
        // UFOLoc = GameObject.FindWithTag("UFO");
        spawnPoints = new List<GameObject>();
        spawnPoints.AddRange(GameObject.FindGameObjectsWithTag("CowSpawn"));
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Error checking
        if (cows.Length < 1)
            Debug.LogError("No cows assigned");
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
    private void Update()
    {
        cowAmount = GameObject.FindGameObjectsWithTag("Cow").Length;
        
        if (cowAmount < maxCowAmount)
        {
            if (UFOLoc == null)
                UFOLoc = GameObject.FindGameObjectWithTag("UFO");
            
            //Compare spawnpoints elapsed time
            float[] ElapsedTimes = new float[spawnPoints.Count];
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                // ((1 + (k/n)) ^ n) / e ^ k where n is the distance between the ufo a given spawn
                float fx = ( (Mathf.Pow(1 + (5 / Vector3.Distance(spawnPoints[i].transform.position, UFOLoc.transform.position)), Vector3.Distance(spawnPoints[i].transform.position, UFOLoc.transform.position)) / Mathf.Exp(5)));
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
    [Server]
    private void SpawnCows(GameObject spawnPoint, int Amount)
    {
        for (int i = 0; i < Amount; i++)
        {
            float xPos = spawnPoint.transform.position.x + Random.Range(-radius, radius);
            float zPos = spawnPoint.transform.position.z + Random.Range(-radius, radius);
            for (int c = 0; c < cows.Length; c++)
            {
                if (Random.value <= cowSpawnRatios[c])
                {
                    GameObject cowClone = Instantiate(cows[c], new Vector3(xPos, spawnPoint.transform.position.y, zPos), Quaternion.identity);
                    RandomizeCow(cowClone);
                    cowClone.transform.parent = transform;
                    NetworkServer.Spawn(cowClone);
                    break;
                }
            }
        }
    }

    // Randomize cow parameters
    private void RandomizeCow(GameObject cow)
    {
        MultiPlayerCowBrain cowBrain = cow.GetComponent<MultiPlayerCowBrain>();
        Rigidbody cowRigidbody = cow.GetComponent<Rigidbody>();

        float mass = cowRigidbody.mass;
        float size = cow.transform.localScale.x; // Assume scale is uniform
        float milk = cowBrain.milk;
        float maxSpeed = cowBrain.maxSpeed;
        float maxWanderTime = cowBrain.maxWanderTime;

        size = Random.Range(size - (size * randomFactor), size + (size * randomFactor));
        mass = Random.Range(mass - (mass * randomFactor), mass + (mass * randomFactor)) + size;
        milk = Random.Range(milk - (milk * randomFactor), milk + (milk * randomFactor)) + size;
        maxSpeed = Random.Range(maxSpeed - (maxSpeed * randomFactor), maxSpeed + (maxSpeed * randomFactor)) + size;
        maxWanderTime = Random.Range(maxWanderTime - (maxWanderTime * randomFactor), maxWanderTime + (maxWanderTime * randomFactor)) - size;

        cowRigidbody.mass = mass;
        cow.transform.localScale *= size;
        cowBrain.milk = milk;
        cowBrain.maxWanderTime = maxWanderTime;
        cowBrain.SetMaxSpeed(maxSpeed);
    }
}
