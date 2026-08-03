using UnityEngine;

public class RisingAcid : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Rising Settings")]
    public float riseSpeed = 0.5f;
    public bool isRising = false;


    void FixedUpdate()
    {
        if (!isRising) return;

        transform.position += Vector3.up * riseSpeed * Time.fixedDeltaTime;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.PlayerDeath();
        }
    }
}
