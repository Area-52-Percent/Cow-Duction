/* MultiPlayerCowAbduction.cs

   Grappling hook system that shoots a raycast from the Main Camera in the direction of the mouse position.
   The grappling hook attaches to Cows and Farmers with a line of Configurable Joints.
   The length of the hook rope can be increased or decreased to pull or push the attached object.
   
   Assumptions:
     This component belongs to a GameObject with Rigidbody and Collider (set as trigger) components.
     There is a GameObject in the scene with the "UIManager" tag and a UIManager component.
     There is a Shader file located at "./Sprites/Default".
     GameObjects that interact with the grappling hook have Collider and Rigidbody components.
 */

using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Mirror;

[RequireComponent(typeof(MultiPlayerSpaceshipController))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class MultiPlayerCowAbduction : NetworkBehaviour
{
    // Private variables
    // private SC_AlienUIManager uiManager;
    private MultiPlayerSpaceshipController spaceshipController;
    private Rigidbody rb;
    private ConfigurableJoint[] attachedObjectJoints;
    private GameObject attachedObject;
    private Rigidbody attachedRigidbody;
    private GameObject probeClone;
    private float captureLength;
    private bool grappling;
    
    [Header("Parameters")]
    public float grappleTime = 0.5f;
    public float grappleCooldown = 0.5f;
    public float maxCaptureLength = 50.0f;
    public int numberOfJoints = 5;
    public float captureSpeed = 5.0f;
    public float reticleAttractionForce = 1.0f;
    [Tooltip("A canvas image whose position will be used in raycast calls")]
    [SerializeField] private GameObject reticle = null;
    [Tooltip("A prefab which will spawn when firing the grapple")]
    [SerializeField] private GameObject probe = null;
    [Tooltip("An empty transform which marks the grapple origin position")]
    [SerializeField] private Transform grappleOrigin = null;
    [Tooltip("(Optional) The line renderer for the grapple rope")]
    [SerializeField] private LineRenderer lineRenderer;

    [Header("SFX")]
    [SerializeField] private AudioClip grappleShot = null;
    [SerializeField] private AudioClip grappleHit = null;
    [SerializeField] private AudioClip grappleBreak = null;
    [SerializeField] private AudioClip grappleReel = null;
    [SerializeField] private AudioClip cowSuction = null;

    [Header("Waypoint Icon")]
    [SerializeField] private RectTransform waypointIcon = null;
    [SerializeField] private Sprite cowIcon = null;
    [SerializeField] private Sprite unknownIcon = null;
    [SerializeField] private Sprite warningIcon = null;

    public GameObject GetAttachedObject()
    {
        return attachedObject;
    }

    // Awake is called after all objects are initialized
    void Awake()
    {
        spaceshipController = GetComponent<MultiPlayerSpaceshipController>();
        rb = GetComponent<Rigidbody>();
        // uiManager = GameObject.FindWithTag("UIManager").GetComponent<SC_AlienUIManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetGame();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        ResetGame();
    }

    void OnDisable()
    {
        if (isLocalPlayer)
        {
            ResetGame();
        }
    }

    [Header("Diagnostics")]
    public float grapplePushPull = 0f;
    public bool isFiring = false;
    public bool isPulling = false;
    public bool isReleasingGrapple = false;

    void Update()
    {
        if (!isLocalPlayer) return;

        grapplePushPull = Input.GetAxis("GrapplePushPull");
        isFiring = Input.GetButtonDown("Fire1");
        isPulling = Input.GetButton("Fire1");
        isReleasingGrapple = Input.GetButtonDown("GrappleRelease");
    }

    void FixedUpdate()
    {
        // Left click casts a raycast in the direction of the cursor position.
        if (isFiring && Time.timeScale > Mathf.Epsilon)
        {
            // Do not shoot ray if object is already attached
            if (!attachedObject && !grappling && Camera.main)
            {
                Ray ray = new Ray();
                if (reticle != null)
                {
                    // Convert reticle world coordinates to screen coordinates
                    Vector3 reticlePoint = RectTransformUtility.WorldToScreenPoint(null, reticle.GetComponent<RectTransform>().position);
                    ray = Camera.main.ScreenPointToRay(reticlePoint);
                }
                else
                {
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                }

                // Ignore UFO layer and trigger colliders
                int layerMask = ~(1 << gameObject.layer);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
                {
                    captureLength = Vector3.Distance(grappleOrigin.position, hit.transform.position);
                    StartCoroutine(ShootGrapple(hit));
                }
            }
        }

        if (isReleasingGrapple)
        {
            GrappleRelease();
        }

        AudioSource ufoAudioSource = GetComponent<AudioSource>();
        if (grapplePushPull > 0f || isPulling)
        {
            GrapplePull();

            if (attachedObject)
            {
                if (!ufoAudioSource.isPlaying)
                {
                    ufoAudioSource.loop = true;
                    ufoAudioSource.clip = grappleReel;
                    ufoAudioSource.Play();
                }
            }
            else if (ufoAudioSource.clip == grappleReel)
            {
                ufoAudioSource.clip = null;
            }
        }
        else if (grapplePushPull < 0f)
        {
            GrapplePush();
        }
        else
        {
            if (ufoAudioSource.clip == grappleReel)
            {
                ufoAudioSource.clip = null;
            }
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
                rb.AddForce((attachedObject.transform.position - transform.position) / 2, ForceMode.Acceleration);
            }
            else
            {
                Ray ray = new Ray();
                if (reticle != null)
                {
                    Vector3 reticlePoint = RectTransformUtility.WorldToScreenPoint(null, reticle.GetComponent<RectTransform>().position);
                    ray = Camera.main.ScreenPointToRay(reticlePoint);
                }
                else
                {
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                }
                Vector3 worldPoint = ray.origin + (ray.direction * captureLength);
                Debug.DrawRay(ray.origin, ray.direction * captureLength);

                // Attached object is attracted to reticle postion
                attachedRigidbody.AddForce((worldPoint - attachedObject.transform.position) * reticleAttractionForce, ForceMode.Acceleration);

                rb.AddForce((attachedObject.transform.position - transform.position).normalized, ForceMode.Acceleration);
            }

            // Grapple breaks if attached object is too far
            if (Vector3.Distance(transform.position, attachedObject.transform.position) > maxCaptureLength)
            {
                GrappleRelease();
            }

            if (waypointIcon != null && !waypointIcon.gameObject.activeSelf)
                waypointIcon.gameObject.SetActive(true);
            
            // Waypoint tracks position of attached object, clamped to the screen
            Vector3 waypointIconPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, attachedObject.transform.position);
            if (waypointIconPosition.x < waypointIcon.rect.width)
                waypointIconPosition.x = waypointIcon.rect.width;
            else if (waypointIconPosition.x > Screen.width - waypointIcon.rect.width)
                waypointIconPosition.x = Screen.width - waypointIcon.rect.width;
            if (waypointIconPosition.y < waypointIcon.rect.height)
                waypointIconPosition.y = waypointIcon.rect.height;
            else if (waypointIconPosition.y > Screen.height - waypointIcon.rect.height)
                waypointIconPosition.y = Screen.height - waypointIcon.rect.height;
            waypointIcon.position = waypointIconPosition;
        }
        else // attachedObject == null
        {
            if (waypointIcon != null && waypointIcon.gameObject.activeSelf)
                waypointIcon.gameObject.SetActive(false);

            if (ufoAudioSource.isPlaying && ufoAudioSource.clip == grappleReel)
            {
                ufoAudioSource.loop = false;
                ufoAudioSource.clip = null;
            }
        }
    }

    // Render a line to the object including any in-between joints
    private void RenderLine(GameObject obj)
    {
        if (obj == attachedObject && attachedObjectJoints != null)
        {
            // Set up points along each joint
            var points = new Vector3[numberOfJoints + 1];
            points[0] = grappleOrigin.position;
            for (int j = 0; j < numberOfJoints; j++)
            {
                points[j + 1] = attachedObjectJoints[j].transform.position;
            }
            lineRenderer.positionCount = points.Length;
            lineRenderer.numCornerVertices = lineRenderer.positionCount;
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

    // Play a grappling animation
    private IEnumerator ShootGrapple(RaycastHit hit)
    {
        Vector3 grappleHitPoint = hit.point;

        if (hit.distance > maxCaptureLength)
        {
            Ray ray = new Ray();
            if (reticle != null)
            {
                Vector3 reticlePoint = RectTransformUtility.WorldToScreenPoint(null, reticle.GetComponent<RectTransform>().position);
                ray = Camera.main.ScreenPointToRay(reticlePoint);
            }
            else
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            }
            grappleHitPoint = ray.origin + (ray.direction * maxCaptureLength);
        }

        grappling = true;

        if (probe)
        {
            probeClone = Instantiate(probe, grappleOrigin.position, Quaternion.identity);
            NetworkServer.Spawn(probeClone);
        }
        
        GetComponent<AudioSource>().PlayOneShot(grappleShot, 0.5f);
        
        // Extend the grapple
        float counter = 0.0f;
        while (counter < grappleTime)
        {
            counter += Time.fixedDeltaTime / grappleTime;
            probeClone.transform.position = Vector3.Lerp(grappleOrigin.position, grappleHitPoint, counter / grappleTime);
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
                probeClone.transform.position = Vector3.Lerp(grappleHitPoint, grappleOrigin.position, counter / grappleTime);
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
        if (hit.distance > maxCaptureLength)
            return false;
        
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
                    attachedObjectJoints[j].connectedBody = rb;
                    // attachedObjectJoints[j].connectedBody = grappleOrigin.GetComponent<Rigidbody>();
                else
                    attachedObjectJoints[j].connectedBody = attachedObjectJoints[j - 1].gameObject.GetComponent<Rigidbody>();
            }
            attachedObject = hit.transform.gameObject;

            // Set spaceship movement penalty
            spaceshipController.SetMovementMultiplier(1 - Mathf.Clamp01(attachedRigidbody.mass / rb.mass));

            if (probeClone)
            {
                probeClone.transform.parent = attachedObject.transform;
                probeClone.transform.position = attachedObject.transform.position;
            }

            // Toggle UI indicator
            // if (uiManager != null)
            //     uiManager.ToggleReticle();

            // Change waypoint sprite
            if (hit.transform.tag == "Cow") {
                // uiManager.SetWaypointIcon(cowIcon);

                SC_CowBrain cowBrain = hit.transform.GetComponent<SC_CowBrain>();
                if (cowBrain)
                    cowBrain.PlayMoo(3f);
            }
            // else if (hit.transform.tag == "Farmer" || hit.transform.tag == "Bull")
            //     uiManager.SetWaypointIcon(warningIcon);
            // else
            //     uiManager.SetWaypointIcon(unknownIcon);

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
            spaceshipController.SetMovementMultiplier(1f);

            // Toggle UI indicator
            // if (uiManager != null)
            //     uiManager.ToggleReticle();

            // Enable attached object colliders if necessary
            foreach (Collider col in attachedObject.GetComponents<Collider>())
            {
                if (col.isTrigger)
                    col.isTrigger = false;
            }

            // Play grapple break audio clip
            GetComponent<AudioSource>().PlayOneShot(grappleBreak, 1f);

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
                rb.AddForceAtPosition((grappleOrigin.position - attachedObject.transform.position).normalized, Camera.main.transform.position, ForceMode.Acceleration);
            }
            else
            {
                // Only suck in cows
                if (attachedObject.tag == "Cow")
                {
                    // Release cow if object in between UFO and cow
                    int layerMask = ~(gameObject.layer);
                    if (Physics.Raycast(transform.position, (attachedObject.transform.position - transform.position).normalized, out RaycastHit hit, maxCaptureLength, layerMask))
                    {
                        if (hit.collider.tag == "Cow")
                        {
                            // Disable attached object colliders
                            if (!attachedObject.GetComponent<Collider>().isTrigger)
                            {
                                foreach (Collider col in attachedObject.GetComponents<Collider>())
                                {
                                    col.isTrigger = true;
                                }
                            }
                            // Spring attached body towards UFO
                            attachedRigidbody.AddForce((transform.position - attachedObject.transform.position) * attachedRigidbody.mass * 10f, ForceMode.Impulse);
                        }
                        else
                        {
                            GrappleRelease();
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
            spaceshipController.AddImpulseForce(attachedRigidbody.velocity.normalized, Mathf.Clamp(attachedRigidbody.mass * 0.5f, 1f, 10f));

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
            // uiManager.IncreaseScore(col.GetComponent<SC_CowBrain>().GetMilk());
            // uiManager.ToggleReticle();

            // Reset movement penalty
            spaceshipController.SetMovementMultiplier(1f);

            // Play suction audio
            GetComponent<AudioSource>().PlayOneShot(cowSuction, 0.5f);
        }
    }

    // Reset parameters to starting values
    public void ResetGame() {
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
        if (probeClone != null)
            Destroy(probeClone);
    }
}
