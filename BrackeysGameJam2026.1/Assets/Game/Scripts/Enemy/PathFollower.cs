using UnityEngine;
using System.Collections.Generic;

public class PathFollower : MonoBehaviour
{
    public float speed = 5f;
    public float nextWaypointDistance = 0.2f;
    public bool isStopped = false;

    private List<AStarPathfinding.Node> path;
    private int currentWaypoint = 0;
    private Vector3 targetPosition;

    public void SetDestination(Vector3 target)
    {
        targetPosition = target;
        path = AStarPathfinding.Instance.FindPath(transform.position, targetPosition);
        currentWaypoint = 0;
    }

    private void Update()
    {
        if (isStopped || path == null || currentWaypoint >= path.Count) return;

        Vector3 targetWaypoint = path[currentWaypoint].worldPosition;
        Vector3 direction = (targetWaypoint - transform.position);
        float distanceToWaypoint = direction.magnitude;

        // Push Out Logic: If we are overlapping an obstacle, nudge away from it
        Collider2D overlap = Physics2D.OverlapPoint(transform.position, AStarPathfinding.Instance.obstacleMask);
        if (overlap != null)
        {
            Vector2 pushOutDir = (Vector2)(transform.position - overlap.bounds.center).normalized;
            if (pushOutDir == Vector2.zero) pushOutDir = Random.insideUnitCircle.normalized; // Fallback
            transform.position += (Vector3)pushOutDir * speed * 0.5f * Time.deltaTime;
        }

        // Move towards waypoint
        if (distanceToWaypoint > 0.01f)
        {
            transform.position += direction.normalized * speed * Time.deltaTime;
        }

        if (distanceToWaypoint < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    public float remainingDistance
    {
        get
        {
            if (path == null || currentWaypoint >= path.Count) return 0;
            return Vector3.Distance(transform.position, targetPosition);
        }
    }

    public Vector3 velocity
    {
        get
        {
            if (path == null || currentWaypoint >= path.Count || isStopped) return Vector3.zero;
            return (path[currentWaypoint].worldPosition - transform.position).normalized * speed;
        }
    }
}

