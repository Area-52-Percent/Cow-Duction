using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllers : MonoBehaviour
{
    public static GameControllers Instance { get; private set; }

    public Controllers controllers { get; private set; }
    // Start is called before the first frame update
   
    private void CreateSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        CreateSingleton();
        controllers = transform.Find("Controllers").GetComponent<Controllers>();
    }
}
