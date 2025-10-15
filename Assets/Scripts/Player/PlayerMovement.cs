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
        // Получаем компоненты
        rb = GetComponent<Rigidbody2D>();

        // Создаём объект Input Actions
        inputActions = new PlayerInputAction();

        playerShooting = GetComponent<PlayerShooting>();
    }

    private void OnEnable()
    {
        // Включаем Input Actions
        inputActions.Enable();

        // Подписываемся на событие Move
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Jump.started += OnJump;
        inputActions.Player.Jump.canceled += OnJump;
    }

    private void OnDisable()
    {
        // Отписываемся от событий Move
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Jump.started -= OnJump;
        inputActions.Player.Jump.canceled -= OnJump;

        // Выключаем Input Actions
        inputActions.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // Получаем значение от Input System
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        else if (context.canceled && rb.linearVelocity.y > 0) // Отпустили Space во время взлёта
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f); // Обрезаем прыжок
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

        // Переворачиваем спрайт по оси X
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        // Сообщаем системе стрельбы о направлении
        if (playerShooting != null)
        {
            playerShooting.SetFacingDirection(isFacingRight);
        }
    }

    private void FixedUpdate()
    {
        // Проверям, на земле ли игрок
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Применяем дополнительную гравитацию при падении
        if (rb.linearVelocityY < 0) // Если падаем вниз
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallGravityMultiplier - 1) * Time.fixedDeltaTime;

        // Применяем движение через физику
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocityY);

        FlipSprite();
    }
}
