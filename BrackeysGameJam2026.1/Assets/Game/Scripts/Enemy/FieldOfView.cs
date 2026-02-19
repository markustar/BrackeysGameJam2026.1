using UnityEngine;

public class FieldOfView : UnityEngine.MonoBehaviour
{
    [Header("Settings")]
    public float fov = 90f;
    public float viewDistance = 5f;
    public int rayCount = 50;
    public LayerMask layerMask; // Obstacles
    public LayerMask playerMask;

    private Mesh mesh;
    private Vector3 origin;
    private float startingAngle;
    private Vector3 currentAimDirection;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        origin = Vector3.zero;
    }

    private void LateUpdate()
    {
        int rayCountLocal = rayCount;
        float angle = startingAngle;
        float angleIncrease = fov / rayCountLocal;

        Vector3[] vertices = new Vector3[rayCountLocal + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCountLocal * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        Quaternion inverseRotation = Quaternion.Inverse(transform.rotation);

        for (int i = 0; i <= rayCountLocal; i++)
        {
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, GetVectorFromAngle(angle), viewDistance, layerMask);

            if (raycastHit2D.collider == null)
            {
                vertex = origin + GetVectorFromAngle(angle) * viewDistance;
            }
            else
            {
                vertex = (Vector3)raycastHit2D.point - transform.position;
            }

            // Convert world-space vertex to local-space
            vertices[vertexIndex] = inverseRotation * vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.bounds = new Bounds(origin, Vector3.one * 1000f);
    }

    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }

    public void SetAimDirection(Vector3 aimDirection)
    {
        currentAimDirection = aimDirection.normalized;
        startingAngle = GetAngleFromVectorFloat(currentAimDirection) + fov / 2f;
    }

    private Vector3 GetVectorFromAngle(float angle)
    {
        // angle = 0 -> 360
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    private float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    public bool IsPlayerInRange(Transform playerTransform, string playerTag)
    {
        Vector3 playerPos = playerTransform.position;
        // 2D logic: ensure Z is same as transform
        playerPos.z = transform.position.z;

        Vector3 dirToPlayer = (playerPos - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(currentAimDirection != Vector3.zero ? currentAimDirection : transform.right, dirToPlayer);

        if (angleToPlayer < fov / 2f)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerPos);
            if (distanceToPlayer <= viewDistance)
            {
                // Combine masks to ensure we can hit both obstacles AND the player
                LayerMask combinedMask = layerMask | playerMask;

                // Offset the raycast slightly to avoid self-intersection with enemy collider
                Vector2 rayOrigin = (Vector2)transform.position + ((Vector2)dirToPlayer * 0.2f);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, dirToPlayer, distanceToPlayer, combinedMask);

                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag(playerTag))
                    {
                        Debug.DrawRay(rayOrigin, dirToPlayer * distanceToPlayer, Color.green);
                        return true;
                    }
                    else
                    {
                        // Ray hit something else (an obstacle) before hitting the player
                        Debug.DrawRay(rayOrigin, dirToPlayer * hit.distance, Color.red);
                    }
                }
                else
                {
                    // Should theoretically not happen if combinedMask includes player and we are in range
                    Debug.DrawRay(rayOrigin, dirToPlayer * distanceToPlayer, Color.yellow);
                }
            }
        }
        return false;
    }
}
