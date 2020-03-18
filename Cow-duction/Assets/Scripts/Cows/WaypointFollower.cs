using UnityEngine;
using UnityEngine.AI;

public class WaypointFollower : MonoBehaviour
{
    public NavMeshAgent agent;
    public WaypointSystem waypointSystem;
    public Vector3 nextWaypoint;
    public int currentWaypointIndex = 0;
    public float precision = .5f;

    private void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = precision;

        UpdateWaypoint();
    }

    private void Update()
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

    private void UpdateWaypoint()
    {
        nextWaypoint = waypointSystem.waypoints[currentWaypointIndex];
        agent.SetDestination(nextWaypoint);
    }
}
