using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// This main script handles player movement and input. It is attached to the player GameObject in the Unity scene.
    /// </summary>

    [Header("References")]
    public Rigidbody rb;
    public Animator anim;
    public PlayerInput playerInput;
    public Spring spring;

    [Header("Movement Settings")]
    public float rotSpeed;

    private InputAction rotateLeftAction;
    private InputAction rotateRightAction;
    private InputAction jumpAction;

    [HideInInspector] public float rotLeftInput;
    [HideInInspector] public float rotRightInput;
    [HideInInspector] public bool jumpInput;

    float noInputTime;

    // Update is called once per frame
    void Update()
    {
        Rotate();
        AutoTurn();
    }

    void Rotate()
    {
        if (rotLeftInput - rotRightInput == 0) return;

        float rotationAmount = (rotLeftInput - rotRightInput) * rotSpeed * Time.deltaTime;

        //transform.Rotate(0f, 0f, rotationAmount);
        transform.RotateAround(spring.pivotPoint.position, Vector3.forward, rotationAmount);
    }

    private void AutoTurn()
    {
        if (rotLeftInput == 0 && rotRightInput == 0 && !jumpInput) noInputTime += Time.deltaTime;
        else noInputTime = 0f;

        if (noInputTime >= 2f && Mathf.Abs(transform.rotation.z) < 0.18)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, rotSpeed/10 * Time.deltaTime);
        }
    }

    #region Input
    private void OnEnable()
    {
        rotateLeftAction = playerInput.actions["Left"];
        rotateRightAction = playerInput.actions["Right"];
        jumpAction = playerInput.actions["Jump"];

        rotateLeftAction.performed += RotateLeft;
        rotateLeftAction.canceled += RotateLeft;

        rotateRightAction.performed += RotateRight;
        rotateRightAction.canceled += RotateRight;

        jumpAction.performed += context => jumpInput = true;
        jumpAction.canceled += context => jumpInput = false;
    }

    private void OnDisable()
    {
        rotateLeftAction.performed -= RotateLeft;
        rotateLeftAction.canceled -= RotateLeft;
        rotateRightAction.performed -= RotateRight;
        rotateRightAction.canceled -= RotateRight;
        jumpAction.performed -= context => jumpInput = true;
        jumpAction.canceled -= context => jumpInput = false;
    }

    private void RotateLeft(InputAction.CallbackContext context)
    {
        rotLeftInput = context.ReadValue<float>();
    }

    private void RotateRight(InputAction.CallbackContext context)
    {
        rotRightInput = context.ReadValue<float>();
    }
    #endregion
}
