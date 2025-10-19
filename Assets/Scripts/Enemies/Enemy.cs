using System.Data;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRange = 10f;

    [Header("Ground & Wall Detection")]
    [SerializeField] private Transform groundCheck; // Точка проверки земли впереди
    [SerializeField] private Transform wallCheck;   // Точка проперки стены
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.5f;
    [SerializeField] private float wallCheckDistance = 0.3f;

    [Header("Patrol")]
    [SerializeField] private bool shouldPatrol = true;
    [SerializeField] private float patrolPauseTime = 2f; // Время паузы перед разворотом

    [Header("Death Effects")]
    [SerializeField] private GameObject deathEffectPrefab;

    private Health health;
    private Transform player;
    private Rigidbody2D rb;
    private bool isFacingRight = true;

    // AI States
    private enum EnemyState { Patrol, Chase }
    private EnemyState currentState = EnemyState.Patrol;
    private float patrolTimer;
    private bool isPaused;

    private void Awake()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();

        // Находим игрока по тегу
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        patrolTimer = patrolPauseTime;
    }

    private void OnEnable()
    {
        // Подписываемся на событие смерти
        health.OnDeath.AddListener(OnDeath);
    }

    private void OnDisable()
    {
        // Отписываемся от события смерти
        health.OnDeath.RemoveListener(OnDeath);
    }

    private void FixedUpdate()
    {
        if (health.IsDead()) return;

        // Определяем состояние
        UpdateStatus();

        // Действем в зависимости от состояния
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                ChasePlayer();
                break;
        }
    }

    private void UpdateStatus()
    {
        if (player == null) return;

        float distatnceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distatnceToPlayer <= detectionRange)
        {
            currentState = EnemyState.Chase;
        }
        else if (shouldPatrol)
        {
            currentState = EnemyState.Patrol;
        }
    }

    private void Patrol()
    {
        if (isPaused)
        {
            patrolTimer -= Time.fixedDeltaTime;
            if (patrolTimer <= 0)
            {
                isPaused = false;
                patrolTimer = patrolPauseTime;
            }
            return;
        }

        // Проверяем препятствия
        if (IsWallAhead() || !IsGroundAhead())
        {
            // Стена впереди или обрыв - разворачиваемся
            Flip();
            isPaused = true; // Пауза после разворота
            return;
        }

        // Двигаемся в текущем направлении
        Move();
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        // Проверяем только движение по горизонтали
        float directionX = Mathf.Sign(player.position.x - transform.position.x);

        // Проверяем препятствия только если идём к игроку
        if ((directionX > 0 && isFacingRight) || (directionX < 0 && !isFacingRight))
        {
            // Если стена - останавливаемся
            if (IsWallAhead())
            {
                return;
            }
        }

        // Поворачиваемся к игроку
        if (directionX > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (directionX < 0 && isFacingRight)
        {
            Flip();
        }

        Move();
    }

    private void Move()
    {
        float horizontalMovement = (isFacingRight ? 1 : -1) * moveSpeed * Time.fixedDeltaTime;
        Vector2 newPosition = new Vector2(rb.position.x + horizontalMovement, rb.position.y);
        rb.MovePosition(newPosition);
    }

    private bool IsGroundAhead()
    {
        if (groundCheck == null) return true;

        // Проверяем есть ли земля впереди (чтобы не упасть)
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    private bool IsWallAhead()
    {
        if (wallCheck == null) return false;

        // Проверяем стену впереди
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, direction, wallCheckDistance, groundLayer);
        return hit.collider != null;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDeath()
    {
        // Спавним эффект смерти
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Vector3 direction = isFacingRight ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + direction * wallCheckDistance);
        }
    }
}
