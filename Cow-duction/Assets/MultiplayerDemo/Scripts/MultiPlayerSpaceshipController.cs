using UnityEngine;
using Mirror;

public class MultiPlayerSpaceshipController : NetworkBehaviour
{
    private Rigidbody rb;
    private const float MAXANGLE = 360;

    public Transform cameraTransform;

    [Header("Parameters")]
    public float maxHeight = 50f;
    public float moveSpeed = 20f;
    public float movementMultiplier = 1f;
    public float rotateSpeed = 1f;
    public float rotateMovingSpeed = .5f;
    public float rotateCorrectionSpeed = 1.5f;
    [Range(0f, 360f)]
    public float maxRotation = 30f;
    [Range(0f, 360f)]
    public float maxMoveRotation = 10f;
    public bool invertY = true;

    [Header("Diagnostics")]
    public float horizontal;     // Controller: (LeftJoystick X-axis)  Keyboard: (D)(A)
    public float vertical;       // Controller: (LeftJoystick Y-axis)  Keyboard: (W)(S)
    public float turnHorizontal; // Controller: (RightJoystick X-axis) Keyboard: (Right)(Left)
    public float turnVertical;   // Controller: (RightJoystick Y-axis) Keyboard: (Up)(Down)
    public float roll;           // Controller: (RT)(LT)               Keyboard: (E)(Q)
    public float lift;           // Controller: (RB)(LB)               Keyboard: (Z)(C)

    // OnStartLocalPlayer is called when the local player object is set up
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        // Parent main camera to spaceship
        Camera.main.orthographic = false;
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = cameraTransform.localPosition;
        Camera.main.transform.localEulerAngles = cameraTransform.localEulerAngles;
    }

    // OnDisable is called when the object is removed from the server
    private void OnDisable()
    {
        if (isLocalPlayer)
        {
            // Return main camera to starting position
            Camera.main.transform.SetParent(null);
            Camera.main.transform.localPosition = new Vector3(0f, 10f, -10f);
            Camera.main.transform.localEulerAngles = Vector3.zero;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isLocalPlayer) return;

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        turnHorizontal = Input.GetAxis("TurnHorizontal");
        turnVertical = Input.GetAxis("TurnVertical");
        roll = Input.GetAxis("Roll");
        lift = Input.GetAxis("Lift");
    }

    // FixedUpdate is called in fixed time intervals
    private void FixedUpdate()
    {
        // Anti-gravity force
        rb.AddForce(-Physics.gravity.y * Vector3.up * rb.mass);

        if (!isLocalPlayer) return;

        // Positional movement
        if (Mathf.Abs(horizontal) > 0f)
        {
            Vector3 direction = new Vector3(transform.right.x, 0, transform.right.z).normalized;
            Move(direction * horizontal * moveSpeed * Time.fixedDeltaTime);
            rb.AddRelativeTorque(Vector3.back * horizontal * rotateMovingSpeed, ForceMode.Acceleration);
        }
        if (Mathf.Abs(vertical) > 0f)
        {
            Vector3 direction = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
            Move(direction * vertical * moveSpeed * Time.fixedDeltaTime);
            rb.AddRelativeTorque(Vector3.right * vertical * rotateMovingSpeed, ForceMode.Acceleration);
        }
        if (Mathf.Abs(lift) > 0f && transform.position.y < maxHeight)
        {
            Move(transform.up * lift * moveSpeed * Time.fixedDeltaTime);
        }

        // Rotational movement
        if (Mathf.Abs(turnHorizontal) > 0f)
        {
            rb.AddRelativeTorque(Vector3.up * turnHorizontal * rotateSpeed, ForceMode.Acceleration);
        }
        if (Mathf.Abs(turnVertical) > 0f)
        {
            rb.AddRelativeTorque((invertY ? Vector3.right : Vector3.left) * turnVertical * rotateSpeed, ForceMode.Acceleration);
        }
        if (Mathf.Abs(roll) > 0f)
        {
            rb.AddRelativeTorque(Vector3.back * roll * rotateSpeed, ForceMode.Acceleration);
        }
    }

    // LateUpdate is called after each Update call
    private void LateUpdate()
    {
        if (Mathf.Abs(horizontal) > 0f || Mathf.Abs(vertical) > 0f)
        {
            ClampRotation(maxMoveRotation);
        }
        else
        {
            ClampRotation(maxRotation);
        }
    }

    // Apply a velocity change in the specified direction
    public void Move(Vector3 direction)
    {
        rb.AddForce(direction * movementMultiplier, ForceMode.VelocityChange);
    }

    // Apply an instant force on the rigidbody with specified direction and magnitude
    public void AddImpulseForce(Vector3 dir, float force)
    {
        rb.AddRelativeForce(dir * force * rb.mass, ForceMode.Impulse);
    }

    // Set the multiplier affecting the magnitude of the velocity change in Move
    public void SetMovementMultiplier(float multiplier)
    {
        movementMultiplier = multiplier;
    }

    // Clamp the transform rotation by applying a relative torque on the rigidbody
    private void ClampRotation(float max)
    {
        float correctionMultiplier = 1f;
        if (Mathf.Sign(horizontal) == Mathf.Sign(roll) ||
            Mathf.Sign(vertical) == Mathf.Sign(turnVertical))
        {
            correctionMultiplier = 2f;
        }
        
        if (transform.localEulerAngles.x > MAXANGLE / 2)
        {
            if (transform.localEulerAngles.x < MAXANGLE - max)
            {
                rb.AddRelativeTorque(Vector3.right * rotateCorrectionSpeed * correctionMultiplier, ForceMode.Acceleration);
            }
        }
        else
        {
            if (transform.localEulerAngles.x > max)
            {
                rb.AddRelativeTorque(-Vector3.right * rotateCorrectionSpeed * correctionMultiplier, ForceMode.Acceleration);
            }
        }

        if (transform.localEulerAngles.z > MAXANGLE / 2)
        {
            if (transform.localEulerAngles.z < MAXANGLE - max)
            {
                rb.AddRelativeTorque(-Vector3.back * rotateCorrectionSpeed * correctionMultiplier, ForceMode.Acceleration);
            }
        }
        else
        {
            if (transform.localEulerAngles.z > max)
            {
                rb.AddRelativeTorque(Vector3.back * rotateCorrectionSpeed * correctionMultiplier, ForceMode.Acceleration);
            }
        }
    }
}
