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
    private float obtainedCows;

    public float shootForce;
    public GameObject cow;
    private void Awake()
    {
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<SC_AlienUIManager>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && Time.timeScale > Mathf.Epsilon)
        {
            if (obtainedCows >= 0)
            {
                Instantiate(cow, transform.position, transform.rotation);
            }
        }
    }
    public void AddCow()
    {
        obtainedCows++;
    }
}
