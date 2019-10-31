using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CowSpawner : MonoBehaviour
{
    public GameObject cowPrefab;
    public int maxCowAmount = 10;
    public float radius = 10.0f;
    public float spawnRate = 5f;
    public int intialSpawnAmount = 9;
    public List<GameObject> spawnPoints;
    public Slider maxCowSlider;
    public Slider cowRateSlider;

    private int cowAmount = 0;
    private float elapsedTime = 0.0f;
    private int nextSpawnLoc = 0;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("UI").GetComponent<UIManager>().CowSpawner = this.gameObject;
        
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

    private void SpawnCows(GameObject spawnPoint, int Amount)
    {
        for (int i = 0; i < Amount; i++)
        {
            float xPos = spawnPoint.transform.position.x + Random.Range(0, radius);
            float zPos = spawnPoint.transform.position.z + Random.Range(0, radius);
            Instantiate(cowPrefab, new Vector3(xPos, spawnPoint.transform.position.y, zPos), Quaternion.identity);
        }
    }
}
