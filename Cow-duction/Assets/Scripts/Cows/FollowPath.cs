using UnityEngine;
using UnityEngine.AI;

public class FollowPath : MonoBehaviour
{
    public WaypointSystem waypointSystem;
    public Transform nextWaypoint;
    public int currentWaypointIndex = 0;
    public NavMeshAgent agent;
    public bool pathFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();

        UpdateWaypoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWaypointIndex < waypointSystem.waypoints.Capacity - 1)
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
