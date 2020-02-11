using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SwitchScene : MonoBehaviour
{
    private InputMaster controls;

    private void Awake()
    {
        controls = new InputMaster();
        controls.Shooter.Shoot.performed += context => Shoot();
        controls.Driver.Movement.performed += context => Move();
    }
    void Start()
    {
        Time.timeScale = 1;
    }

    void Shoot()
    {
        Debug.Log("shoot");
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    private void Move()
    {
        Debug.Log("moving in scene");
        SceneManager.LoadScene(1, LoadSceneMode.Single);

    }
    
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey && !Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }
}
