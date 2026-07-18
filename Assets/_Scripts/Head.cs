using UnityEngine;

public class Head : MonoBehaviour
{
    [Header("References")]
    public Spring spring;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 7)
        {
            Collider myColliderThatHit = collision.contacts[0].thisCollider;

            ContactPoint contact = collision.contacts[0];
            Vector3 bounceDirection = contact.normal;
            bounceDirection.z = 0f;
            bounceDirection.Normalize();

            spring.rb.linearVelocity = Vector3.zero;
            spring.rb.angularVelocity = Vector3.zero;
            spring.rb.AddForce(bounceDirection * 3, ForceMode.Impulse);
        }
    }
}
