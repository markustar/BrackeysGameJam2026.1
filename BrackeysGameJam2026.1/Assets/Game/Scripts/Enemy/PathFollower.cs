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

        Vector3 direction = (path[currentWaypoint].worldPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        float distance = Vector3.Distance(transform.position, path[currentWaypoint].worldPosition);
        if (distance < nextWaypointDistance)
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

