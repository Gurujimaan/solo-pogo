using System.Collections;
using UnityEngine;

public class Spring : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public PlayerController controller;
    public Transform pogoTipOrigin; 

    [Header("Bounce Settings")]
    public float rayDistance = 0.6f;
    public LayerMask groundLayer;
    public float baseJumpPower = 12f;
    public float maxJumpPower = 22f;
    public float maxJumpTime = 0.5f;
    public float jumpCooldown = 0.2f;

    private float jumpTimer = 0f;
    private bool floor = false;
    private bool isCharging;

    void FixedUpdate()
    {
        jumpTimer += Time.fixedDeltaTime;
        if (isCharging || floor || jumpTimer < jumpCooldown) return;

        Ray ray = new Ray(pogoTipOrigin.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, groundLayer))
        {
            StartCoroutine(HandleBounceSequence());
        }
    }

    private IEnumerator HandleBounceSequence()
    {
        isCharging = true;
        floor = true;
        jumpTimer = 0;

        float momentum = Mathf.Clamp(rb.linearVelocity.magnitude, 0f, 15f);
        if (momentum < 0.8f) momentum = 0f;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        yield return new WaitForSeconds(0.12f);

        float finalJumpPower = baseJumpPower;

        if (controller.jumpInput)                                       // SuperJump
        {
            float chargeTimer = 0f;
            while (controller.jumpInput && chargeTimer < maxJumpTime)
            {
                chargeTimer += Time.deltaTime;
                rb.linearVelocity = Vector3.zero;

                finalJumpPower = Mathf.Lerp(baseJumpPower, maxJumpPower, chargeTimer / maxJumpTime);
                yield return null;
            }

            float addedMomentumForce = momentum / 3f;
            rb.AddForce(transform.up * (finalJumpPower + addedMomentumForce), ForceMode.VelocityChange);
        }
        else                                                           //Normal Jump
        {
            float addedMomentumForce = momentum / 10f;
            rb.AddForce(transform.up * (baseJumpPower + addedMomentumForce), ForceMode.VelocityChange);
        }

        floor = false;
        isCharging = false;
    }

    private void OnDrawGizmos()
    {
        if (pogoTipOrigin == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(pogoTipOrigin.position, -transform.up * rayDistance);
    }
}