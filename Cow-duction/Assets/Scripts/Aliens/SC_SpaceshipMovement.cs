/* SC_SpaceshipMovement.cs

   Physics based movement scheme simulating spaceship flight.
   
   Assumptions:
     This component is attached to a GameObject with a Rigidbody component.
 */

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SC_SpaceshipMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    public float horizontalSpeed = 10.0f;
    public float verticalSpeed = 10.0f;
    public float maxHeight = 50.0f;
    public float minHeight = 10.0f;
    public float hoverHeight = 5.0f;
    public float rotationForce = 0.05f;
    public float maxRotation = 20.0f;
    public bool invertLook = false;
    [SerializeField] private float movementPenaltyFactor = 1f;
    [SerializeField] private bool movementEnabled;

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
        if (movementEnabled && Mathf.Abs(horizontalInput) > 0.0f)
        {
            horizontalForce = transform.right;
            horizontalForce.y = 0;
            horizontalForce.Normalize();
            _rb.AddForce(horizontalForce * horizontalInput * horizontalSpeed * movementPenaltyFactor, ForceMode.Acceleration);

            if ((horizontalInput < 0 && (transform.eulerAngles.z < maxRotation || transform.eulerAngles.z > 360.0f - maxRotation)) || 
                (horizontalInput > 0 && (transform.eulerAngles.z > 360.0f - maxRotation || transform.eulerAngles.z < maxRotation)))
            {
                _rb.AddRelativeTorque(Vector3.back * horizontalInput * rotationForce, ForceMode.Acceleration);
            }
        }

        // Move forward and backward
        if (movementEnabled && Mathf.Abs(verticalInput) > 0.0f)
        {
            horizontalForce = transform.forward;
            horizontalForce.y = 0;
            horizontalForce.Normalize();
            _rb.AddForce(horizontalForce * verticalInput * horizontalSpeed * movementPenaltyFactor, ForceMode.Acceleration);

            if ((verticalInput > 0 && (transform.localEulerAngles.x < maxRotation || transform.localEulerAngles.x > 360.0f - maxRotation)) || 
                (verticalInput < 0 && (transform.localEulerAngles.x > 360.0f - maxRotation || transform.localEulerAngles.x < maxRotation)))
            {
                _rb.AddRelativeTorque(Vector3.right * verticalInput * rotationForce, ForceMode.Acceleration);
            }
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
        if (Mathf.Abs(rollInput) > 0.0f && (transform.localEulerAngles.z < maxRotation || transform.localEulerAngles.z > 360.0f - maxRotation))
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
            return;

        // Lift        
        if (Mathf.Abs(liftInput) > 0.0f && transform.position.y > minHeight && transform.position.y < maxHeight)
        {
            _rb.AddForce(Vector3.up * liftInput * verticalSpeed * movementPenaltyFactor, ForceMode.Acceleration);
        }

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
            movementEnabled = true;
        else
            movementEnabled = false;
    }

    // Reset spaceship to starting position and enable movement
    public void ResetGame()
    {
        _rb.MovePosition(Vector3.up * 10.0f);
        _rb.MoveRotation(Quaternion.identity);
        movementEnabled = true;
    }
}
