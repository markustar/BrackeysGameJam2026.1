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
    public float DealingDamage;
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;

    [Header("Detection")]
    public float detectionMeter = 0f;
    public float detectionSpeed = 100f; // Increased for faster response
    public float detectionDecay = 20f;
    public float detectionThreshold = 100f;
    public float chaseMaxRange = 10f; // The "Chase Area" radius
    public string playerTag = "Player";
    public LayerMask obstacleMask;

    [Header("References")]
    public FieldOfView fov;

    [Header("Animation")]
    public EnemyAnimation enemyAnimation;

    private PathFollower agent;
    private int currentWaypointIndex = 0;
    private State currentState = State.Patrol;
    private Transform player;
    private Vector3 lastSeenPlayerPos;
    private bool isWaiting = false;
    private bool canSeePlayer = false;

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
            
            // PRIORITY 1: Stare at player if visible and detecting/chasing
            if (canSeePlayer && player != null)
            {
                Vector3 dirToPlayer = (player.position - transform.position).normalized;
                float targetAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, targetAngle), rotationSpeed * Time.deltaTime);
                
                // FOV points where the body is currently looking
                fov.SetAimDirection(transform.right);
            }
            // PRIORITY 2: Search at last known position if player lost during detection
            else if (detectionMeter > 0 && currentState != State.Chase && currentState != State.Attack)
            {
                Vector3 dirToLastPos = (lastSeenPlayerPos - transform.position).normalized;
                float targetAngle = Mathf.Atan2(dirToLastPos.y, dirToLastPos.x) * Mathf.Rad2Deg;
                
                // Rotate body towards last seen
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, targetAngle), (rotationSpeed * 0.5f) * Time.deltaTime);

                // Add a scanning offset relative to the body's CURRENT direction
                float scanAngle = 30f;
                float scanSpeed = 3f;
                float offset = Mathf.Sin(Time.time * scanSpeed) * scanAngle;
                
                fov.SetAimDirection(Quaternion.Euler(0, 0, offset) * transform.right);
                
                agent.isStopped = true;
            }
            // PRIORITY 3: Movement-based rotation
            else if (agent.velocity.sqrMagnitude > 0.01f)
            {
                Vector3 moveDir = agent.velocity;
                float targetAngle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, targetAngle), rotationSpeed * Time.deltaTime);
                
                fov.SetAimDirection(transform.right);
            }
            // PRIORITY 4: Idle/Patrol wait rotation
            else if (currentState == State.Idle)
            {
                float peakAngle = 45f;
                float cycleTime = 2f;
                float t = (Time.time % cycleTime) / cycleTime;
                float angleOffset = Mathf.PingPong(t * peakAngle * 4, peakAngle * 2) - peakAngle;

                fov.SetAimDirection(Quaternion.Euler(0, 0, angleOffset) * transform.right);
            }
            else
            {
                fov.SetAimDirection(transform.right);
            }
        }

        if (enemyAnimation != null && agent != null)
        {
            enemyAnimation.UpdateMovement(transform.right, agent.velocity.magnitude);
        }
    }

    private void HandleDetection()
    {
        if (player == null) return;

        canSeePlayer = fov != null && fov.IsPlayerInRange(player, playerTag);

        if (canSeePlayer)
        {
            detectionMeter += detectionSpeed * Time.deltaTime;
            
            // STOP AND STARE: If we haven't reached full detection, stop the agent
            if (detectionMeter < detectionThreshold && currentState != State.Chase && currentState != State.Attack)
            {
                agent.isStopped = true;
            }
        }
        else if (detectionMeter > 0)
        {
            detectionMeter -= detectionDecay * Time.deltaTime;
            
            // Resume patrolling if the player is lost during detection
            if (detectionMeter <= 0 && currentState == State.Patrol)
            {
                agent.isStopped = false;
            }
        }

        if (canSeePlayer && player != null)
        {
            lastSeenPlayerPos = player.position;
        }

        detectionMeter = Mathf.Clamp(detectionMeter, 0, detectionThreshold);

        if (fov != null)
        {
            fov.SetDetectionLevel(detectionMeter / detectionThreshold);
        }

        if (detectionMeter >= detectionThreshold)
        {
            if (currentState != State.Chase && currentState != State.Attack)
            {
                Debug.Log("[EnemyAI] Player Detected! Starting Chase.");
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
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // LEASH LOGIC: If player leaves the "Chase Area"
        if (distanceToPlayer > chaseMaxRange)
        {
            Debug.Log("[EnemyAI] Player left Chase Area. Returning to patrol.");
            detectionMeter = 0; // Immediate reset
            currentState = State.Patrol;
            agent.isStopped = false;
            SetDestinationToWaypoint();
            return;
        }

        agent.speed = chaseSpeed;
        agent.isStopped = false;
        agent.SetDestination(player.position);

        if (distanceToPlayer <= attackRange)
        {
            currentState = State.Attack;
            agent.isStopped = true;
        }
    }

    private void AttackBehavior()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle), rotationSpeed * Time.deltaTime);

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
        if (enemyAnimation != null)
        {
            enemyAnimation.TriggerAttack();
        }
        player.gameObject.GetComponent<PlayerHealth>().TakeDamage(DealingDamage);
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
