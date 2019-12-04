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
    private Rigidbody _rb;
    private float movementPenaltyFactor = 1f;
    private bool movementEnabled;
    private bool grounded;

    // Public variables
    public float horizontalSpeed = 10.0f;
    public float groundSpeed = 1.0f;
    public float verticalSpeed = 10.0f;
    public float maxHeight = 50.0f;
    public float minHeight = 10.0f;
    public float hoverHeight = 5.0f;
    public float rotationForce = 0.05f;
    public float maxRotation = 20.0f;
    public bool invertLook = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Confined;
        movementEnabled = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
                _rb.AddForce(horizontalForce.normalized * horizontalInput * horizontalSpeed * movementPenaltyFactor, ForceMode.Acceleration);

                // Rotate towards the direction of motion
                if ((horizontalInput < 0 && (transform.eulerAngles.z < maxRotation || transform.eulerAngles.z > 360.0f - maxRotation)) || 
                    (horizontalInput > 0 && (transform.eulerAngles.z > 360.0f - maxRotation || transform.eulerAngles.z < maxRotation)))
                {
                    _rb.AddRelativeTorque(Vector3.back * horizontalInput * rotationForce, ForceMode.Acceleration);
                }
            }
            else if (grounded)
                _rb.AddForce(horizontalForce.normalized * horizontalInput * groundSpeed * movementPenaltyFactor, ForceMode.Acceleration);
        }

        // Move forward and backward
        if (Mathf.Abs(verticalInput) > 0.0f)
        {
            horizontalForce = transform.forward;
            horizontalForce.y = 0;

            if (movementEnabled)
            {
                _rb.AddForce(horizontalForce.normalized * verticalInput * horizontalSpeed * movementPenaltyFactor, ForceMode.Acceleration);
                
                // Rotate towards the direction of motion
                if ((verticalInput > 0 && (transform.localEulerAngles.x < maxRotation || transform.localEulerAngles.x > 360.0f - maxRotation)) || 
                    (verticalInput < 0 && (transform.localEulerAngles.x > 360.0f - maxRotation || transform.localEulerAngles.x < maxRotation)))
                {
                    _rb.AddRelativeTorque(Vector3.right * verticalInput * rotationForce, ForceMode.Acceleration);
                }
            }
            else if (grounded)
                _rb.AddForce(horizontalForce.normalized * verticalInput * groundSpeed * movementPenaltyFactor, ForceMode.Acceleration);
        }

        // Turn left and right
        if (Mathf.Abs(turnHorizontalInput) > 0.0f)
        {
            _rb.AddRelativeTorque(Vector3.up * turnHorizontalInput * rotationForce * 2, ForceMode.Acceleration);        
        }        

        // Tilt forward and backward
        if (Mathf.Abs(turnVerticalInput) > 0.0f)
        {            
            _rb.AddRelativeTorque((invertLook ? Vector3.right : Vector3.left) * turnVerticalInput * rotationForce * 2, ForceMode.Acceleration);
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
            _rb.AddRelativeTorque(Vector3.back * rollInput * rotationForce * 2, ForceMode.Acceleration);
        }
        // Rotate z axis back upright
        else
        {
            if (transform.localEulerAngles.z > maxRotation && transform.localEulerAngles.z < 180.0f)
            {
                _rb.AddRelativeTorque(Vector3.back * rotationForce, ForceMode.Acceleration);
            }
            else if (transform.localEulerAngles.z < 360.0f - maxRotation && transform.localEulerAngles.z > 180.0f)
            {
                _rb.AddRelativeTorque(Vector3.forward * rotationForce, ForceMode.Acceleration);
            }
        }

        // Disable all controls past this if movement not enabled
        if (!movementEnabled)
        {
            // Check if grounded
            int layerMask = ~(1 << gameObject.layer);
            Debug.DrawRay(transform.position + (transform.up * 0.5f), -transform.up * 2.5f, Color.red);
            if (Physics.Raycast(transform.position + (transform.up * 0.5f), -transform.up, 2.5f, layerMask))
            {
                grounded = true;
            }
            else
            {
                grounded = false;
            }
            return;
        }

        // Lift
        if ((liftInput > 0.0f && transform.position.y < maxHeight) || 
            (liftInput < 0.0f && transform.position.y > minHeight))
        {
            _rb.AddForce(Vector3.up * liftInput * verticalSpeed * movementPenaltyFactor, ForceMode.Acceleration);
        }

        // Hover if detecting object too close underneath
        if (Physics.Raycast(transform.position, Vector3.down, hoverHeight))
        {
            _rb.AddForce(Vector3.up * verticalSpeed * movementPenaltyFactor, ForceMode.Acceleration);
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
    public void SetMovementPenaltyFactor(float factor)
    {
        movementPenaltyFactor = factor;
    }

    // Reset movement multiplier to 1
    public void ResetMovementPenaltyFactor()
    {
        movementPenaltyFactor = 1f;
    }

    // Disable translational movement
    public void AllowMovement(bool allow)
    {
        if (allow)
        {
            movementEnabled = true;
            _rb.drag = 1f;
            _rb.angularDrag = 1f;
        }
        else
        {
            movementEnabled = false;
            _rb.drag = 0f;
            _rb.angularDrag = 0.05f;
        }
    }

    // Reset spaceship to starting position and enable movement
    public void ResetGame()
    {
        _rb.MovePosition(Vector3.up * 5.0f);
        _rb.velocity = Vector3.zero;
        _rb.AddForce((Vector3.up * 5.0f) * _rb.mass, ForceMode.Impulse);
        _rb.MoveRotation(Quaternion.identity);
        AllowMovement(true);
    }
}
