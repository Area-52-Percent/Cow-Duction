using UnityEngine;
using UnityEngine.AI;

public class FollowPath : MonoBehaviour
{
    public NavMeshAgent agent;
    public WaypointSystem waypointSystem;
    public Transform nextWaypoint;
    public int currentWaypointIndex = 0;
    public float precision = .5f;

    // Start is called before the first frame update
    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = precision;

        UpdateWaypoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWaypointIndex < waypointSystem.waypoints.Count - 1)
        {
            if (Vector3.Distance(transform.position, agent.destination) <= agent.stoppingDistance)
            {
                currentWaypointIndex++;
                UpdateWaypoint();
            }
        }
    }

    void UpdateWaypoint()
    {
        nextWaypoint = waypointSystem.waypoints[currentWaypointIndex];
        agent.SetDestination(nextWaypoint.position);
    }
}
