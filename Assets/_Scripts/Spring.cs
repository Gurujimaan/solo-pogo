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
            Vector2 normal = hit.normal;
            pivotPoint.position = hit.point;
            StartCoroutine(HandleBounceSequence(normal));
        }
        else pivotPoint.position = transform.position;
    }

    private IEnumerator HandleBounceSequence(Vector2 normal)
    {
        isCharging = true;
        floor = true;
        anim.Play("Jumping", 0, 0f);

        float momentum = Mathf.Clamp(rb.linearVelocity.magnitude, 0f, 6f);
        if (momentum < 0.8f) momentum = 0f;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        yield return new WaitForSeconds(0.12f);

        float finalJumpPower = baseJumpPower;

        if (controller.jumpInput)                                       // SuperJump
        {
            float chargeTimer = 0f;
            float angle = Vector3.Angle(normal, transform.up);
            while (controller.jumpInput && chargeTimer < maxJumpTime && angle < 60f)
            {
                chargeTimer += Time.deltaTime;
                rb.linearVelocity = Vector3.zero;
                angle = Vector3.Angle(normal, transform.up);

                finalJumpPower = Mathf.Lerp(baseJumpPower, maxJumpPower, chargeTimer / maxJumpTime);
                yield return null;
            }

            float addedMomentumForce = momentum / 3f;
            rb.AddForce(transform.up * (finalJumpPower + momentum), ForceMode.VelocityChange);
            jumpParticle.Play();
        }
        else                                                           //Normal Jump
        {
            float addedMomentumForce = momentum / 8f;
            rb.AddForce(transform.up * (baseJumpPower + addedMomentumForce), ForceMode.VelocityChange);
        }
        jumpTimer = 0;
        floor = false;
        isCharging = false;
        anim.Play("Release", 0, 0f);
    }

    private IEnumerator HandleReleaseSequence()
    {
        anim.Play("Release", 0, 0f);
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float durationInSeconds = stateInfo.length;
        Debug.Log(stateInfo.length);
        yield return new WaitForSeconds(durationInSeconds);
        anim.Play("Idle", 0, 0f);
    }


    private void OnDrawGizmos()
    {
        if (pogoTipOrigin == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(pogoTipOrigin.position, -transform.up * rayDistance);
    }
}