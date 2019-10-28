/* CowBrain.cs

    Simple AI that randomly selects a destination on a flat plane to travel to.
    The destination updates once the agent is within 1 meter of it.
   
   Assumptions:
     This component is attached to a GameObject with Collider, NavMeshAgent and Rigidbody components.
 */

using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CowBrain : MonoBehaviour
{
    [SerializeField] private NavMeshAgent cowAgent;
    [SerializeField] private int minX;
    [SerializeField] private int maxX;
    [SerializeField] private int minZ;
    [SerializeField] private int maxZ;

    // Start is called before the first frame update
    void Start()
    {
        cowAgent = GetComponent<NavMeshAgent>();
        cowAgent.destination = new Vector3(Random.Range(minX, maxX), 0f, Random.Range(minZ, maxZ));
    }

    // Update is called once per frame
    void Update()
    {
        if (cowAgent.enabled && cowAgent.remainingDistance < 1f)
        {
            cowAgent.destination = new Vector3(Random.Range(minX, maxX), 0f, Random.Range(minZ, maxZ));
        }
    }
}
