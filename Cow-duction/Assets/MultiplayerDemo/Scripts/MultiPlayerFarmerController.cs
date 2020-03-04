using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransform))]
public class MultiPlayerFarmerController : NetworkBehaviour
{
    public CharacterController characterController;
    public Animator animator;
    [Tooltip("UI camera which the main camera will align with")]
    public Transform cameraTransform;
    [Tooltip("The transform of the farmer's gun")]
    public Transform gunTransform;
    public GameObject projectile;
    public ParticleSystem gunsmoke;

    [Header("Parameters")]
    public float moveSpeed = 8f;
    public float turnSensitivity = 20f;
    public float tiltSensitivity = 16f;
    public float touchSensitivity = 5f;
    public bool invertY = false;

    [Header("Touch Diagnostics")]
    public float touchX = 0f;
    public float touchY = 0f;
    public Vector2 positionTouchBegan;
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
        if (animator == null)
            animator = GetComponent<NetworkAnimator>().animator;
    }

    // OnStartLocalPlayer is called when the local player object is set up
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        LockCursor(true);
    }

    // OnDisable is called when the object is removed from the server
    private void OnDisable()
    {
        if (isLocalPlayer)
        {
            LockCursor(false);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isLocalPlayer) return;

        int touchCount = Input.touchCount;

        if (touchCount > 0)
        {
            if (!Input.multiTouchEnabled) touchCount = 1;

            for (int t = 0; t < touchCount; t++)
            {
                touch = Input.GetTouch(t);

                if (touch.position.x < Screen.width / 2) // Moving
                {
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                                positionTouchBegan = touch.position;
                            break;
                        case TouchPhase.Moved:
                        case TouchPhase.Stationary:
                                touchX = (touch.position.x - positionTouchBegan.x) / 100f;
                                touchY = (touch.position.y - positionTouchBegan.y) / 100f;

                                horizontal = Mathf.Clamp(touchX, -1f, 1f);
                                vertical = Mathf.Clamp(touchY, -1f, 1f);
                            break;
                        case TouchPhase.Ended:
                                horizontal = 0;
                                vertical = 0;
                            break;
                    }
                }
                else // Aiming
                {
                    switch (touch.phase)
                    {
                        case TouchPhase.Moved:
                            touchX = touch.deltaPosition.x;
                            touchY = touch.deltaPosition.y;

                            turn += touchX * touchSensitivity * Time.deltaTime;
                            tilt += (invertY ? touchY : -touchY) * touchSensitivity * Time.deltaTime;
                            break;
                    }
                }
            }
        }
        else // Keyboard input
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            turn += mouseX * turnSensitivity * Time.deltaTime;
            tilt += (invertY ? mouseY : -mouseY) * tiltSensitivity * Time.deltaTime;

            if (Input.GetButtonDown("Fire1"))
            {
                CmdFireProjectile();
            }
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
        cameraTransform.localRotation = Quaternion.Euler(tilt, 0f, 0f);

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
        animator.SetFloat("speed", velocity.magnitude);
    }

    public void FireProjectile()
    {
        gunsmoke.Play();
    }

    [Command]
    public void CmdFireProjectile()
    {
        GameObject projectileClone = Instantiate(projectile, cameraTransform.position + cameraTransform.forward, cameraTransform.rotation);
        gunsmoke.Play();
        NetworkServer.Spawn(projectileClone);
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
