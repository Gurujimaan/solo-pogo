using System.Collections;
using UnityEngine;

public class Spring : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            playerController.rb.linearVelocity = Vector3.zero; // Reset the player's velocity to zero
            yield return new WaitForSeconds(0.1f); // Wait for a short duration before applying the jump force

            playerController.rb.AddForce(transform.up * playerController.baseJumpPower, ForceMode.Impulse);
        }

        yield return null;
    }
}
