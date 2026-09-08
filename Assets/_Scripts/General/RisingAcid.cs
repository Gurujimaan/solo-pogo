using UnityEngine;

public class RisingAcid : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Rising Settings")]
    private float riseSpeed = 0.5f;
    public float baseRiseSpeed = 0.5f;
    public float fastRiseDistance = 5.0f;
    public bool isRising = false;


    void FixedUpdate()
    {
        if (!isRising) return;

        if (player.position.y - transform.position.y > fastRiseDistance)
        {
            riseSpeed += 0.1f;
        }
        else
        {
            riseSpeed = baseRiseSpeed;
        }

        transform.position += Vector3.up * riseSpeed * Time.fixedDeltaTime;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.PlayerDeath();
        }
    }


    public void ResetAcid()
    {
        isRising = false;
        transform.position = new Vector3(transform.position.x, -7, transform.position.z);
    }
}
