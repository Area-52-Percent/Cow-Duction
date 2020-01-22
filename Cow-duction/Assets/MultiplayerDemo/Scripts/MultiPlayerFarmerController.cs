﻿using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransform))]
public class MultiPlayerFarmerController : NetworkBehaviour
{
    public CharacterController characterController;
    [Tooltip("An empty transform which the main camera will align with")]
    public Transform cameraTransform;
    [Tooltip("The transform of the farmer's gun")]
    public Transform gunTransform;

    [Header("Parameters")]
    public float moveSpeed = 8f;
    public float turnSensitivity = 5f;
    public float tiltSensitivity = 2f;
    public float touchSensitivity = 2f;
    public bool invertY = false;

    [Header("Touch Diagnostics")]
    public float touchX = 0f;
    public float touchY = 0f;
    public Vector2 positionTouchBegan;
    public double timeTouchBegan = 0f;
    public double timeTouchEnded = 0f;
    public float tapTime = 0.25f;
    public bool touchMove = false;
    public bool touchAim = false;
    public Touch touch;

    [Header("Diagnostics")]
    public float mouseX = 0f;
    public float mouseY = 0f;
    public float horizontal = 0f;
    public float vertical = 0f;
    public float turn = 0f;
    public float tilt = 0f;
    public float jumpSpeed = 0f;
    public bool isGrounded = true;
    public bool isFalling = false;
    public bool isCursorLocked = false;
    public Vector3 velocity;
  
    private void OnValidate()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }

    // OnStartLocalPlayer is called when the local player object is set up
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = cameraTransform.localPosition;
        Camera.main.transform.localEulerAngles = cameraTransform.localEulerAngles;
        gunTransform.SetParent(Camera.main.transform);

        LockCursor(true);
    }

    // OnDisable is called when the object is removed from the server
    private void OnDisable()
    {
        if (isLocalPlayer)
        {
            gunTransform.SetParent(transform);
            Camera.main.transform.SetParent(null);
            Camera.main.transform.localPosition = new Vector3(0f, 10f, -10f);
            Camera.main.transform.localEulerAngles = Vector3.zero;

            LockCursor(false);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    timeTouchBegan = NetworkTime.time;
                    positionTouchBegan = touch.position;
                    if (touch.position.x < Screen.width) // Left = Move
                        touchMove = true;
                    else // Right = Aim
                        touchAim = true;
                    break;
                case TouchPhase.Moved:
                    if (touchMove) // Moving
                    {
                        touchX = touch.position.x - positionTouchBegan.x;
                        touchY = touch.position.y - positionTouchBegan.y;
                    }
                    else // Aiming
                    {
                        touchX = touch.deltaPosition.x;
                        touchY = touch.deltaPosition.y;
                    }
                    break;
                case TouchPhase.Ended:
                    timeTouchEnded = NetworkTime.time;
                    if (timeTouchEnded - timeTouchBegan <= tapTime) // Tap to fire
                        Debug.Log("Fire Gun");
                    touchMove = false;
                    touchAim = false;
                    break;
            }
            if (touchAim)
            {
                turn += touchX * touchSensitivity;
                tilt += (invertY ? touchY : -touchY) * touchSensitivity;
            }
            if (touchMove)
            {
                horizontal = Mathf.Clamp(touchX, -1f, 1f);
                vertical = Mathf.Clamp(-touchY, -1f, 1f);
            }
        }
        else // Keyboard input
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            turn += mouseX * turnSensitivity;
            tilt += (invertY ? mouseY : -mouseY) * tiltSensitivity;
        }

        tilt = Mathf.Clamp(tilt, -90f, 90f);

        if (isGrounded)
            isFalling = false;

        if ((isGrounded || !isFalling) && jumpSpeed < 1f && Input.GetKey(KeyCode.Space))
            jumpSpeed = Mathf.Lerp(jumpSpeed, 1f, 0.5f);
        else if (!isGrounded)
        {
            isFalling = true;
            jumpSpeed = 0;
        }

        if (Input.GetButtonDown("Cancel"))
        {
            if (isCursorLocked)
                LockCursor(false);
            else
                LockCursor(true);
        }
    }

    // FixedUpdate is called in fixed time intervals
    private void FixedUpdate()
    {
        if (!isLocalPlayer || characterController == null) return;

        transform.rotation = Quaternion.Euler(0f, turn, 0f);
        Camera.main.transform.localRotation = Quaternion.Euler(tilt, 0f, 0f);

        Vector3 direction = new Vector3(horizontal, jumpSpeed, vertical);
        direction = Vector3.ClampMagnitude(direction, 1f);
        direction = transform.TransformDirection(direction);
        direction *= moveSpeed;

        if (jumpSpeed > 0)
            characterController.Move(direction * Time.fixedDeltaTime);
        else
            characterController.SimpleMove(direction);

        isGrounded = characterController.isGrounded;
        velocity = characterController.velocity;
    }

    // Lock or unlock the cursor
    private void LockCursor(bool lockCursor)
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            isCursorLocked = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            isCursorLocked = false;
        }
    }
}
