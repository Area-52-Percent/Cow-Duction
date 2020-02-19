using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaypointSystem : MonoBehaviour
{
    public List<Transform> waypoints;

    // Start is called before the first frame update
    void Start()
    {
        if (waypoints.Capacity > 0)
            waypoints.Clear();
        
        foreach(Transform child in transform)
        {
            waypoints.Add(child);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (waypoints.Capacity > 0)
            waypoints.Clear();
        
        foreach(Transform child in transform)
        {
            waypoints.Add(child);
        }

        Gizmos.color = Color.cyan;
        if (waypoints.Capacity > 0)
        {
            for (int i = 0; i < waypoints.Capacity - 1; i++)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
    }
#endif
}
