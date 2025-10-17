using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damageAmout = 10;
    [SerializeField] private float damageCooldown = 1f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 5f;

    private float lastDamageTime;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ���������, ������ �� ���������� ������� � ���������� �����
        if (Time.time - lastDamageTime < damageAmout) return;

        // ��������� ������������ ������
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null && playerHealth.IsInvincible())
        {
            return; // ����� �������� - �� ������� ����
        }
        
        // �������� ������� ����
        Health health = collision.gameObject.GetComponent<Health>();
        if(health != null)
        {
            health.TakeDamage(damageAmout);
            lastDamageTime = Time.time;

            // ��������� ������������
            ApplyKnockback(collision);
        }
    }

    private void ApplyKnockback(Collision2D collision)
    {
        Rigidbody2D targetRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (targetRb != null && knockbackForce > 0)
        {
            // ����������� ������������ - �� ����� � ����
            Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;

            // ��������� ������������ ������������ ��� "��������"
            knockbackDirection.y = 0.5f;
            knockbackDirection.Normalize();

            // ��������� ����
            targetRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }
    }
}
