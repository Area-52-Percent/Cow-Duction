using System.Collections.Generic;
using UnityEngine;

public class WaypointSystem : MonoBehaviour
{
    public List<Transform> waypoints;

    // Start is called before the first frame update
    void Start()
    {
        InitializeWaypoints();
    }

    void InitializeWaypoints()
    {
        waypoints.Clear();
        
        foreach(Transform child in transform)
        {
            waypoints.Add(child);
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
                Gizmos.DrawLine(waypoints[i - 1].position, waypoints[i].position);
            }
        }
    }
}
