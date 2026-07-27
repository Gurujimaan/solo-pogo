using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public GameObject player;

    [Header("Settings")]
    public float followSpeed = 2f;
    public Vector3 offset;
    public float size = 6f;
    public float fullSize = 10f;
    public float zoomSpeed = 2f;
    public float zoomReqSpeed = 2f;    //Player speed needed to zoom out

    private Rigidbody playerRb;

    void Start()
    {
        playerRb = player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetPosition = player.transform.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.fixedDeltaTime);

        if(playerRb.linearVelocity.magnitude > zoomReqSpeed)
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, fullSize, zoomSpeed * Time.fixedDeltaTime);
        }
        else
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, size, zoomSpeed * Time.fixedDeltaTime);
        }
    }
}
