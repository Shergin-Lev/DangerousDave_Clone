using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float fallGravityMultiplier = 2.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private bool isGrounded;

    private PlayerInputAction inputActions;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private bool isFacingRight = true;
    private PlayerShooting playerShooting;

    private void Awake()
    {
        // �������� ����������
        rb = GetComponent<Rigidbody2D>();

        // ������ ������ Input Actions
        inputActions = new PlayerInputAction();

        playerShooting = GetComponent<PlayerShooting>();
    }

    private void OnEnable()
    {
        // �������� Input Actions
        inputActions.Enable();

        // ������������� �� ������� Move
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Jump.started += OnJump;
        inputActions.Player.Jump.canceled += OnJump;
    }

    private void OnDisable()
    {
        // ������������ �� ������� Move
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Jump.started -= OnJump;
        inputActions.Player.Jump.canceled -= OnJump;

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
        if (context.started && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        else if (context.canceled && rb.linearVelocity.y > 0) // ��������� Space �� ����� �����
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f); // �������� ������
        }
    }

    private void FlipSprite()
    {
        if (moveInput.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        // �������������� ������ �� ��� X
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        // �������� ������� �������� � �����������
        if (playerShooting != null)
        {
            playerShooting.SetFacingDirection(isFacingRight);
        }
    }

    private void FixedUpdate()
    {
        // ��������, �� ����� �� �����
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // ��������� �������������� ���������� ��� �������
        if (rb.linearVelocityY < 0) // ���� ������ ����
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallGravityMultiplier - 1) * Time.fixedDeltaTime;

        // ��������� �������� ����� ������
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocityY);

        FlipSprite();
    }
}
