using UnityEngine;

public class Head : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            Collider myColliderThatHit = collision.contacts[0].thisCollider;
            if (myColliderThatHit.gameObject.name != "Head") return;

            ContactPoint contact = collision.contacts[0];
            Vector3 bounceDirection = contact.normal;
            bounceDirection.z = 0f;
            bounceDirection.Normalize();

            playerController.rb.linearVelocity = Vector3.zero;
            playerController.rb.angularVelocity = Vector3.zero;
            playerController.rb.AddForce(bounceDirection * 3, ForceMode.Impulse);
        }
    }
}
