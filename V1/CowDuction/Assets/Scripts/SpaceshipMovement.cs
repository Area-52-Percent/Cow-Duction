﻿using UnityEngine;

public class SpaceshipMovement : MonoBehaviour
{
    [SerializeField] CowAbduction cowAbduction;
    [SerializeField] private Rigidbody _rb;
    public float speed = 10.0f;
    public float maxHeight = 15.0f;
    public float minHeight = 10.0f;
    public float rotationForce = 0.05f;
    public float maxRotation = 20.0f;
    public bool invertLook = false;

    // Start is called before the first frame update
    void Start()
    {
        cowAbduction = GetComponent<CowAbduction>();
        _rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 horizontalForce = Vector3.zero;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        // float mouseXInput = Input.GetAxis("Mouse X");
        // float mouseYInput = Input.GetAxis("Mouse Y");

        if (Mathf.Abs(horizontalInput) > 0.0f)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                horizontalForce = transform.right;
                horizontalForce.y = 0;
                _rb.AddForce(horizontalForce * horizontalInput * speed, ForceMode.Acceleration);
                if (horizontalInput < 0 && (transform.eulerAngles.z < maxRotation || transform.eulerAngles.z > 350.0f - maxRotation) || horizontalInput > 0 && (transform.eulerAngles.z > 360.0f - maxRotation || transform.eulerAngles.z < maxRotation + 10.0f))
                {
                    _rb.AddRelativeTorque(Vector3.back * horizontalInput * rotationForce, ForceMode.Acceleration);
                }
            }
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                _rb.AddRelativeTorque(Vector3.up * horizontalInput * rotationForce * 2, ForceMode.Acceleration);
            }
        }
        else
        { // Rotate z axis back upright
            if (transform.localEulerAngles.z > maxRotation && transform.localEulerAngles.z < 180.0f)
            {
                _rb.AddRelativeTorque(Vector3.back * rotationForce, ForceMode.Acceleration);
            }
            else if (transform.localEulerAngles.z < 360.0f - maxRotation && transform.localEulerAngles.z > 180.0f)
            {
                _rb.AddRelativeTorque(Vector3.forward * rotationForce, ForceMode.Acceleration);
            }
        }

        if (Mathf.Abs(verticalInput) > 0.0f)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            {
                horizontalForce = transform.forward;
                horizontalForce.y = 0;
                _rb.AddForce(horizontalForce * verticalInput * speed, ForceMode.Acceleration);
                if (verticalInput > 0 && (transform.localEulerAngles.x < maxRotation || transform.localEulerAngles.x > 350.0f - maxRotation) || verticalInput < 0 && (transform.localEulerAngles.x > 360.0f - maxRotation || transform.localEulerAngles.x < maxRotation + 10.0f))
                {
                    _rb.AddRelativeTorque(Vector3.right * verticalInput * rotationForce, ForceMode.Acceleration);
                }
            }
            if ((Input.GetKey(KeyCode.UpArrow) && (transform.localEulerAngles.x > 280.0f || transform.localEulerAngles.x < 90.0f)) || 
                (Input.GetKey(KeyCode.DownArrow) && (transform.localEulerAngles.x < 80.0f || transform.localEulerAngles.x > 270.0f)))
            {
                _rb.AddRelativeTorque((invertLook ? Vector3.right : Vector3.left) * verticalInput * rotationForce * 2, ForceMode.Acceleration);
            }
        }
        else
        { // Rotate x axis back upright 
            if (transform.localEulerAngles.x > maxRotation && transform.localEulerAngles.x < 180.0f)
            {
                _rb.AddRelativeTorque(Vector3.left * rotationForce, ForceMode.Acceleration);
            }
            else if (transform.localEulerAngles.x < 360.0f - maxRotation && transform.localEulerAngles.x > 180.0f)
            {
                _rb.AddRelativeTorque(Vector3.right * rotationForce, ForceMode.Acceleration);
            }
        }

        // Roll
        if (Input.GetKey(KeyCode.Q))
        {
            _rb.AddRelativeTorque(Vector3.forward * rotationForce * 2, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.E))
        {
            _rb.AddRelativeTorque(Vector3.back * rotationForce * 2, ForceMode.Acceleration);
        }

        // Lift
        if (Input.GetKey(KeyCode.Z) && transform.position.y < maxHeight)
        {
            _rb.AddForce(Vector3.up * speed, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.C) && transform.position.y > minHeight)
        {
            _rb.AddForce(Vector3.down * speed, ForceMode.Acceleration);
        }

        // Rotate the spaceship with mouse movement
        // if (Mathf.Abs(mouseXInput) > 0.0f)
        // {
        //     _rb.AddRelativeTorque(Vector3.up * mouseXInput * rotationForce, ForceMode.Acceleration);
        // }
        // if (Mathf.Abs(mouseYInput) > 0.0f)
        // {
        //     _rb.AddRelativeTorque(Vector3.left * mouseYInput * rotationForce, ForceMode.Acceleration);
        // }

        // Constant upward force keeping the spaceship floating
        // if (cowAbduction.attachedObject == null)
            _rb.AddForce(Vector3.up * -Physics.gravity.y, ForceMode.Acceleration);
        // else
        //     _rb.AddForce(Vector3.up * -Physics.gravity.y * ((float)cowAbduction.numberOfJoints / 10.0f) * cowAbduction.attachedObject.GetComponent<Rigidbody>().mass , ForceMode.Acceleration);
    }
}
