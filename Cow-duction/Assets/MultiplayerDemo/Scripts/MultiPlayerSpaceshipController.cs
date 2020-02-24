using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkTransform))]
public class MultiPlayerSpaceshipController : NetworkBehaviour
{
    private Rigidbody rb;
    private const float MAXANGLE = 360;

    [Header("Parameters")]
    public float maxHeight = 50f;
    public float moveSpeed = 30f;
    public float movementMultiplier = 1f;
    public float rotateSpeed = 30f; // In degrees per second
    [Range(0, 360)]
    public float maxRotation = 30f;
    [Range(0, 360)]
    public float maxMoveRotation = 15f;
    public bool invertY = true;

    [Header("Diagnostics")]
    [Range(-1, 1)]
    public float horizontal; // Controller: (L-Joystick X-axis) Keyboard: (D)(A)
    [Range(-1, 1)]
    public float vertical;   // Controller: (L-Joystick Y-axis) Keyboard: (W)(S)
    [Range(-1, 1)]
    public float yaw;        // Controller: (R-Joystick X-axis) Keyboard: (Right)(Left)
    [Range(-1, 1)]
    public float pitch;      // Controller: (R-Joystick Y-axis) Keyboard: (Up)(Down)
    [Range(-1, 1)]
    public float roll;       // Controller: (RT)(LT)            Keyboard: (E)(Q)
    [Range(-1, 1)]
    public float lift;       // Controller: (RB)(LB)            Keyboard: (Z)(C)

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rotateSpeed *= Mathf.Deg2Rad;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isLocalPlayer) return;

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        yaw = Input.GetAxis("TurnHorizontal");
        pitch = Input.GetAxis("TurnVertical");
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
            Move(direction * horizontal);
            Rotate(Vector3.back * horizontal);
        }
        if (Mathf.Abs(vertical) > 0f)
        {
            Vector3 direction = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
            Move(direction * vertical);
            Rotate(Vector3.right * vertical);
        }
        if (Mathf.Abs(lift) > 0f)
        {
            if (!(lift > 0 && transform.position.y >= maxHeight))
            {
                Move(transform.up * lift);
            }
        }

        // Rotational movement
        if (Mathf.Abs(yaw) > 0f)
        {
            Rotate(Vector3.up * yaw);
        }
        if (Mathf.Abs(pitch) > 0f)
        {
            Rotate((invertY ? Vector3.right : Vector3.left) * pitch);
        }
        if (Mathf.Abs(roll) > 0f)
        {
            Rotate(Vector3.back * roll);
        }

        // Clamp rotation based on whether spaceship is moving or not
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
    public void Move(Vector3 dir)
    {
        rb.AddForce(dir * moveSpeed * movementMultiplier * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }

    // Apply a relative torque in the specified direction
    private void Rotate(Vector3 torque)
    {
        rb.AddRelativeTorque(torque * rotateSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }

    // Apply an instant force on the rigidbody with specified direction and magnitude
    public void AddImpulseForce(Vector3 dir, float force)
    {
        rb.AddRelativeForce(dir * force * rb.mass, ForceMode.Impulse);
    }

    // Set the multiplier affecting the magnitude of the velocity change in Move
    public void SetMovementMultiplier(float multiplier = 1f)
    {
        movementMultiplier = multiplier;
    }

    // Clamp the transform rotation by applying a relative torque on the rigidbody
    private void ClampRotation(float max)
    {
        float correctionMultiplier = 1f;
        // Double multiplier if input applies double rotation in the same direction
        if (Mathf.Abs(horizontal) > 0 && Mathf.Abs(roll) > 0 && Mathf.Sign(horizontal) == Mathf.Sign(roll) ||
            Mathf.Abs(vertical) > 0 && Mathf.Abs(pitch) > 0 && Mathf.Sign(vertical) == Mathf.Sign(invertY ? pitch : -pitch))
        {
            correctionMultiplier *= 2f;
        }
        
        // Correct pitch
        if (transform.localEulerAngles.x > MAXANGLE / 2)
        {
            if (transform.localEulerAngles.x < MAXANGLE - max)
            {
                Rotate(Vector3.right * correctionMultiplier);
            }
        }
        else
        {
            if (transform.localEulerAngles.x > max)
            {
                Rotate(-Vector3.right * correctionMultiplier);
            }
        }
        // Correct roll
        if (transform.localEulerAngles.z > MAXANGLE / 2)
        {
            if (transform.localEulerAngles.z < MAXANGLE - max)
            {
                Rotate(-Vector3.back * correctionMultiplier);
            }
        }
        else
        {
            if (transform.localEulerAngles.z > max)
            {
                Rotate(Vector3.back * correctionMultiplier);
            }
        }
    }
}
