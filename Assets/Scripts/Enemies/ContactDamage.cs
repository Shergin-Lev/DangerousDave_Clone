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
        // Проверяем, прошло ли достаточно времени с последнего урона
        if (Time.time - lastDamageTime < damageAmout) return;

        // Проверяем неуязвимость игрока
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null && playerHealth.IsInvincible())
        {
            return; // Игрок неуязвим - не наносим урон
        }
        
        // Пытаемся нанести урон
        Health health = collision.gameObject.GetComponent<Health>();
        if(health != null)
        {
            health.TakeDamage(damageAmout);
            lastDamageTime = Time.time;

            // Применяем отталкивание
            ApplyKnockback(collision);
        }
    }

    private void ApplyKnockback(Collision2D collision)
    {
        Rigidbody2D targetRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (targetRb != null && knockbackForce > 0)
        {
            // Направление отталкивания - от врага к цели
            Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;

            // Добавляем вертикальную состовляющую для "подброса"
            knockbackDirection.y = 0.5f;
            knockbackDirection.Normalize();

            // Применяем силу
            targetRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }
    }
}
