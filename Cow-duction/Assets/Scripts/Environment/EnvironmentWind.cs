/*  EnvironmentWind.cs

    Pushes rigidbodies within the trigger zone in the forward direction of the transform.
*/

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnvironmentWind : MonoBehaviour
{
    [SerializeField] private float pushForce = 10.0f;
    private Vector3 pushDirection;

    // Start is called before the first frame update
    void Start()
    {
        pushDirection = transform.forward;
    }

    // Pushes rigidbody in forward direction while inside trigger zone
    void OnTriggerStay(Collider other)
    {
        Rigidbody otherRigidbody = other.gameObject.GetComponent<Rigidbody>();
        if (otherRigidbody)
        {
            otherRigidbody.AddForce(pushForce * pushDirection, ForceMode.Acceleration);
        }
    }
}
