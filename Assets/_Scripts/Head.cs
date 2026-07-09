using UnityEngine;

public class Head : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 bounceDirection = contact.normal;
            bounceDirection.z = 0f;
            bounceDirection.Normalize();

            playerController.rb.AddForce(bounceDirection * 2, ForceMode.Impulse);
            Debug.Log("Bounce applied to player!");
        }
    }
}
