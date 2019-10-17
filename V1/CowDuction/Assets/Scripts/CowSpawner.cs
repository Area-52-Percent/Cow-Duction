using UnityEngine;

public class CowSpawner : MonoBehaviour
{
    public GameObject cowPrefab;
    public int cowAmount = 10;
    public float radius = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Error checking
        if (cowPrefab == null)
            Debug.LogError("Cow prefab not assigned");
        if (cowAmount < 1)
            cowAmount = 10;
        if (radius < 1)
            radius = 10;

        // Spawn cows        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
