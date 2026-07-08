using System.Collections;
using UnityEngine;

public class Spring : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;

    private GameObject floor;

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor") && floor == null)
        {
            floor = other.gameObject;

            float momentum = playerController.rb.linearVelocity.magnitude;

            //Debug.Log(momentum);

            playerController.rb.linearVelocity = Vector3.zero;
            playerController.rb.angularVelocity = Vector3.zero;
            yield return new WaitForSeconds(0.15f); // Wait for a short duration before applying the jump force

            playerController.rb.AddForce(transform.up * playerController.baseJumpPower, ForceMode.Impulse);

            floor = null;
        }

        yield return null;
    }
}
