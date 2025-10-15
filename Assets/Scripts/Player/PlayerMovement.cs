using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private bool isGrounded;

    private PlayerInputAction inputActions;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        // �������� ����������
        rb = GetComponent<Rigidbody2D>();

        // ������ ������ Input Actions
        inputActions = new PlayerInputAction();
    }

    private void OnEnable()
    {
        // �������� Input Actions
        inputActions.Enable();

        // ������������� �� ������� Move
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        // ������������ �� ������� Move
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Jump.performed -= OnJump;

        // ��������� Input Actions
        inputActions.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // �������� �������� �� Input System
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void FixedUpdate()
    {
        // ��������, �� ����� �� �����
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // ��������� �������� ����� ������
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocityY);
    }
}
