using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private Transform PlayerTransform;
    private NavMeshPath path;
    private float elapsed = 0.0f;
    void Awake()
    {
        PlayerTransform = GameObject.FindWithTag("Player").transform;
    }
    void Start()
    {
        path = new NavMeshPath();
        elapsed = 0.0f;
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed > 0.5f)
        {
            elapsed -= 0.5f;
            NavMesh.CalculatePath(transform.position, PlayerTransform.position, NavMesh.AllAreas, path);
        }
        for (int i = 0; i < path.corners.Length - 1; i++)
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
    }
}
