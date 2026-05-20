using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayer : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float jumpForce = 5f;

    [Header("Camera")]
    public Transform cameraTransform;
    public float mouseSensitivity = 0.2f;
    public float minPitch = -60f;
    public float maxPitch = 70f;

    private float _cameraPitch = 0f;

    private Vector2 _moveInput;
    private Vector2 _lookInput;

    private Rigidbody rb;
    private Animator animator;

    private bool isGrounded = true;
    private bool isRunning = false;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();
        RotatePlayer();
        RotateCamera();
        UpdateAnimations();
    }

    void HandleMovement()
    {
        float x = _moveInput.x;
        float y = _moveInput.y;

        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 move = new Vector3(x, 0, y);

        transform.Translate(
            move * currentSpeed * Time.deltaTime,
            Space.Self
        );
    }

    void RotatePlayer()
    {
        transform.Rotate(
            0,
            _lookInput.x * mouseSensitivity,
            0
        );
    }

    void RotateCamera()
    {
        if (cameraTransform == null) return;

        _cameraPitch -= _lookInput.y * mouseSensitivity;

        _cameraPitch = Mathf.Clamp(
            _cameraPitch,
            minPitch,
            maxPitch
        );

        cameraTransform.localRotation =
            Quaternion.Euler(_cameraPitch, 0f, 0f);
    }

    void UpdateAnimations()
    {
        if (animator == null) return;

        float x = _moveInput.x;
        float y = _moveInput.y;

        Vector2 movementInput = new Vector2(x, y);

        animator.SetFloat("VelX", x);
        animator.SetFloat("VelY", y);

        animator.SetFloat(
            "Blend",
            movementInput.magnitude
        );

        animator.SetBool(
            "IsJumping",
            !isGrounded
        );

        animator.SetBool(
            "IsRunning",
            isRunning
        );
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isRunning = true;
        }

        if (context.canceled)
        {
            isRunning = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                0,
                rb.linearVelocity.z
            );

            rb.AddForce(
                Vector3.up * jumpForce,
                ForceMode.Impulse
            );

            isGrounded = false;

            animator.SetBool(
                "IsJumping",
                true
            );
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;

        animator.SetBool(
            "IsJumping",
            false
        );
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;

        animator.SetBool(
            "IsJumping",
            true
        );
    }
}