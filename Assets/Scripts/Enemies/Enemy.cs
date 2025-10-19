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
    [SerializeField] private Transform groundCheck; // ����� �������� ����� �������
    [SerializeField] private Transform wallCheck;   // ����� �������� �����
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.5f;
    [SerializeField] private float wallCheckDistance = 0.3f;

    [Header("Patrol")]
    [SerializeField] private bool shouldPatrol = true;
    [SerializeField] private float patrolPauseTime = 2f; // ����� ����� ����� ����������

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

        // ������� ������ �� ����
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        patrolTimer = patrolPauseTime;
    }

    private void OnEnable()
    {
        // ������������� �� ������� ������
        health.OnDeath.AddListener(OnDeath);
    }

    private void OnDisable()
    {
        // ������������ �� ������� ������
        health.OnDeath.RemoveListener(OnDeath);
    }

    private void FixedUpdate()
    {
        if (health.IsDead()) return;

        // ���������� ���������
        UpdateStatus();

        // �������� � ����������� �� ���������
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

        // ��������� �����������
        if (IsWallAhead() || !IsGroundAhead())
        {
            // ����� ������� ��� ����� - ���������������
            Flip();
            isPaused = true; // ����� ����� ���������
            return;
        }

        // ��������� � ������� �����������
        Move();
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        // ��������� ������ �������� �� �����������
        float directionX = Mathf.Sign(player.position.x - transform.position.x);

        // ��������� ����������� ������ ���� ��� � ������
        if ((directionX > 0 && isFacingRight) || (directionX < 0 && !isFacingRight))
        {
            // ���� ����� - ���������������
            if (IsWallAhead())
            {
                return;
            }
        }

        // �������������� � ������
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

        // ��������� ���� �� ����� ������� (����� �� ������)
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    private bool IsWallAhead()
    {
        if (wallCheck == null) return false;

        // ��������� ����� �������
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
        // ������� ������ ������
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
