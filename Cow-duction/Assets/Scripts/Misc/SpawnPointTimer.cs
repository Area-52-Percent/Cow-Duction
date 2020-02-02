using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointTimer : MonoBehaviour
{
    [SerializeField] public float elapsedTime = 0.0f;
    [SerializeField] public float specialElapsedTime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        specialElapsedTime += Time.deltaTime;
    }
}
