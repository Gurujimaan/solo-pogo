using UnityEngine;

public class Particle : MonoBehaviour
{
    public Animator animator;
    public float lifetime = 0.6f;

    private void OnEnable()
    {
        if (animator != null) animator.Play("Jump Particles");
        Invoke("Deactivate", lifetime);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
