using UnityEngine;

public class CowAbduction : MonoBehaviour
{
    private Rigidbody _rb;
    // Joint parameters
    public float maxCaptureLength = 50.0f;
    public float captureSpeed = 5.0f;
    [SerializeField] private float captureLength;
    [SerializeField] private ConfigurableJoint attachedObjectJoint;
    // Line parameters
    public LineRenderer lineRenderer;
    public Color lineStartColor = Color.green;
    public Color lineEndColor = Color.white;

    // Start is called before the first frame update
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        // Create a line renderer
        if (!lineRenderer)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.widthMultiplier = 1f;
            lineRenderer.positionCount = 2;
            lineRenderer.startColor = lineStartColor;
            lineRenderer.endColor = lineEndColor;
            lineRenderer.enabled = false;
        }
    }

    // FixedUpdate is called in fixed intervals
    private void FixedUpdate()
    {
        // Left click casts a raycast in the direction of the cursor position.
        // When it hits a cow, the cow becomes attached to my rigidbody through a configurable joint.
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            Ray preRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Vector3 rayOrigin = _rb.transform.position;
            Ray ray = new Ray(rayOrigin, preRay.direction);
            if (Physics.Raycast(ray, out hit, maxCaptureLength))
            {
                Debug.DrawLine(transform.position, hit.point, Color.yellow);
                Debug.Log("Ray cast hit " + hit.transform.gameObject.name);
                if (hit.collider.tag == "Cow")
                {
                    Debug.Log("Cow has been picked up");
                    if (hit.rigidbody != null) 
                    {
                        if (!hit.transform.gameObject.GetComponent<ConfigurableJoint>())
                        {
                            if (attachedObjectJoint != null) 
                            {
                                Destroy(attachedObjectJoint);
                            }                            
                            attachedObjectJoint = hit.transform.gameObject.AddComponent<ConfigurableJoint>();
                            attachedObjectJoint.connectedBody = _rb;
                            attachedObjectJoint.autoConfigureConnectedAnchor = false;
                            attachedObjectJoint.connectedAnchor = Vector3.zero;

                            attachedObjectJoint.xMotion = ConfigurableJointMotion.Limited;
                            attachedObjectJoint.yMotion = ConfigurableJointMotion.Limited;
                            attachedObjectJoint.zMotion = ConfigurableJointMotion.Limited;

                            SoftJointLimitSpring softJointLimitSpring = new SoftJointLimitSpring();
                            softJointLimitSpring.spring = 1.0f;
                            softJointLimitSpring.damper = 0.2f;
                            attachedObjectJoint.linearLimitSpring = softJointLimitSpring;

                            SoftJointLimit softJointLimit = new SoftJointLimit();
                            softJointLimit.limit = captureLength;
                            softJointLimit.contactDistance = 0.1f;
                            attachedObjectJoint.linearLimit = softJointLimit;

                            JointDrive xzJointDrive = new JointDrive();
                            xzJointDrive.positionSpring = 1f;
                            xzJointDrive.maximumForce = 1.0f;
                            attachedObjectJoint.xDrive = xzJointDrive;
                            attachedObjectJoint.zDrive = xzJointDrive;

                            captureLength = Vector3.Distance(transform.position, attachedObjectJoint.transform.position);
                        }
                    }                    
                }                
            }
        }
        // Releasing left click removes the joint from the currently attached object (if any).
        else if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
        {
            if (attachedObjectJoint)
            {
                Destroy(attachedObjectJoint);  
            }
            if (lineRenderer.enabled)
            {
                lineRenderer.enabled = false;
            }
        }

        // Holding left click applies a force on the currently attached object (if any), towards my position.
        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space)) 
        {
            if (attachedObjectJoint != null) 
            {
                // Add drive forces pushing the attached object towards my y position
                if (attachedObjectJoint.yDrive.positionSpring < 100.0f)
                {
                    JointDrive yJointDrive = new JointDrive();
                    yJointDrive.positionSpring = attachedObjectJoint.yDrive.positionSpring + (Time.deltaTime * captureSpeed * 10);
                    // yJointDrive.maximumForce = attachedObjectJoint.yDrive.maximumForce + (Time.deltaTime * captureSpeed);
                    attachedObjectJoint.yDrive = yJointDrive;
                }
                if (captureLength > 0)
                {
                    captureLength -= Time.deltaTime * captureSpeed;
                    SoftJointLimit softJointLimit = new SoftJointLimit();
                    softJointLimit.limit = captureLength;
                    softJointLimit.contactDistance = 0.1f;
                    attachedObjectJoint.linearLimit = softJointLimit;
                }
                attachedObjectJoint.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * captureSpeed);
                // Draw a line between me and the attached object
                if (lineRenderer != null)
                {
                    var points = new Vector3[2];
                    points[0] = transform.position + Vector3.down;
                    points[1] = attachedObjectJoint.transform.position;
                    lineRenderer.SetPositions(points);
                    if (!lineRenderer.enabled)
                        lineRenderer.enabled = true;
                }
            }
        }
    }

    // OnTriggerEnter is called when a collision with another collider is detected
    private void OnTriggerEnter(Collider col) 
    {
        if(col.gameObject.tag == "Cow")
        {
            Debug.Log("Collision with cow detected");
            Destroy(col.gameObject);
            lineRenderer.enabled = false;
        }
    }
}
