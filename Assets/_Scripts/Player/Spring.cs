using System.Collections;
using UnityEngine;

public class Spring : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public PlayerController controller;
    public Animator anim;
    public Transform pogoTipOrigin;
    public Transform pivotPoint;
    public ParticleSystem jumpParticle;

    [Header("Bounce Settings")]
    public float rayDistance = 0.4f;
    public LayerMask floorLayer;
    public LayerMask falseFloorLayer;
    public float baseJumpPower = 12f;
    public float maxJumpPower = 22f;
    public float maxJumpTime = 0.5f;
    public float jumpCooldown = 0.2f;

    private float jumpTimer = 0f;
    private bool floor = false;
    [HideInInspector] public bool isCharging;
    private LayerMask groundLayer;

    void Update()
    {
        jumpTimer += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (isCharging || floor || jumpTimer < jumpCooldown) return;

        float predictiveDistance = rayDistance + rb.linearVelocity.magnitude * Time.fixedDeltaTime;
        Ray ray = new Ray(pogoTipOrigin.position, -transform.up);

        if (rb.linearVelocity.y > 0)
            groundLayer = floorLayer;
        else 
            groundLayer = falseFloorLayer;

        if (Physics.Raycast(ray, out RaycastHit hit, predictiveDistance, groundLayer))
        {
            pivotPoint.position = hit.point;
            StartCoroutine(HandleBounceSequence(hit));
        }
        else
        {
            pivotPoint.position = transform.position;
        }
    }

    private IEnumerator HandleBounceSequence(RaycastHit hit)
    {
        Vector2 normal = hit.normal;
        floor = true;
        isCharging = true;

        if (anim != null) anim.Play("Jumping", 0, 0f);

        float momentum = Mathf.Clamp(rb.linearVelocity.magnitude, 0f, 6f);
        if (momentum < 0.8f) momentum = 0f;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (hit.distance > rayDistance)
        {
            rb.MovePosition(hit.point + (transform.up * rayDistance * 2));
        }
        float finalJumpPower = baseJumpPower;

        if (controller != null && controller.jumpInput) // SuperJump
        {
            float chargeTimer = 0f;
            float angle = Vector3.Angle(normal, transform.up);
            rb.linearVelocity = Vector3.zero;
            rb.useGravity = false;

            while (controller.jumpInput && chargeTimer < maxJumpTime && angle < 60f)
            {
                chargeTimer += Time.deltaTime;
                angle = Vector3.Angle(normal, transform.up);

                rb.MovePosition(rb.position - (transform.up * Time.deltaTime) * 0.15f);

                finalJumpPower = Mathf.Lerp(baseJumpPower, maxJumpPower, chargeTimer / maxJumpTime);
                yield return null;
            }

            rb.useGravity = true;
            float addedMomentumForce = momentum / 3f;
            rb.AddForce(transform.up * (finalJumpPower + addedMomentumForce), ForceMode.VelocityChange);

            if (jumpParticle != null) jumpParticle.Play();
        }
        else // Normal Jump
        {
            yield return new WaitForSeconds(0.15f);
            rb.AddForce(transform.up * baseJumpPower, ForceMode.VelocityChange);
        }

        jumpTimer = 0f;
        floor = false;
        isCharging = false;

        if (anim != null) anim.Play("Release", 0, 0f);
    }

    private void OnDrawGizmos()
    {
        if (pogoTipOrigin == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(pogoTipOrigin.position, -transform.up * rayDistance);
    }
}