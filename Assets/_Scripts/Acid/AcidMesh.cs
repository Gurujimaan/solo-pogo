using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AcidMesh : MonoBehaviour
{
    [System.Serializable]
    public struct Spring
    {
        public float height;
        public float targetHeight;
        public float velocity;
    }

    [Header("Mesh Settings")]
    [SerializeField] private int columns = 40;
    [SerializeField] private float width = 10f;
    [SerializeField] private float poolDepth = 3f;

    [Header("Spring Physics")]
    [SerializeField] private float stiffness = 100f;
    [SerializeField] private float damping = 5f;
    [SerializeField] private float spread = 0.1f;
    [SerializeField] private int propagationPasses = 4;

    [Header("Idle Surface Wave")]
    [SerializeField] private float waveFrequency = 1.5f;
    [SerializeField] private float waveSpeed = 2f;
    [SerializeField] private float waveHeight = 0.08f;

    private Spring[] springs;
    private Mesh filterMesh;
    private MeshFilter meshFilter;

    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;

    // Cache propagation arrays to eliminate per-frame allocations
    private float[] leftDeltas;
    private float[] rightDeltas;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        InitSprings();
        BuildInitialMesh();
    }

    private void InitSprings()
    {
        int springCount = columns + 1;
        springs = new Spring[springCount];
        leftDeltas = new float[springCount];
        rightDeltas = new float[springCount];

        // Use local space: surface defaults to local Y = 0
        for (int i = 0; i < springCount; i++)
        {
            springs[i].height = 0f;
            springs[i].targetHeight = 0f;
            springs[i].velocity = 0f;
        }
    }

    private void BuildInitialMesh()
    {
        filterMesh = new Mesh();
        meshFilter.mesh = filterMesh;

        int vertexCount = (columns + 1) * 2;
        vertices = new Vector3[vertexCount];
        uvs = new Vector2[vertexCount];
        triangles = new int[columns * 6];

        float xStep = width / columns;
        float xOrigin = -width * 0.5f;

        for (int i = 0; i <= columns; i++)
        {
            float xPos = xOrigin + (i * xStep);
            float uvX = (float)i / columns;

            // Bottom vertex (Local space)
            vertices[i * 2] = new Vector3(xPos, -poolDepth, 0f);
            uvs[i * 2] = new Vector2(uvX, 0f);

            // Top vertex (Local space)
            vertices[i * 2 + 1] = new Vector3(xPos, 0f, 0f);
            uvs[i * 2 + 1] = new Vector2(uvX, 1f);
        }

        int t = 0;
        for (int i = 0; i < columns; i++)
        {
            int bLeft = i * 2;
            int tLeft = i * 2 + 1;
            int bRight = (i + 1) * 2;
            int tRight = (i + 1) * 2 + 1;

            triangles[t++] = bLeft;
            triangles[t++] = tLeft;
            triangles[t++] = tRight;

            triangles[t++] = bLeft;
            triangles[t++] = tRight;
            triangles[t++] = bRight;
        }

        filterMesh.vertices = vertices;
        filterMesh.uv = uvs;
        filterMesh.triangles = triangles;
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        UpdateSpringPhysics(dt);
        UpdateMeshVertices();
    }

    private void UpdateSpringPhysics(float dt)
    {
        // 1. Hooke's Law integration with deltaTime
        for (int i = 0; i <= columns; i++)
        {
            float x = springs[i].height - springs[i].targetHeight;
            float acceleration = -stiffness * x - damping * springs[i].velocity;

            springs[i].velocity += acceleration * dt;
            springs[i].height += springs[i].velocity * dt;
        }

        // 2. Neighbor Propagation (Re-using cached arrays)
        for (int pass = 0; pass < propagationPasses; pass++)
        {
            for (int i = 0; i <= columns; i++)
            {
                if (i > 0)
                {
                    leftDeltas[i] = spread * (springs[i].height - springs[i - 1].height);
                    springs[i - 1].velocity += leftDeltas[i];
                }
                if (i < columns)
                {
                    rightDeltas[i] = spread * (springs[i].height - springs[i + 1].height);
                    springs[i + 1].velocity += rightDeltas[i];
                }
            }

            for (int i = 0; i <= columns; i++)
            {
                if (i > 0) springs[i - 1].height += leftDeltas[i];
                if (i < columns) springs[i + 1].height += rightDeltas[i];
            }
        }
    }

    private void UpdateMeshVertices()
    {
        float time = Time.time * waveSpeed;

        for (int i = 0; i <= columns; i++)
        {
            // Apply sine wave displacement directly to the visual vertex height
            float xOffset = ((float)i / columns) * waveFrequency;
            float idleWave = Mathf.Sin(time + xOffset) * waveHeight;

            int topIndex = i * 2 + 1;
            vertices[topIndex].y = springs[i].height + idleWave;
        }

        filterMesh.vertices = vertices;
        filterMesh.RecalculateBounds();
    }

    /// <summary>
    /// Applies a velocity impulse to the surface spring nearest to worldX.
    /// </summary>

    public void Splash(float worldX, float force)
    {
        // Convert world X position into local pool space
        float localX = transform.InverseTransformPoint(new Vector3(worldX, 0f, 0f)).x;
        float xOrigin = -width * 0.5f;
        float relativeX = localX - xOrigin;

        if (relativeX < 0f || relativeX > width) return;

        int index = Mathf.RoundToInt((relativeX / width) * columns);
        index = Mathf.Clamp(index, 0, columns);

        springs[index].velocity += force;
    }
}