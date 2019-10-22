using UnityEngine;

public class CowAbduction : MonoBehaviour
{
    private Rigidbody _rb;
    public Rigidbody beamOrigin;
    [SerializeField] private UIManager uiManager;
    // Joint parameters
    public float maxCaptureLength = 50.0f;
    public int numberOfJoints = 3;
    public float captureSpeed = 5.0f;
    [SerializeField] private float captureCooldown;
    [SerializeField] private bool captureCooldownActive;
    [SerializeField] private float captureLength;
    [SerializeField] private ConfigurableJoint[] attachedObjectJoints;
    [SerializeField] public GameObject attachedObject;
    // Line parameters
    [SerializeField] private LineRenderer lineRenderer;
    public Color lineStartColor = Color.green;
    public Color lineEndColor = Color.white;

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
            lineRenderer.widthMultiplier = 1f;
            lineRenderer.positionCount = 2;
            lineRenderer.startColor = lineStartColor;
            lineRenderer.endColor = lineEndColor;
        }
        lineRenderer.enabled = false;
        attachedObjectJoints = new ConfigurableJoint[numberOfJoints];
        captureCooldownActive = false;
        captureCooldown = 0.5f;
    }

    // FixedUpdate is called in fixed intervals
    private void FixedUpdate()
    {
        if (captureCooldownActive)
        {
            if (captureCooldown < 0.5f)
                captureCooldown += Time.fixedDeltaTime;
            else
                captureCooldownActive = false;    
        }

        // Left click casts a raycast in the direction of the cursor position.
        // If a cow is already attached, release it.
        // Otherwise if the raycast hits a cow, the cow becomes attached to my rigidbody through configurable joints.
        if (Input.GetMouseButtonDown(0))
        {
            if (!captureCooldownActive)
            {
                captureCooldownActive = true;
                // Do not shoot ray if cow is already attached
                if (attachedObject == null)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, maxCaptureLength))
                    {
                        Debug.DrawLine(transform.position, hit.point, Color.yellow);
                        Debug.Log("Ray cast hit " + hit.transform.gameObject.name);
                        if (hit.collider.tag == "Cow" && hit.rigidbody != null)
                        {
                            if (!hit.transform.gameObject.GetComponent<ConfigurableJoint>())
                            {
                                captureLength = Vector3.Distance(transform.position, hit.transform.position);

                                for(int j = 0; j < numberOfJoints; j++)
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

                                        go.transform.position = (j + 1) * (hit.transform.position - transform.position) / (float)numberOfJoints;

                                        attachedObjectJoints[j] = go.AddComponent<ConfigurableJoint>();
                                    }                                                                                        
                                    
                                    attachedObjectJoints[j].autoConfigureConnectedAnchor = false;
                                    attachedObjectJoints[j].connectedAnchor = Vector3.zero;
                                    attachedObjectJoints[j].projectionMode = JointProjectionMode.PositionAndRotation;

                                    attachedObjectJoints[j].xMotion = ConfigurableJointMotion.Limited;
                                    attachedObjectJoints[j].yMotion = ConfigurableJointMotion.Limited;
                                    attachedObjectJoints[j].zMotion = ConfigurableJointMotion.Limited;

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
                            }
                            attachedObject = hit.transform.gameObject;                        
                        }
                    }
                }
            }
        }

        // Release the attached cow
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (attachedObject != null)
            {
                attachedObject.GetComponent<Rigidbody>().drag = 1.0f;
                foreach (ConfigurableJoint cj in attachedObjectJoints)
                {
                    if (cj != null)
                    {
                        if (cj.tag == "Cow")
                            Destroy(cj);
                        else
                            Destroy(cj.gameObject);
                    }
                }
                if (lineRenderer.enabled)
                    lineRenderer.enabled = false;
                attachedObject = null;
            }
        }

        // Holding comma pulls the attached cow towards the UFO
        if (Input.GetKey(KeyCode.Comma)) 
        {
            if (attachedObject != null) 
            {
                if (captureLength > 0)
                {
                    captureLength -= Time.deltaTime * captureSpeed;
                    SoftJointLimit softJointLimit = new SoftJointLimit();
                    softJointLimit.limit = captureLength / (float)numberOfJoints;
                    softJointLimit.contactDistance = 0.1f;
                    foreach(ConfigurableJoint cj in attachedObjectJoints)
                        cj.linearLimit = softJointLimit;
                }
            }
        }

        // Holding period pushes the attached cow away from the UFO
        if (Input.GetKey(KeyCode.Period)) 
        {
            if (attachedObject != null) 
            {
                if (captureLength < maxCaptureLength)
                {
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
            var points = new Vector3[2];
            points[0] = transform.position + Vector3.down;
            points[1] = attachedObject.transform.position;
            lineRenderer.SetPositions(points);
            if (!lineRenderer.enabled)
                lineRenderer.enabled = true;
            
            Rigidbody attachedRigidboy = attachedObject.GetComponent<Rigidbody>();
            Vector3 newVelocity = attachedRigidboy.velocity;
            Mathf.Clamp(newVelocity.y, -1, 1);
            attachedRigidboy.velocity = newVelocity;
        }
    }

    // OnTriggerEnter is called when a collision with another collider is detected
    private void OnTriggerEnter(Collider col) 
    {
        if(col.gameObject.tag == "Cow")
        {
            Debug.Log("Collision with cow detected");
            Destroy(col.gameObject);
            foreach(ConfigurableJoint cj in attachedObjectJoints)
            {
                if (cj != null && cj.tag != "Cow")
                    Destroy(cj.gameObject);
            }
            lineRenderer.enabled = false;
            attachedObject = null;
            if(uiManager == null)
                uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
            uiManager.IncreaseScore(1);
            _rb.AddRelativeForce(_rb.transform.up * _rb.mass, ForceMode.Impulse);
        }
    }

    public void ResetGame() {
        Start();
    }
}
