using UnityEngine;
using System.Collections;


public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        Patrol,
        Idle,
        Chase,
        Attack
    }

    [Header("Navigation")]
    public PatrolPath patrolPath;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float waypointWaitTime = 2f;
    public float stopDistance = 0.5f;
    public float rotationSpeed = 360f;

    [Header("Combat")]
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;

    [Header("Detection")]
    public float detectionMeter = 0f;
    public float detectionSpeed = 50f; // Percent per second
    public float detectionDecay = 20f;
    public float detectionThreshold = 100f;
    public string playerTag = "Player";
    public LayerMask obstacleMask;

    [Header("References")]
    public FieldOfView fov;

    private PathFollower agent;
    private int currentWaypointIndex = 0;
    private State currentState = State.Patrol;
    private Transform player;
    private bool isWaiting = false;

    private void Start()
    {
        agent = GetComponent<PathFollower>();
        player = GameObject.FindGameObjectWithTag(playerTag)?.transform;

        if (player == null)
        {
            Debug.LogError($"[EnemyAI] Player with tag '{playerTag}' NOT found in scene!");
        }

        if (patrolPath != null && patrolPath.waypoints.Length > 0)
        {
            SetDestinationToWaypoint();
        }
    }

    private void Update()
    {
        HandleDetection();

        switch (currentState)
        {
            case State.Patrol:
                PatrolBehavior();
                break;
            case State.Idle:
                // Just waiting
                break;
            case State.Chase:
                ChaseBehavior();
                break;
            case State.Attack:
                AttackBehavior();
                break;
        }

        // Update FOV orientation
        if (fov != null)
        {
            fov.SetOrigin(Vector3.zero);
            Vector3 moveDir = agent.velocity;

            if (moveDir.sqrMagnitude > 0.01f)
            {
                float targetAngle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

                // Rotate the body smoothly
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // CRITICAL: Point the FOV EXACTLY where we are moving, regardless of body lag
                fov.SetAimDirection(moveDir);
            }
            else if (currentState == State.Idle)
            {
                // Predictable idle rotation: Look left, then right
                float peakAngle = 45f;
                float cycleTime = 2f;
                float t = (Time.time % cycleTime) / cycleTime;
                float angleOffset = Mathf.PingPong(t * peakAngle * 4, peakAngle * 2) - peakAngle;

                Quaternion idleRotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + angleOffset);
                // Note: We don't want to permanently rotate the transform here as it messes up the base direction
                // Instead, we just point the FOV
                fov.SetAimDirection(Quaternion.Euler(0, 0, angleOffset) * transform.right);
            }
            else
            {
                fov.SetAimDirection(transform.right);
            }
        }
    }

    private void HandleDetection()
    {
        if (player == null) return;

        bool canSeePlayer = fov != null && fov.IsPlayerInRange(player, playerTag);

        if (canSeePlayer)
        {
            detectionMeter += detectionSpeed * Time.deltaTime;
        }
        else
        {
            detectionMeter -= detectionDecay * Time.deltaTime;
        }

        detectionMeter = Mathf.Clamp(detectionMeter, 0, detectionThreshold);

        if (detectionMeter >= detectionThreshold)
        {
            if (currentState != State.Chase && currentState != State.Attack)
            {
                currentState = State.Chase;
                StopAllCoroutines();
                isWaiting = false;
            }
        }
        else if (detectionMeter <= 0 && (currentState == State.Chase || currentState == State.Attack))
        {
            currentState = State.Patrol;
            agent.isStopped = false;
            SetDestinationToWaypoint();
        }
    }

    private void PatrolBehavior()
    {
        if (isWaiting) return;

        if (agent.remainingDistance < stopDistance)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    private void ChaseBehavior()
    {
        agent.speed = chaseSpeed;
        agent.isStopped = false;
        agent.SetDestination(player.position);

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            currentState = State.Attack;
            agent.isStopped = true;
        }
    }

    private void AttackBehavior()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle), 500f * Time.deltaTime);

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }

        if (Vector3.Distance(transform.position, player.position) > attackRange * 1.2f)
        {
            currentState = State.Chase;
            agent.isStopped = false;
        }
    }

    private void PerformAttack()
    {
        Debug.Log("Enemy Attacks Player!");
    }

    private IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        currentState = State.Idle;
        agent.isStopped = true;

        yield return new WaitForSeconds(waypointWaitTime);

        currentWaypointIndex = (currentWaypointIndex + 1) % patrolPath.waypoints.Length;
        SetDestinationToWaypoint();

        agent.isStopped = false;
        currentState = State.Patrol;
        isWaiting = false;
    }

    private void SetDestinationToWaypoint()
    {
        if (patrolPath == null || patrolPath.waypoints.Length == 0) return;

        agent.speed = patrolSpeed;
        agent.SetDestination(patrolPath.waypoints[currentWaypointIndex].position);
    }
}
