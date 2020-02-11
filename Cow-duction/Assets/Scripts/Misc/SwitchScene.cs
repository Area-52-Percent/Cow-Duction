using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    private SC_SpaceshipMovement player1;
    private SC_CowAbduction player2;
    void Start()
    {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("j1Fire1") && !Input.GetKey(KeyCode.Escape))
        {
            Debug.Log("hi");
            player1.player = "j1";
            player2.player = "j2";
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
        else if(Input.GetButton("j2Fire1") && !Input.GetKey(KeyCode.Escape))
        {
            Debug.Log("hello");
            player1.player = "j2";
            player2.player = "j1";
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }


}
