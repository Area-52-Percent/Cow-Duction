/* CowAbduction.cs

   Grappling hook system that shoots a raycast from the Main Camera in the direction of the mouse position.
   The grappling hook attaches to Cows and Farmers with a line of Configurable Joints.
   The length of the hook rope can be increased or decreased to pull or push the attached object.
   
   Assumptions:
     This component belongs to a GameObject with Rigidbody and Collider (set as trigger) components.
     There is a GameObject in the scene with the "UIManager" tag and a UIManager component.
     There is a Shader file located at "./Sprites/Default".
     GameObjects that interact with the grappling hook have the "Cow" or "Farmer" tag, Collider and Rigidbody components.
 */

using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class CowAbduction : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    public Rigidbody beamOrigin;
    [SerializeField] private UIManager uiManager;
    // Joint parameters
    public float maxCaptureLength = 50.0f;
    public int numberOfJoints = 3;
    public float captureSpeed = 5.0f;
    [SerializeField] private float captureLength;
    [SerializeField] private ConfigurableJoint[] attachedObjectJoints;    
    public GameObject attachedObject;
    // Grapple parameters
    public float grappleTime = 0.5f;
    [SerializeField] private bool grappling;
    public GameObject probe;
    [SerializeField] GameObject probeClone;
    // Line parameters
    [SerializeField] private LineRenderer lineRenderer;

    // Awake is called after all objects are initialized
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Create a line renderer
        if (!lineRenderer)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.widthMultiplier = 0.5f;
        }
        lineRenderer.enabled = false;

        attachedObjectJoints = new ConfigurableJoint[numberOfJoints];
        grappling = false;
    }

    // FixedUpdate is called in fixed intervals
    private void FixedUpdate()
    {
        // Left click casts a raycast in the direction of the cursor position.
        // If a cow is already attached, release it.
        // Otherwise if the raycast hits a cow, the cow becomes attached to my rigidbody through configurable joints.
        if (Input.GetMouseButtonDown(0) && Time.timeScale > 0f)
        {
            // Do not shoot ray if cow is already attached
            if (attachedObject == null && !grappling)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;                    
                
                if (Physics.Raycast(ray, out hit, maxCaptureLength))
                {
                    captureLength = Vector3.Distance(transform.position, hit.transform.position);

                    StartCoroutine(ShootGrapple(hit));                        
                }
            }
        }

        // Release the attached object
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (attachedObject != null)
            {
                if (attachedObject.GetComponent<NavMeshAgent>())
                    attachedObject.GetComponent<NavMeshAgent>().enabled = true;

                attachedObject.GetComponent<Rigidbody>().drag = 1.0f;

                foreach (ConfigurableJoint cj in attachedObjectJoints)
                {
                    if (cj != null)
                    {
                        if (cj.tag == "Cow" || cj.tag == "Farmer")
                            Destroy(cj);
                        else
                            Destroy(cj.gameObject);
                    }
                }

                if (lineRenderer.enabled)
                    lineRenderer.enabled = false;
                
                if (probeClone != null)
                    Destroy(probeClone);

                attachedObject = null;

                // Reset carry mass
                _rb.GetComponent<SpaceshipMovement>().ResetCarryMass();
            }
        }

        // Holding comma pulls the attached cow towards the UFO
        if (Input.GetKey(KeyCode.Comma)) 
        {
            if (attachedObject != null && attachedObject.tag == "Cow") 
            {
                if (captureLength > 0f)
                {
                    // Decrease joint limits over time
                    captureLength -= Time.deltaTime * captureSpeed;
                    SoftJointLimit softJointLimit = new SoftJointLimit();
                    softJointLimit.limit = captureLength / (float)numberOfJoints;
                    softJointLimit.contactDistance = 0.1f;
                    foreach(ConfigurableJoint cj in attachedObjectJoints)
                        cj.linearLimit = softJointLimit;
                }
                else
                {
                    // Force joint limits to zero
                    foreach(ConfigurableJoint cj in attachedObjectJoints)
                    {
                        SoftJointLimit softJointLimit = new SoftJointLimit();
                        softJointLimit.limit = 0f;
                        softJointLimit.contactDistance = 0.1f;
                        cj.linearLimit = softJointLimit;
                    }
                }
            }
        }

        // Holding period pushes the attached cow away from the UFO
        if (Input.GetKey(KeyCode.Period)) 
        {
            if (attachedObject != null && attachedObject.tag == "Cow") 
            {
                if (captureLength < maxCaptureLength)
                {
                    // Increase joint limits over time
                    captureLength += Time.deltaTime * captureSpeed;
                    SoftJointLimit softJointLimit = new SoftJointLimit();
                    softJointLimit.limit = captureLength / (float)numberOfJoints;
                    softJointLimit.contactDistance = 0.1f;
                    foreach(ConfigurableJoint cj in attachedObjectJoints)
                        cj.linearLimit = softJointLimit;
                }
            }
        }

        // Draw a line between me and the attached object
        if (attachedObject != null && lineRenderer != null)
        {
            RenderLine(attachedObject);

            Rigidbody attachedRigidboy = attachedObject.GetComponent<Rigidbody>();
            if (attachedRigidboy.velocity.magnitude > 1.0f)
                attachedRigidboy.AddForce(-attachedRigidboy.velocity, ForceMode.Acceleration);
        }
    }

    // Render a line to the object including any in-between joints
    private void RenderLine(GameObject obj)
    {
        if (attachedObject != null)
        {
            // Set up points along each joint
            var points = new Vector3[numberOfJoints + 1];
            points[0] = transform.position - transform.up;
            for (int j = 0; j < numberOfJoints; j++)
            {
                points[j + 1] = attachedObjectJoints[j].transform.position;
            }
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
        }
        else
        {
            // Set up two points for a straight line
            var points = new Vector3[2];
            points[0] = transform.position - transform.up;
            points[1] = obj.transform.position;
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
        }
        if (!lineRenderer.enabled)
            lineRenderer.enabled = true;
    }

    // Play a grappling animation
    private IEnumerator ShootGrapple(RaycastHit hit)
    {
        if (probe != null)
            probeClone = Instantiate(probe, transform.position, Quaternion.identity);
        
        grappling = true;
        // Extend the grapple
        float counter = 0.0f;
        while (counter < grappleTime)
        {            
            counter += Time.fixedDeltaTime / grappleTime;
            probeClone.transform.position = Vector3.Lerp(transform.position - transform.up, hit.point, counter / grappleTime);
            RenderLine(probeClone);
            yield return null;
        }
               
        if (!AttachBody(hit))
        {
            // Retract the grapple if it did not attach to a cow or farmer
            counter = 0.0f;
            while (counter < grappleTime)
            {            
                counter += Time.fixedDeltaTime / grappleTime;
                probeClone.transform.position = Vector3.Lerp(hit.point, transform.position - transform.up, counter / grappleTime);
                RenderLine(probeClone);
                yield return null;                
            }
            Destroy(probeClone);
        }
        grappling = false;
    }

    // Attach to an object through a series of joints
    private bool AttachBody(RaycastHit hit)
    {
        Debug.Log("Ray cast hit " + hit.transform.gameObject.name);
        if ((hit.collider.tag == "Cow" || hit.collider.tag == "Farmer") && hit.rigidbody != null)
        {
            // Disable AI while attached
            if (hit.transform.gameObject.GetComponent<NavMeshAgent>())
            {
                hit.transform.gameObject.GetComponent<NavMeshAgent>().enabled = false;
            }
            // Delete any existing joint
            if (hit.transform.gameObject.GetComponent<ConfigurableJoint>())
            {
                Destroy(hit.transform.gameObject.GetComponent<ConfigurableJoint>());
            }
            // Set up an array of joints simulating a rope
            attachedObjectJoints = new ConfigurableJoint[numberOfJoints];
            for (int j = 0; j < numberOfJoints; j++)
            {
                // The last joint connects the cow to the joint chain
                if (j == numberOfJoints - 1)
                    attachedObjectJoints[j] = hit.transform.gameObject.AddComponent<ConfigurableJoint>();
                else
                {
                    GameObject go = new GameObject("Joint");
                    Rigidbody goRigidbody = go.AddComponent<Rigidbody>();

                    goRigidbody.mass = _rb.mass / 10.0f;
                    goRigidbody.drag = 1.0f;
                    goRigidbody.angularDrag = 1.0f;

                    // Add mass to spaceship
                    _rb.GetComponent<SpaceshipMovement>().AddCarryMass(goRigidbody.mass);

                    go.transform.position = (j + 1) * (transform.position - hit.transform.position) / (float)numberOfJoints;

                    attachedObjectJoints[j] = go.AddComponent<ConfigurableJoint>();
                }
                // Manually configure joint anchor
                attachedObjectJoints[j].autoConfigureConnectedAnchor = false;
                attachedObjectJoints[j].connectedAnchor = Vector3.zero;
                attachedObjectJoints[j].projectionMode = JointProjectionMode.PositionAndRotation;
                // Limit joint motion
                attachedObjectJoints[j].xMotion = ConfigurableJointMotion.Limited;
                attachedObjectJoints[j].yMotion = ConfigurableJointMotion.Limited;
                attachedObjectJoints[j].zMotion = ConfigurableJointMotion.Limited;
                // Set up joint linear limit
                SoftJointLimit softJointLimit = new SoftJointLimit();
                softJointLimit.limit = captureLength / (float)numberOfJoints;
                softJointLimit.contactDistance = 0.1f;
                attachedObjectJoints[j].linearLimit = softJointLimit;
                // The first joint is connected to the UFO
                if (j == 0)
                    attachedObjectJoints[j].connectedBody = beamOrigin;
                else
                    attachedObjectJoints[j].connectedBody = attachedObjectJoints[j - 1].gameObject.GetComponent<Rigidbody>();                
            }
            attachedObject = hit.transform.gameObject;

            // Add mass to spaceship
                _rb.GetComponent<SpaceshipMovement>().AddCarryMass(attachedObject.GetComponent<Rigidbody>().mass);

            if (probeClone != null)
            {
                probeClone.transform.parent = attachedObject.transform;
                probeClone.transform.position = attachedObject.transform.position;
            }
            return true;
        }
        return false;
    }

    // OnTriggerEnter is called when a collision with another collider is detected
    private void OnTriggerEnter(Collider col) 
    {
        if(col.gameObject.tag == "Cow" && attachedObject != null)
        {
            // Destroy grappling hook and cow
            Destroy(col.gameObject);
            if (probeClone != null)
                Destroy(probeClone);
            foreach(ConfigurableJoint cj in attachedObjectJoints)
            {
                if (cj.gameObject != null && cj.tag != "Cow")
                    Destroy(cj.gameObject);
            }            
            attachedObject = null;
            lineRenderer.enabled = false;
            // Call the UI manager to increase score
            if(uiManager == null)
                uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
            uiManager.IncreaseScore(1);
            // Apply small upward force for physical feedback
            _rb.AddRelativeForce(_rb.transform.up * _rb.mass, ForceMode.Impulse);
            // Reset carry mass
            _rb.GetComponent<SpaceshipMovement>().ResetCarryMass();
        }
    }

    // Reset parameters to starting values
    public void ResetGame() {
        Start();
    }
}
