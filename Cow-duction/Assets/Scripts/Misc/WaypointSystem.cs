using System.Collections.Generic;
using UnityEngine;

public class WaypointSystem : MonoBehaviour
{
    public List<Vector3> waypoints;

    private void Awake()
    {
        InitializeWaypoints();
    }

    private void InitializeWaypoints()
    {
        waypoints.Clear();
        
        foreach(Transform child in transform)
        {
            waypoints.Add(child.position);
        }
    }

    private void OnDrawGizmos()
    {
        if (transform.childCount != waypoints.Count)
            InitializeWaypoints();

        Gizmos.color = Color.cyan;
        if (waypoints.Count > 1)
        {
            for (int i = 1; i < waypoints.Count; i++)
            {
                Gizmos.DrawLine(waypoints[i - 1], waypoints[i]);
            }
        }
    }
}
