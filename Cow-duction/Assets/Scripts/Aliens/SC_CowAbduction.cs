/* SC_CowAbduction.cs

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

[RequireComponent(typeof(SC_SpaceshipMovement))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class SC_CowAbduction : MonoBehaviour
{
    private SC_SpaceshipMovement spaceshipMovement;
    public Rigidbody beamOrigin;
    private SC_AlienUIManager uiManager;
    // Joint parameters
    public float maxCaptureLength = 50.0f;
    public int numberOfJoints = 3;
    public float captureSpeed = 5.0f;
    private float captureLength;
    private ConfigurableJoint[] attachedObjectJoints;    
    public GameObject attachedObject;
    // Grapple parameters
    public float grappleTime = 0.5f;
    private bool grappling;
    [SerializeField] private GameObject probe = null; // Set up in inspector
    private GameObject probeClone;
    [SerializeField] private AudioClip grappleShot = null; // Set up in inspector
    [SerializeField] private AudioClip grappleHit = null; // Set up in inspector
    [SerializeField] private AudioClip cowSuction = null; // Set up in inspector
    // Line parameters
    [SerializeField] private LineRenderer lineRenderer;

    // Awake is called after all objects are initialized
    private void Awake()
    {
        spaceshipMovement = GetComponent<SC_SpaceshipMovement>();
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<SC_AlienUIManager>();
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
            lineRenderer.positionCount = 0;
        }
        lineRenderer.enabled = false;

        attachedObjectJoints = new ConfigurableJoint[numberOfJoints];
        grappling = false;
    }

    // FixedUpdate is called in fixed intervals
    private void FixedUpdate()
    {
        // Left click casts a raycast in the direction of the cursor position.
        if (Input.GetButtonDown("Fire1") && Time.timeScale > Mathf.Epsilon)
        {
            // Do not shoot ray if cow is already attached
            if (attachedObject == null && !grappling)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;                    
                int layerMask = ~(1 << gameObject.layer);
                
                if (Physics.Raycast(ray, out hit, maxCaptureLength, layerMask))
                {
                    captureLength = Vector3.Distance(transform.position, hit.transform.position);

                    StartCoroutine(ShootGrapple(hit));                        
                }
            }
        }

        if (Input.GetButtonDown("GrappleRelease"))
        {
            GrappleRelease();
        }
        if (Input.GetAxis("GrapplePushPull") > 0f)
        {
            GrapplePull();
        }
        else if (Input.GetAxis("GrapplePushPull") < 0f)
        {
            GrapplePush();
        }

        // Draw a line between me and the attached object
        if (attachedObject != null)
        {
            RenderLine(attachedObject);

            Rigidbody attachedRigidboy = attachedObject.GetComponent<Rigidbody>();
            if (attachedRigidboy.velocity.magnitude > 1.0f)
                attachedRigidboy.AddForce(-attachedRigidboy.velocity, ForceMode.Acceleration);
        }
        else
        {
            for (int pos = 0; pos < lineRenderer.positionCount; pos++)
            {
                lineRenderer.SetPosition(pos, transform.position);
            }
        }
    }

    // Render a line to the object including any in-between joints
    private void RenderLine(GameObject obj)
    {
        if (attachedObject != null)
        {
            // Set up points along each joint
            var points = new Vector3[numberOfJoints + 1];
            points[0] = transform.position;
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
            points[0] = transform.position;
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
        
        GetComponent<AudioSource>().PlayOneShot(grappleShot, 0.5f);
        
        grappling = true;
        // Extend the grapple
        float counter = 0.0f;
        while (counter < grappleTime)
        {            
            counter += Time.fixedDeltaTime / grappleTime;
            probeClone.transform.position = Vector3.Lerp(transform.position, hit.point, counter / grappleTime);
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
                probeClone.transform.position = Vector3.Lerp(hit.point, transform.position, counter / grappleTime);
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
        // Debug.Log("Ray cast hit " + hit.transform.gameObject.name);
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

                    // goRigidbody.mass = _rb.mass / 10.0f;
                    goRigidbody.mass = beamOrigin.mass / 10.0f;
                    goRigidbody.drag = 1.0f;
                    goRigidbody.angularDrag = 1.0f;                    

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

            // Set spaceship movement penalty (TO DO: set based on cow mass)
            spaceshipMovement.SetMovementPenaltyFactor(0.5f);

            if (probeClone != null)
            {
                probeClone.transform.parent = attachedObject.transform;
                probeClone.transform.position = attachedObject.transform.position;
            }

            // Toggle UI indicator
            if (!uiManager.cowIcon.enabled)
            {
                uiManager.ToggleCowIcon();
            }

            if (probeClone.GetComponent<AudioSource>())
            {
                probeClone.GetComponent<AudioSource>().PlayOneShot(grappleHit);
            }
            return true;
        }
        return false;
    }

    // Release the attached object
    private void GrappleRelease()
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
            spaceshipMovement.ResetMovementPenaltyFactor();

            // Toggle UI indicator
            if (uiManager.cowIcon.enabled)
            {
                uiManager.ToggleCowIcon();
            }
        }
    }

    // Pull grapple rope 
    private void GrapplePull()
    {
        if (attachedObject != null && attachedObject.tag == "Cow") 
        {
            if (captureLength > 0.0f)
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
                // Disable attached object colliders
                foreach (Collider col in attachedObject.GetComponents<Collider>())
                {
                    col.isTrigger = true;
                }
                // Force joint limits to zero
                foreach (ConfigurableJoint cj in attachedObjectJoints)
                {
                    SoftJointLimit softJointLimit = new SoftJointLimit();
                    softJointLimit.limit = 0;
                    softJointLimit.contactDistance = 0.1f;
                    cj.linearLimit = softJointLimit;
                }
            }
        }
    }

    // Loosen grapple rope
    private void GrapplePush()
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

    // OnTriggerEnter is called when a collision with another collider is detected
    private void OnTriggerEnter(Collider col) 
    {
        if(col.gameObject.tag == "Cow" && col.gameObject == attachedObject)
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
                uiManager = GameObject.FindWithTag("UIManager").GetComponent<SC_AlienUIManager>();
            uiManager.IncreaseScore(1);
            // Apply small upward force for physical feedback
            spaceshipMovement.AddUpwardImpulse(5.0f);
            // Reset carry mass
            spaceshipMovement.ResetMovementPenaltyFactor();
            // Play suction audio
            GetComponent<AudioSource>().PlayOneShot(cowSuction, 0.5f);
        }
    }

    // Reset parameters to starting values
    public void ResetGame() {
        Start();
    }
}
