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
    // Private variables
    private SC_AlienUIManager uiManager;
    private SC_SpaceshipMovement spaceshipMovement;
    private ConfigurableJoint[] attachedObjectJoints;
    private GameObject attachedObject;
    private Rigidbody attachedRigidbody;
    private GameObject probeClone;
    private float captureLength;
    private bool grappling;
    
    // Public variables
    public float grappleTime = 0.5f;
    public float grappleCooldown = 0.5f;
    public float maxCaptureLength = 50.0f;
    public int numberOfJoints = 3;
    public float captureSpeed = 5.0f;

    // Serialized private variables
    [SerializeField] private GameObject reticle = null; // Set up in inspector
    [SerializeField] private GameObject probe = null; // Set up in inspector
    [SerializeField] private Transform grappleOrigin = null; // Set up in inspector
    [SerializeField] private AudioClip grappleShot = null; // Set up in inspector
    [SerializeField] private AudioClip grappleHit = null; // Set up in inspector
    [SerializeField] private AudioClip cowSuction = null; // Set up in inspector
    [SerializeField] private LineRenderer lineRenderer; // (Optional) Set up in inspector

    public GameObject GetAttachedObject()
    {
        return attachedObject;
    }

    // Awake is called after all objects are initialized
    private void Awake()
    {
        spaceshipMovement = GetComponent<SC_SpaceshipMovement>();
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<SC_AlienUIManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Create a line renderer if not set up
        if (!lineRenderer)
        {
            Color lineColor = new Color(255f, 255f, 255f, 0.5f);
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = lineColor;
            lineRenderer.widthMultiplier = 0.25f;
            lineRenderer.positionCount = 0;
        }
        lineRenderer.enabled = false;

        if (!grappleOrigin)
        {
            grappleOrigin = transform;
        }

        attachedObjectJoints = new ConfigurableJoint[numberOfJoints];
        grappling = false;
    }

    // Update is called once per frame
    private void Update()
    {
        // Left click casts a raycast in the direction of the cursor position.
        if (Input.GetButtonDown("Fire1") && Time.timeScale > Mathf.Epsilon)
        {
            // Do not shoot ray if object is already attached
            if (!attachedObject && !grappling)
            {
                // Convert reticle world coordinates to screen coordinates
                Vector3 reticlePoint = RectTransformUtility.WorldToScreenPoint(null, reticle.GetComponent<RectTransform>().position);
                Ray ray = Camera.main.ScreenPointToRay(reticlePoint);
                RaycastHit hit;
                // Ignore UFO layer
                int layerMask = ~(1 << gameObject.layer);
                
                if (Physics.Raycast(ray, out hit, maxCaptureLength, layerMask))
                {
                    captureLength = Vector3.Distance(grappleOrigin.position, hit.transform.position);

                    StartCoroutine(ShootGrapple(hit));
                }
            }
        }

        // Input set up in Input Manager
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

        // Check if object is attached
        if (attachedObject)
        {
            // Enable the line renderer
            RenderLine(attachedObject);

            // Limit velocity of attached object
            if (attachedRigidbody.velocity.magnitude > 1.0f)
                attachedRigidbody.AddForce(-attachedRigidbody.velocity, ForceMode.Acceleration);

            // Check if object has a brain and tugs when grappled
            SC_CowBrain brain = attachedObject.GetComponent<SC_CowBrain>();
            if (!brain)
                brain = attachedObject.GetComponent<SC_FarmerBrain>();
            if (brain && brain.GetTugWhenGrappled())
            {
                attachedRigidbody.AddForce((attachedObject.transform.position - grappleOrigin.position), ForceMode.Acceleration);
                GetComponent<Rigidbody>().AddForce((attachedObject.transform.position - transform.position).normalized, ForceMode.Acceleration);

                // Grapple breaks if attached object is too far
                if (Vector3.Distance(transform.position, attachedObject.transform.position) > captureLength)
                {
                    GrappleRelease();
                }
            }
            else
            {
                Vector3 reticlePoint = RectTransformUtility.WorldToScreenPoint(null, reticle.GetComponent<RectTransform>().position);
                Ray ray = Camera.main.ScreenPointToRay(reticlePoint);
                Vector3 worldPoint = ray.origin + (ray.direction * captureLength);
                Debug.DrawRay(ray.origin, ray.direction * captureLength);

                attachedRigidbody.AddForce(worldPoint - attachedObject.transform.position, ForceMode.Acceleration);

                // Grapple breaks if attached object is too far
                if (Vector3.Distance(transform.position, attachedObject.transform.position) > maxCaptureLength)
                {
                    GrappleRelease();
                }
            }
        }
    }

    // Render a line to the object including any in-between joints
    private void RenderLine(GameObject obj)
    {
        if (obj == attachedObject)
        {
            // Set up points along each joint
            var points = new Vector3[numberOfJoints + 1];
            points[0] = grappleOrigin.position;
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
            points[0] = grappleOrigin.position;
            points[1] = obj.transform.position;
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
        }
        if (!lineRenderer.enabled)
            lineRenderer.enabled = true;
    }

    // Play a grappling animation (TO DO: Use Rigidbody instead of RaycastHit)
    private IEnumerator ShootGrapple(RaycastHit hit)
    {
        grappling = true;

        if (probe)
            probeClone = Instantiate(probe, grappleOrigin.position, Quaternion.identity);
        
        GetComponent<AudioSource>().PlayOneShot(grappleShot, 0.5f);
        
        // Extend the grapple
        float counter = 0.0f;
        while (counter < grappleTime)
        {
            counter += Time.fixedDeltaTime / grappleTime;
            probeClone.transform.position = Vector3.Lerp(grappleOrigin.position, hit.point, counter / grappleTime);
            RenderLine(probeClone);
            yield return null;
        }
        
        if (!AttachBody(hit))
        {
            // Hang at hit position briefly
            counter = 0.0f;
            while (counter < grappleCooldown)
            {
                counter += Time.fixedDeltaTime / grappleTime;
                RenderLine(probeClone);
                yield return null;
            }
           
            // Retract the grapple if it did not attach to a cow or farmer
            counter = 0.0f;
            while (counter < grappleTime)
            {
                counter += Time.fixedDeltaTime / grappleTime;
                probeClone.transform.position = Vector3.Lerp(hit.point, grappleOrigin.position, counter / grappleTime);
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
        if (hit.rigidbody)
        {
            attachedRigidbody = hit.rigidbody;
            attachedRigidbody.useGravity = false;

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
                    go.tag = "Joint";
                    
                    // TO DO: Try to replace hard-coded values
                    Rigidbody goRigidbody = go.AddComponent<Rigidbody>();
                    goRigidbody.mass = 0.1f;
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
                softJointLimit.limit = (captureLength / (float)numberOfJoints) + 0.1f;
                softJointLimit.contactDistance = 0.1f;
                attachedObjectJoints[j].linearLimit = softJointLimit;
                // The first joint is connected to the UFO
                if (j == 0)
                    // attachedObjectJoints[j].connectedBody = GetComponent<Rigidbody>();
                    attachedObjectJoints[j].connectedBody = grappleOrigin.GetComponent<Rigidbody>();
                else
                    attachedObjectJoints[j].connectedBody = attachedObjectJoints[j - 1].gameObject.GetComponent<Rigidbody>();
            }
            attachedObject = hit.transform.gameObject;

            // Set spaceship movement penalty
            spaceshipMovement.SetMovementPenaltyFactor(1 - Mathf.Clamp01(attachedRigidbody.mass / GetComponent<Rigidbody>().mass));

            if (probeClone)
            {
                probeClone.transform.parent = attachedObject.transform;
                probeClone.transform.position = attachedObject.transform.position;
            }

            // Toggle UI indicators
            if (hit.transform.tag == "Cow")
                uiManager.ToggleCowIcon(true);
            else
                uiManager.ToggleCowIcon(false);
            uiManager.ToggleReticle();

            // Play grapple hit sfx
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
        if (attachedObject)
        {
            // Re-enable nav mesh agent
            SC_CowBrain brain = attachedObject.GetComponent<SC_CowBrain>();
            if (!brain)
                brain = attachedObject.GetComponent<SC_FarmerBrain>();
            if (brain)
                StartCoroutine(brain.Recover());

            // Destroy joints
            foreach (ConfigurableJoint cj in attachedObjectJoints)
            {
                if (cj.tag != "Joint")
                    Destroy(cj);
                else
                    Destroy(cj.gameObject);
            }

            // Disable line renderer
            if (lineRenderer.enabled)
                lineRenderer.enabled = false;

            // Destroy probe
            if (probeClone)
                Destroy(probeClone);

            // Reset movement penalty
            spaceshipMovement.ResetMovementPenaltyFactor();

            // Toggle UI indicators
            if (attachedObject.tag == "Cow")
                uiManager.ToggleCowIcon(true);
            else
                uiManager.ToggleCowIcon(false);
            uiManager.ToggleReticle();

            // Enable attached object colliders if necessary
            foreach (Collider col in attachedObject.GetComponents<Collider>())
            {
                if (col.isTrigger)
                    col.isTrigger = false;
            }

            attachedObject = null;
            attachedRigidbody.useGravity = true;
            attachedRigidbody = null;
        }
    }

    // Pull grapple rope 
    private void GrapplePull()
    {
        if (attachedObject && attachedRigidbody)
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
                
                // Apply force on UFO for physical feedback
                GetComponent<Rigidbody>().AddForceAtPosition((grappleOrigin.position - attachedObject.transform.position).normalized, grappleOrigin.position, ForceMode.Acceleration);
            }
            else
            {
                // Only suck in cows
                if (attachedObject.tag == "Cow")
                {
                    // Release cow if object in between UFO and cow
                    if (Physics.Raycast(grappleOrigin.position, (attachedObject.transform.position - grappleOrigin.position), out RaycastHit hit))
                    {
                        if (hit.collider.tag != "Cow")
                        {
                            GrappleRelease();
                        }
                        else
                        {
                            // Disable attached object colliders
                            foreach (Collider col in attachedObject.GetComponents<Collider>())
                            {
                                col.isTrigger = true;
                            }
                            // Spring attached body towards UFO
                            attachedRigidbody.AddForce((grappleOrigin.position - attachedObject.transform.position) * attachedRigidbody.mass, ForceMode.Impulse);
                        }
                    }
                }
                else
                {
                    GrappleRelease();
                }
            }
        }
    }

    // Loosen grapple rope
    private void GrapplePush()
    {
        if (attachedObject && attachedRigidbody) 
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
            // Apply force for physical feedback
            spaceshipMovement.AddImpulseForce(attachedRigidbody.velocity.normalized, Mathf.Clamp(attachedRigidbody.mass * 0.5f, 1f, 10f));

            // Destroy grappling hook and attached object
            if (probeClone)
                Destroy(probeClone);
            
            foreach(ConfigurableJoint cj in attachedObjectJoints)
            {
                if (cj.gameObject)
                    Destroy(cj.gameObject);
            }

            attachedObject = null;
            lineRenderer.enabled = false;

            // Call the UI manager to increase score            
            uiManager.IncreaseScore(col.GetComponent<SC_CowBrain>().GetMilk());
            uiManager.ToggleReticle();

            // Reset movement penalty
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
