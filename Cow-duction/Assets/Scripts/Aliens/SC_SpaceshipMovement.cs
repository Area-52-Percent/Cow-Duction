/* SC_SpaceshipMovement.cs

   Physics based movement scheme simulating spaceship flight.
   
   Assumptions:
     This component is attached to a GameObject with a Rigidbody component.
 */

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SC_SpaceshipMovement : MonoBehaviour
{
    // Private variables
    private GameManager gameManager;
    private Rigidbody _rb;
    private float movementMultiplier = 1f;
    private bool movementEnabled;
    private bool grounded;

    // Public variables
    public float horizontalSpeed = 10.0f;
    public float groundSpeed = 1.0f;
    public float verticalSpeed = 10.0f;
    public float maxHeight = 50.0f;
    public float minHeight = 10.0f;
    public float hoverHeight = 5.0f;
    public float rotationForce = 0.25f;
    public float autoRotationForce = 0.05f;
    public float maxRotation = 20.0f;
    public bool invertLook = false;
    [SerializeField] private AudioClip engine = null; // Set up in inspector

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        _rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Confined;
        movementEnabled = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!gameManager.GetGameStarted())
        {
            _rb.AddForce(Vector3.up * -Physics.gravity.y, ForceMode.Acceleration);
            return;
        }

        Vector3 horizontalForce = Vector3.zero;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float turnHorizontalInput = Input.GetAxis("TurnHorizontal");
        float turnVerticalInput = Input.GetAxis("TurnVertical");
        float rollInput = Input.GetAxis("Roll");
        float liftInput = Input.GetAxis("Lift");

        // Move left and right
        if (Mathf.Abs(horizontalInput) > 0.0f)
        {
            horizontalForce = transform.right;
            horizontalForce.y = 0;

            if (movementEnabled)
            {
                _rb.AddForce(horizontalForce.normalized * horizontalInput * horizontalSpeed * movementMultiplier, ForceMode.Acceleration);

                // Rotate towards the direction of motion
                if ((horizontalInput < 0 && (transform.eulerAngles.z < maxRotation || transform.eulerAngles.z > 360.0f - maxRotation)) || 
                    (horizontalInput > 0 && (transform.eulerAngles.z > 360.0f - maxRotation || transform.eulerAngles.z < maxRotation)))
                {
                    _rb.AddRelativeTorque(Vector3.back * horizontalInput * autoRotationForce, ForceMode.Acceleration);
                }
            }
            else if (grounded)
                _rb.AddForce(horizontalForce.normalized * horizontalInput * groundSpeed * movementMultiplier, ForceMode.Acceleration);
        }

        // Move forward and backward
        if (Mathf.Abs(verticalInput) > 0.0f)
        {
            horizontalForce = transform.forward;
            horizontalForce.y = 0;

            if (movementEnabled)
            {
                _rb.AddForce(horizontalForce.normalized * verticalInput * horizontalSpeed * movementMultiplier, ForceMode.Acceleration);
                
                // Rotate towards the direction of motion
                if ((verticalInput > 0 && (transform.localEulerAngles.x < maxRotation || transform.localEulerAngles.x > 360.0f - maxRotation)) || 
                    (verticalInput < 0 && (transform.localEulerAngles.x > 360.0f - maxRotation || transform.localEulerAngles.x < maxRotation)))
                {
                    _rb.AddRelativeTorque(Vector3.right * verticalInput * autoRotationForce, ForceMode.Acceleration);
                }
            }
            else if (grounded)
                _rb.AddForce(horizontalForce.normalized * verticalInput * groundSpeed * movementMultiplier, ForceMode.Acceleration);
        }

        // Turn left and right
        if (Mathf.Abs(turnHorizontalInput) > 0.0f)
        {
            _rb.AddRelativeTorque(Vector3.up * turnHorizontalInput * rotationForce, ForceMode.Acceleration);        
        }

        // Tilt forward and backward
        if ((turnVerticalInput < 0.0f && (transform.localEulerAngles.x < maxRotation || transform.localEulerAngles.x > 270.0f)) || 
            (turnVerticalInput > 0.0f && (transform.localEulerAngles.x > 360.0f - maxRotation || transform.localEulerAngles.x < 90.0f)))
        {
            _rb.AddRelativeTorque((invertLook ? Vector3.right : Vector3.left) * turnVerticalInput * rotationForce, ForceMode.Acceleration);
        }
        // Rotate x axis back upright
        else
        {
            if (transform.localEulerAngles.x > maxRotation && transform.localEulerAngles.x < 180.0f)
            {
                _rb.AddRelativeTorque(Vector3.left * rotationForce, ForceMode.Acceleration);
            }
            else if (transform.localEulerAngles.x < 360.0f - maxRotation && transform.localEulerAngles.x > 180.0f)
            {
                _rb.AddRelativeTorque(Vector3.right * rotationForce, ForceMode.Acceleration);
            }
        }

        // Roll left and right
        if ((rollInput < 0.0f && (transform.localEulerAngles.z < maxRotation || transform.localEulerAngles.z > 270.0f)) || 
            (rollInput > 0.0f && (transform.localEulerAngles.z > 360.0f - maxRotation || transform.localEulerAngles.z < 90.0f)))
        {
            _rb.AddRelativeTorque(Vector3.back * rollInput * rotationForce, ForceMode.Acceleration);
        }
        // Rotate z axis back upright
        else
        {
            if (transform.localEulerAngles.z > maxRotation && transform.localEulerAngles.z < 180.0f)
            {
                _rb.AddRelativeTorque(Vector3.back * autoRotationForce, ForceMode.Acceleration);
            }
            else if (transform.localEulerAngles.z < 360.0f - maxRotation && transform.localEulerAngles.z > 180.0f)
            {
                _rb.AddRelativeTorque(Vector3.forward * autoRotationForce, ForceMode.Acceleration);
            }
        }

        // Hover if detecting object too close underneath
        Debug.DrawRay(transform.position, Vector3.down * hoverHeight, Color.cyan);
        int ufoLayer = ~(1 << (LayerMask.NameToLayer("UFO")));
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hoverHit, hoverHeight, ufoLayer, QueryTriggerInteraction.Ignore))
        {
            _rb.AddForce(Vector3.up * verticalSpeed * movementMultiplier, ForceMode.Acceleration);
            if (_rb.drag < 1f)
                _rb.drag = 1f;
        }

        // Disable all controls past this if movement not enabled
        if (!movementEnabled)
        {
            grounded = true;
            return;
        }
        else if (grounded)
        {
            grounded = false;
        }

        // Lift
        if ((liftInput > 0.0f && transform.position.y < maxHeight) || 
            (liftInput < 0.0f && transform.position.y > minHeight))
        {
            _rb.AddForce(Vector3.up * liftInput * verticalSpeed * movementMultiplier, ForceMode.Acceleration);
        }

        // Constant upward force keeping the spaceship floating
        _rb.AddForce(Vector3.up * -Physics.gravity.y, ForceMode.Acceleration);
    }

    // Toggles invert look for camera up and down rotation
    public void ToggleInvertLook()
    {
        if (invertLook)
            invertLook = false;
        else
            invertLook = true;
    }

    // Apply a force on the spaceship for physical feedback
    public void AddImpulseForce(Vector3 dir, float force)
    {
        _rb.AddRelativeForce(dir * force * _rb.mass, ForceMode.Impulse);
    }
    
    // Set movement multiplier (should be between 0 and 1)
    public void SetMovementMultiplier(float multiplier)
    {
        movementMultiplier = multiplier;
    }

    // Disable translational movement
    public void AllowMovement(bool allow)
    {
        if (allow)
        {
            movementEnabled = true;
            _rb.drag = 1f;
        }
        else
        {
            movementEnabled = false;
            _rb.drag = 0f;
        }
    }

    // Reset spaceship to starting position and enable movement
    public void ResetGame()
    {
        _rb.MovePosition(new Vector3(65f, 40f, -70f));
        _rb.velocity = Vector3.zero;
        _rb.MoveRotation(Quaternion.identity);
        AllowMovement(true);
    }
}
