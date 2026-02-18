using UnityEngine;

public class FieldOfView : UnityEngine.MonoBehaviour
{
    [Header("Settings")]
    public float fov = 90f;
    public float viewDistance = 5f;
    public int rayCount = 50;
    public LayerMask layerMask;

    private Mesh mesh;
    private Vector3 origin;
    private float startingAngle;

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
        for (int i = 0; i <= rayCountLocal; i++)
        {
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, GetVectorFromAngle(angle), viewDistance, layerMask);

            if (raycastHit2D.collider == null)
            {
                // No hit
                vertex = origin + GetVectorFromAngle(angle) * viewDistance;
            }
            else
            {
                // Hit object
                vertex = (Vector3)raycastHit2D.point - transform.position;
            }
            vertices[vertexIndex] = vertex;

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
        startingAngle = GetAngleFromVectorFloat(aimDirection) + fov / 2f;
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
        Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.right, dirToPlayer);

        if (angleToPlayer < fov / 2f)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= viewDistance)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer, distanceToPlayer, layerMask);
                if (hit.collider != null && hit.collider.CompareTag(playerTag))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
