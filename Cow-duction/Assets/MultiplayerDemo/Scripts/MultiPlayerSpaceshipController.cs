using UnityEngine;
using Mirror;

public class MultiPlayerSpaceshipController : NetworkBehaviour
{
    Rigidbody rb;
    const float MAXANGLE = 360;

    public Transform cameraTransform;

    [Header("Parameters")]
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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        // Parent main camera to spaceship
        Camera.main.orthographic = false;
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = cameraTransform.localPosition;
        Camera.main.transform.localEulerAngles = cameraTransform.localEulerAngles;
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

    [Header("Diagnostics")]
    [SerializeField] float horizontal;     // Controller: (LeftJoystick X-axis)  Keyboard: (D)(A)
    [SerializeField] float vertical;       // Controller: (LeftJoystick Y-axis)  Keyboard: (W)(S)
    [SerializeField] float turnHorizontal; // Controller: (RightJoystick X-axis) Keyboard: (Right)(Left)
    [SerializeField] float turnVertical;   // Controller: (RightJoystick Y-axis) Keyboard: (Up)(Down)
    [SerializeField] float roll;           // Controller: (RT)(LT)               Keyboard: (E)(Q)
    [SerializeField] float lift;           // Controller: (RB)(LB)               Keyboard: (Z)(C)

    void Update()
    {
        if (!isLocalPlayer) return;

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        turnHorizontal = Input.GetAxis("TurnHorizontal");
        turnVertical = Input.GetAxis("TurnVertical");
        roll = Input.GetAxis("Roll");
        lift = Input.GetAxis("Lift");
    }

    void FixedUpdate()
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
        if (Mathf.Abs(lift) > 0f)
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

    void LateUpdate()
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

    void Move(Vector3 direction)
    {
        rb.AddForce(direction, ForceMode.VelocityChange);
    }

    public void AddImpulseForce(Vector3 dir, float force)
    {
        rb.AddRelativeForce(dir * force * rb.mass, ForceMode.Impulse);
    }
    
    public void SetMovementMultiplier(float multiplier)
    {
        movementMultiplier = multiplier;
    }

    void ClampRotation(float max)
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
