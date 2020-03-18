﻿using System.Collections.Generic;
using UnityEngine;

public class WaypointSystem : MonoBehaviour
{
    [HideInInspector]
    public List<Vector3> waypoints;
    public WaypointFollower follower;
    public enum DrawMode {Never, Selected, Always};
    public DrawMode drawMode = DrawMode.Selected;

    private void OnValidate()
    {
        if (follower == null) follower = GetComponentInChildren<WaypointFollower>();
    }

    private void Awake()
    {
        InitializeWaypoints();
    }

    private void Start()
    {
        follower.enabled = false;
    }

    private void InitializeWaypoints()
    {
        waypoints.Clear();
        
        foreach(Transform child in transform)
        {
            waypoints.Add(child.position);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "UFO")
        {
            if (!follower.enabled) follower.enabled = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (drawMode != DrawMode.Always) return;

        DrawWaypoints();
    }

    private void OnDrawGizmosSelected()
    {
        if (drawMode == DrawMode.Never) return;

        DrawWaypoints();
    }

    private void DrawWaypoints()
    {
        if (transform.childCount != waypoints.Count || transform.position != waypoints[0])
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
