using UnityEngine;

public class Head : MonoBehaviour
{
    [Header("References")]
    public Spring spring;
    public Collider headCollider;

    private void OnCollisionEnter(Collision collision)
    {
        if (spring.isCharging) return;

        if (collision.gameObject.layer == 7)
        {
            Collider myColliderThatHit = collision.contacts[0].thisCollider;

            ContactPoint contact = collision.contacts[0];
            Vector3 bounceDirection = contact.normal;
            bounceDirection.z = 0f;
            bounceDirection.Normalize();

            spring.rb.linearVelocity = Vector3.zero;
            spring.rb.angularVelocity = Vector3.zero;
            spring.rb.AddForce(bounceDirection * 1.5f, ForceMode.Impulse);
        }
    }
}
