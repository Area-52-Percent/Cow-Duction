using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MultiPlayerSpaceshipController : NetworkBehaviour
{
    Rigidbody spaceshipRigidbody;

    public float moveSpeed = 20f;

    void Start()
    {
        spaceshipRigidbody = GetComponent<Rigidbody>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        // Parent main camera to spaceship
        Camera.main.orthographic = false;
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = new Vector3(0f, 3f, -8f);
        Camera.main.transform.localEulerAngles = new Vector3(10f, 0f, 0f);
    }

    void OnDisable()
    {
        if (isLocalPlayer)
        {
            // Return main camera to starting position
            Camera.main.transform.SetParent(null);
            Camera.main.transform.localPosition = new Vector3(0f, 10f, -10f);
            Camera.main.transform.localEulerAngles = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal"); // Controller: (LeftJoystick X-axis) Keyboard: (A)(D)
        float vertical = Input.GetAxis("Vertical");     // Controller: (LeftJoystick Y-axis) Keyboard: (W)(S)
        float lift = Input.GetAxis("Lift");             // Controller: (RB)(LB)              Keyboard: (Z)(C)
        ForceMode forceMode = ForceMode.VelocityChange;

        spaceshipRigidbody.AddForce(-Physics.gravity.y * Vector3.up);

        if (Mathf.Abs(horizontal) > 0f)
        {
            spaceshipRigidbody.AddForce(transform.right * horizontal * moveSpeed * Time.fixedDeltaTime, forceMode);
        }
        if (Mathf.Abs(vertical) > 0f)
        {
            spaceshipRigidbody.AddForce(transform.forward * vertical * moveSpeed * Time.fixedDeltaTime, forceMode);
        }
        if (Mathf.Abs(lift) > 0f)
        {
            spaceshipRigidbody.AddForce(transform.up * lift * moveSpeed * Time.fixedDeltaTime, forceMode);
        }
    }
}
