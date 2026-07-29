using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public GameObject player;

    [Header("Settings")]
    [Tooltip("The total horizontal width of your playable area in Unity units (wall to wall).")]
    [SerializeField] private float targetWidth = 10f;
    public float followSpeed = 2f;
    public Vector3 offset;

    private Rigidbody playerRb;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        UpdateCameraSize();
    }

    void Start()
    {
        playerRb = player.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        #if UNITY_EDITOR
        // Recalculates dynamically when resizing the Game view window in Editor
        UpdateCameraSize();
        #endif
    }

    public void UpdateCameraSize()
    {
        if (cam == null) cam = GetComponent<Camera>();

        // Ensure camera is in Orthographic mode (2D)
        if (!cam.orthographic)
        {
            cam.orthographic = true;
        }

        // Calculate size needed to keep horizontal width fixed
        float unitsPerPixel = targetWidth / Screen.width;
        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

        cam.orthographicSize = desiredHalfHeight;
    }

    void FixedUpdate()
    {
        Vector3 targetPosition = player.transform.position + offset;

        if(Mathf.Abs(targetPosition.magnitude - transform.position.magnitude) > 0.5f) 
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.fixedDeltaTime);
    }
}
