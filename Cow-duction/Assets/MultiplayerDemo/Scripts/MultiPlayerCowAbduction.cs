using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Mirror;

/// <summary>
/// Grappling hook system that shoots a raycast from the Main Camera in the direction of the reticle or mouse position.
/// The grappling hook attaches to Cows and Farmers with a line of Configurable Joints.
/// The length of the hook rope can be increased or decreased to pull or push the attached object.
/// </summary>
/// <remarks>
/// <para>There should be both a normal collider and a trigger collider on this object. The trigger collider should be larger than the normal collider.</para>
/// <para>GameObjects that interact with the grappling hook should have a Collider component and a Rigidbody if it does not have a NavMeshAgent component.</para>
/// </remarks>
[RequireComponent(typeof(MultiPlayerSpaceshipController))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class MultiPlayerCowAbduction : NetworkBehaviour
{
    private SpaceshipCanvas spaceshipCanvas;
    private MultiPlayerSpaceshipController spaceshipController;
    private AudioSource audioSource;
    private Rigidbody rb;
    private ConfigurableJoint[] attachedObjectJoints;
    private GameObject attachedObject;
    private Rigidbody attachedRigidbody;
    private MultiPlayerCowBrain attachedBrain;
    private GameObject probeClone;
    private RectTransform waypointIcon;
    private float captureLength;
    private bool grappling;

    [Header("Parameters")]
    public int grappleJointCount = 5;
    public float grappleTime = 0.25f;
    public float grappleCooldown = 0.5f;
    public float maxCaptureLength = 50.0f;
    public float captureSpeed = 5.0f;
    public float reticleAttractionForce = 1.0f;

    [Tooltip("A prefab which will spawn when firing the grapple")]
    public GameObject probe = null;
    [Tooltip("An empty transform which marks the grapple origin position")]
    public Transform grappleOrigin = null;
    [Tooltip("(Optional) The line renderer for the grapple rope")]
    public LineRenderer lineRenderer;

    [Header("SFX")]
    public AudioClip grappleShot = null;
    public AudioClip grappleHit = null;
    public AudioClip grappleBreak = null;
    public AudioClip grappleReel = null;
    public AudioClip cowSuction = null;

    [Header("Diagnostics")]
    public float grapplePushPull = 0f;
    public bool isFiring = false;
    public bool isPulling = false;
    public bool isReleasingGrapple = false;

    // OnStartLocalPlayer is called when the local player object is set up
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        ResetGame();
    }

    // OnDisable is called when the object is removed from the server
    private void OnDisable()
    {
        if (isLocalPlayer)
        {
            ResetGame();
        }
    }

    // Awake is called after all objects are initialized
    private void Awake()
    {
        spaceshipController = GetComponent<MultiPlayerSpaceshipController>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        spaceshipCanvas = SpaceshipCanvas.instance;
        waypointIcon = spaceshipCanvas.waypointIcon;

        ResetGame();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isLocalPlayer) return;

        grapplePushPull = Input.GetAxis("GrapplePushPull");
        isFiring = Input.GetButtonDown("Fire1");
        isPulling = Input.GetButton("Fire1");
        isReleasingGrapple = Input.GetButtonDown("GrappleRelease");
    }

    // FixedUpdate is called in fixed time intervals
    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        Ray ray = RayFromReticle(spaceshipCanvas.reticle);
        RaycastHit hit;
        int layerMask = ~(1 << gameObject.layer);

        // Left click casts a raycast in the direction of the cursor position.
        if (isFiring && Time.timeScale > Mathf.Epsilon)
        {
            // Do not shoot ray if object is already attached
            if (!attachedObject && !grappling && Camera.main)
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
                {
                    captureLength = Vector3.Distance(grappleOrigin.position, hit.transform.position);
                    StartCoroutine(ShootGrapple(hit));
                }
            }
        }
        else if (Physics.Raycast(ray, out hit, maxCaptureLength, layerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.tag == "Cow")
            {
                spaceshipCanvas.SetReticleColor(Color.green);
            }
            else if (hit.transform.tag == "Farmer")
            {
                spaceshipCanvas.SetReticleColor(Color.red);
            }
            else if (spaceshipCanvas.GetReticleColor() != Color.white)
            {
                spaceshipCanvas.SetReticleColor(Color.white);
            }
        }

        if (isReleasingGrapple)
        {
            GrappleRelease();
        }

        if (grapplePushPull > 0f || isPulling)
        {
            GrapplePull();

            if (attachedObject)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.loop = true;
                    audioSource.clip = grappleReel;
                    audioSource.Play();
                }
            }
            else if (audioSource.clip == grappleReel)
            {
                audioSource.clip = null;
            }
        }
        else if (grapplePushPull < 0f)
        {
            GrapplePush();
        }
        else
        {
            if (audioSource.clip == grappleReel)
            {
                audioSource.clip = null;
            }
        }

        // Check if object is attached
        if (attachedObject)
        {
            // Enable the line renderer
            RpcRenderLine(attachedObject);

            // Limit velocity of attached object
            if (attachedRigidbody.velocity.magnitude > 1.0f)
                attachedRigidbody.AddForce(-attachedRigidbody.velocity, ForceMode.Acceleration);

            // Check if object has a brain and tugs when grappled
            if (attachedBrain && attachedBrain.tugWhenGrappled)
            {
                attachedRigidbody.AddForce((attachedObject.transform.position - grappleOrigin.position), ForceMode.Acceleration);
                rb.AddForce((attachedObject.transform.position - transform.position) / 2, ForceMode.Acceleration);
            }
            else
            {
                Vector3 worldPoint = ray.origin + (ray.direction * captureLength);

                // Attached object is attracted to reticle postion
                attachedRigidbody.AddForce((worldPoint - attachedObject.transform.position) * reticleAttractionForce, ForceMode.Acceleration);

                rb.AddForce((attachedObject.transform.position - transform.position).normalized, ForceMode.Acceleration);
            }

            // Grapple breaks if attached object is too far
            if (Vector3.Distance(transform.position, attachedObject.transform.position) > maxCaptureLength)
            {
                GrappleRelease();
            }

            ClampWaypointIcon();
        }
        else // attachedObject == null
        {
            if (waypointIcon != null && waypointIcon.gameObject.activeSelf)
                waypointIcon.gameObject.SetActive(false);

            if (audioSource.isPlaying && audioSource.clip == grappleReel)
            {
                audioSource.loop = false;
                audioSource.clip = null;
            }
        }
    }

    private Ray RayFromReticle(RectTransform reticle)
    {
        if (reticle != null)
        {
            Vector3 reticlePoint = RectTransformUtility.WorldToScreenPoint(null, reticle.position);
            return Camera.main.ScreenPointToRay(reticlePoint);
        }
        else
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }

    private void ClampWaypointIcon()
    {
        if (waypointIcon != null)
        {
            if (!waypointIcon.gameObject.activeSelf)
                waypointIcon.gameObject.SetActive(true);

            // Waypoint tracks position of attached object, clamped to the screen
            Vector3 waypointIconPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, attachedObject.transform.position);
            // Clamp x position
            if (waypointIconPosition.x < waypointIcon.rect.width / 2)
                waypointIconPosition.x = waypointIcon.rect.width / 2;
            else if (waypointIconPosition.x > Screen.width - (waypointIcon.rect.width / 2))
                waypointIconPosition.x = Screen.width - (waypointIcon.rect.width / 2);
            // Clamp y position
            if (waypointIconPosition.y < waypointIcon.rect.height / 2)
                waypointIconPosition.y = waypointIcon.rect.height / 2;
            else if (waypointIconPosition.y > Screen.height - (waypointIcon.rect.height / 2))
                waypointIconPosition.y = Screen.height - (waypointIcon.rect.height / 2);
            // Set position
            waypointIcon.position = waypointIconPosition;
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
            lineRenderer.sortingOrder = 1;
        }
        lineRenderer.enabled = false;

        if (!grappleOrigin)
        {
            grappleOrigin = transform;
        }

        attachedObjectJoints = new ConfigurableJoint[grappleJointCount];
        grappling = false;
        if (probeClone != null)
            Destroy(probeClone);
    }

    // Render a line to the object including any in-between joints
    [ClientRpc]
    private void RpcRenderLine(GameObject obj)
    {
        if (obj == null)
        {
            lineRenderer.enabled = false;
            return;
        }

        Vector3[] points;
        if (obj == attachedObject && attachedObjectJoints != null)
        {
            // Set up points along each joint
            points = new Vector3[grappleJointCount + 1];
            points[0] = grappleOrigin.position;
            for (int j = 0; j < grappleJointCount; j++)
            {
                points[j + 1] = attachedObjectJoints[j].transform.position;
            }
            lineRenderer.numCornerVertices = lineRenderer.positionCount;
        }
        else
        {
            // Set up two points for a straight line
            points = new Vector3[2];
            points[0] = grappleOrigin.position;
            points[1] = obj.transform.position;
        }
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);

        if (!lineRenderer.enabled)
            lineRenderer.enabled = true;
    }

    // Play a grappling animation
    private IEnumerator ShootGrapple(RaycastHit hit)
    {
        Vector3 grappleHitPoint = hit.point;

        if (hit.distance > maxCaptureLength)
        {
            Ray ray = RayFromReticle(spaceshipCanvas.reticle);
            grappleHitPoint = ray.origin + (ray.direction * maxCaptureLength);
        }

        grappling = true;

        if (probe)
        {
            probeClone = Instantiate(probe, grappleOrigin.position, Quaternion.identity);
            NetworkServer.Spawn(probeClone);
        }
        
        audioSource.PlayOneShot(grappleShot, 0.5f);
        
        // Extend the grapple
        float counter = 0.0f;
        while (counter < grappleTime)
        {
            counter += Time.fixedDeltaTime / grappleTime;
            probeClone.transform.position = Vector3.Lerp(grappleOrigin.position, grappleHitPoint, counter / grappleTime);
            RpcRenderLine(probeClone);
            yield return null;
        }
        
        if (!AttachBody(hit))
        {
            // Hang at hit position briefly
            counter = 0.0f;
            while (counter < grappleCooldown)
            {
                counter += Time.fixedDeltaTime / grappleTime;
                RpcRenderLine(probeClone);
                yield return null;
            }
           
            // Retract the grapple if it did not attach to a cow or farmer
            counter = 0.0f;
            while (counter < grappleTime)
            {
                counter += Time.fixedDeltaTime / grappleTime;
                probeClone.transform.position = Vector3.Lerp(grappleHitPoint, grappleOrigin.position, counter / grappleTime);
                RpcRenderLine(probeClone);
                yield return null;
            }
            Destroy(probeClone);
            RpcRenderLine(null);
        }
        grappling = false;
    }

    // Attach to an object through a series of joints
    private bool AttachBody(RaycastHit hit)
    {
        if (hit.distance > maxCaptureLength)
            return false;
        
        if (hit.transform.tag == "Cow" || hit.transform.tag == "Farmer")
        {
            if (attachedBrain = hit.transform.gameObject.GetComponent<MultiPlayerCowBrain>())
            {
                attachedBrain.PlayMoo(3f);
                if (!hit.transform.gameObject.GetComponent<Rigidbody>())
                {
                    hit.transform.gameObject.AddComponent<Rigidbody>().mass = attachedBrain.mass;
                }
            }

            if (hit.transform.tag == "Cow")
                spaceshipCanvas.SetWaypointIconSprite(spaceshipCanvas.waypointIconCow);
            else if (hit.transform.tag == "Farmer")
                spaceshipCanvas.SetWaypointIconSprite(spaceshipCanvas.waypointIconThreat);
        }
        else
        {
            spaceshipCanvas.SetWaypointIconSprite(spaceshipCanvas.waypointIconUnknown);
        }

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
            attachedObjectJoints = new ConfigurableJoint[grappleJointCount];
            for (int j = 0; j < grappleJointCount; j++)
            {
                // The last joint connects the cow to the joint chain
                if (j == grappleJointCount - 1)
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

                    go.transform.position = (j + 1) * (transform.position - hit.transform.position) / (float)grappleJointCount;

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
                softJointLimit.limit = (captureLength / (float)grappleJointCount) + 0.1f;
                softJointLimit.contactDistance = 0.1f;
                attachedObjectJoints[j].linearLimit = softJointLimit;
                // The first joint is connected to the UFO
                if (j == 0)
                {
                    attachedObjectJoints[j].connectedBody = rb;
                    attachedObjectJoints[j].connectedAnchor = grappleOrigin.localPosition;
                }
                else
                    attachedObjectJoints[j].connectedBody = attachedObjectJoints[j - 1].gameObject.GetComponent<Rigidbody>();
            }
            attachedObject = hit.transform.gameObject;

            // Set spaceship movement penalty
            spaceshipController.SetMovementMultiplier(1 - Mathf.Clamp01(attachedRigidbody.mass / rb.mass));

            NetworkIdentity objNetId = attachedObject.GetComponent<NetworkIdentity>();
            if (objNetId != null)
            {
                objNetId.AssignClientAuthority(netIdentity.connectionToClient);
            }

            if (probeClone)
            {
                probeClone.transform.parent = attachedObject.transform;
                probeClone.transform.localPosition = Vector3.zero;
            }

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
                RpcRenderLine(null);

            // Destroy probe
            if (probeClone)
                Destroy(probeClone);

            // Reset movement penalty
            spaceshipController.SetMovementMultiplier();

            // Enable attached object colliders if necessary
            foreach (Collider col in attachedObject.GetComponents<Collider>())
            {
                if (col.isTrigger)
                    col.isTrigger = false;
            }

            NetworkIdentity objNetId = attachedObject.GetComponent<NetworkIdentity>();
            if (objNetId != null)
                objNetId.RemoveClientAuthority();

            // Play grapple break audio clip
            audioSource.PlayOneShot(grappleBreak, 1f);

            attachedObject = null;
            attachedRigidbody.useGravity = true;
            attachedRigidbody = null;
            attachedBrain = null;
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
                softJointLimit.limit = captureLength / (float)grappleJointCount;
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
                softJointLimit.limit = captureLength / (float)grappleJointCount;
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
            // spaceshipCanvas.IncreaseScore(col.GetComponent<MultiPlayerCowBrain>().milk);
            GetComponentInChildren<MultiPlayerAlienUIManager>().IncreaseScore(col.GetComponent<MultiPlayerCowBrain>().milk, null);

            GetComponent<MultiPlayerCowShooter>().AddCow();

            // Apply force for physical feedback
            spaceshipController.AddImpulseForce(attachedRigidbody.velocity.normalized, Mathf.Clamp(attachedRigidbody.mass * 0.5f, 1f, 10f));

            // Destroy grappling hook and attached object
            if (probeClone)
                Destroy(probeClone);
            
            foreach(ConfigurableJoint cj in attachedObjectJoints)
            {
                Destroy(cj.gameObject);
            }

            attachedObject = null;
            RpcRenderLine(null);

            // Reset movement penalty
            spaceshipController.SetMovementMultiplier();

            // Play suction audio
            audioSource.PlayOneShot(cowSuction, 0.5f);
        }
    }
}
