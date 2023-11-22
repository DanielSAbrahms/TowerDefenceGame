using UnityEngine;
using System.Collections.Generic;

public class FollowGamePath : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    private float speed = 2f;
    private int currentWaypointIndex = 0;


    void Start() {
    }

    public void Initialize(float s) {
        speed = s;
    }

    void Update()
    {
        if (waypoints.Count == 0)
            return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];

        var targetOffset = new Vector3(targetWaypoint.position.x, 
            targetWaypoint.position.y + (transform.localScale.y / 2), 
            targetWaypoint.position.z);

        transform.position = Vector3.MoveTowards(transform.position, targetOffset, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetOffset) < 0.1f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                GameManager.Instance.EnemyCompletePath(gameObject);
            }
            }
    }
}
