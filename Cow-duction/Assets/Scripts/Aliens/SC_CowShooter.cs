using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SC_SpaceshipMovement))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class SC_CowShooter : MonoBehaviour
{
    private SC_AlienUIManager uiManager;
    private SC_SpaceshipMovement spaceshipMovement;
    private float obtainedCows;

    public float shootForce;
    public GameObject cow;
    private void Awake()
    {
        spaceshipMovement = GetComponent<SC_SpaceshipMovement>();
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<SC_AlienUIManager>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(obtainedCows);
        }
    }
    public void AddCow()
    {
        obtainedCows++;
    }
}
