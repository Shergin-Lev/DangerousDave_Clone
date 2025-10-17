using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Invincibility")]
    [SerializeField] private float invincibilityDuration = 1.5f;
    [SerializeField] private float blinkInterval = 0.1f;

    private Health health;
    private SpriteRenderer spriteRenderer;
    private bool isInvincible;
    private float invincibilityTimer;
    private float blinkTimer;

    private void Awake()
    {
        health = GetComponent<Health>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        health.OnHealthChanged.AddListener(OnHealthChanged);
        health.OnDeath.AddListener(OnDeath);
    }

    private void OnDisable()
    {
        health.OnHealthChanged.RemoveListener(OnHealthChanged);
        health.OnDeath.RemoveListener(OnDeath);
    }

    private void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            blinkTimer -= Time.deltaTime;

            // ������� �������
            if (blinkTimer <= 0)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                blinkTimer = blinkInterval;
            }

            // ����� ������������
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
                spriteRenderer.enabled = true; // �������� ��� ������ �����
            }
        }
    }

    private void OnHealthChanged(int currentHealth)
    {
        Debug.Log($"Player health: {currentHealth}/{health.GetMaxHealth()}");

        // ���������� ������������
        if (!isInvincible && currentHealth < health.GetMaxHealth())
        {
            ActivateInvincibility();
        }
    }

    private void ActivateInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        blinkTimer = blinkInterval;
    }

    private void OnDeath()
    {
        Debug.Log("Player Died! Game over!");
        // TODO: ����� ������� ����� Game Over
        // ���� ������ ������������� �����
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public bool IsInvincible() => isInvincible;
}
