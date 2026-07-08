using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// This main script handles player movement and input. It is attached to the player GameObject in the Unity scene.
    /// </summary>

    [Header("References")]
    public Rigidbody rb;
    public PlayerInput playerInput;
    public Spring spring;

    [Header("Movement Settings")]
    public float baseJumpPower;
    public float rotSpeed;

    private InputAction rotateLeftAction;
    private InputAction rotateRightAction;
    private InputAction jumpAction;

    [HideInInspector] public float rotLeftInput;
    [HideInInspector] public float rotRightInput;
    [HideInInspector] public bool jumpInput;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
    }

    void Rotate()
    {
        if (rotLeftInput - rotRightInput == 0) return;

        float rotationAmount = (rotLeftInput - rotRightInput) * rotSpeed * Time.deltaTime;

        transform.Rotate(0f, 0f, rotationAmount);
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
