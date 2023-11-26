using System.Collections.Generic;
using UnityEngine;

public class FollowGamePath : MonoBehaviour
{
    public List<Transform> waypoints = new();
    private int currentWaypointIndex;
    private float speed = 2f;
    

    private void Update()
    {
        if (waypoints.Count == 0)
            return;

        var targetWaypoint = waypoints[currentWaypointIndex];

        var targetOffset = new Vector3(targetWaypoint.position.x,
            targetWaypoint.position.y + transform.localScale.y / 2,
            targetWaypoint.position.z);

        transform.position = Vector3.MoveTowards(transform.position, targetOffset, speed * Time.deltaTime);
        transform.LookAt(targetOffset);
        
        if (Vector3.Distance(transform.position, targetOffset) < 0.1f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count) GameManager.Instance.EnemyCompletePath(gameObject);
        }
    }

    public void Initialize(float newSpeed)
    {
        speed = newSpeed;
    }
    
    public float CalculateTotalDistance()
    {
        float totalDistance = 0f;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            totalDistance += Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
        }

        return totalDistance;
    }

    public float CalculateTraveledDistance()
    {
        float traveledDistance = 0f;
        int nearestWaypointIndex = currentWaypointIndex;

        // Sum distance from start to the nearest waypoint
        for (int i = 0; i < nearestWaypointIndex; i++)
        {
            traveledDistance += Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
        }

        // Add distance from the nearest waypoint to the GameObject
        traveledDistance += Vector3.Distance(this.transform.position, waypoints[nearestWaypointIndex].position);

        return traveledDistance;
    }
}