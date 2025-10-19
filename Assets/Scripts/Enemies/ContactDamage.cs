using System.Collections.Generic;
using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damageAmout = 10;
    [SerializeField] private float damageCooldown = 1f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 5f;

    // private float lastDamageTime;

    private Dictionary<GameObject, float> lastDamageTimes = new Dictionary<GameObject, float>();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject target = collision.gameObject;

        // Проверяем cooldown для ЭТОЙ конкретной цели
        if (lastDamageTimes.ContainsKey(target))
        {
            if (Time.time - lastDamageTimes[target] < damageCooldown)
                return;
        }

        // Проверяем неуязвимость игрока
        PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
        if (playerHealth != null && playerHealth.IsInvincible())
        {
            return;
        }

        // Наносим урон
        Health health = target.GetComponent<Health>();
        if (health != null && !health.IsDead())
        {
            health.TakeDamage(damageAmout);

            // Обновляем время последнего урона для этой цели
            lastDamageTimes[target] = Time.time;

            // Применяем отталкивание
            ApplyKnockback(collision);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (lastDamageTimes.ContainsKey(collision.gameObject))
        {
            lastDamageTimes.Remove(collision.gameObject);
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

            targetRb.linearVelocity = knockbackDirection * knockbackForce;

            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.ApplyKnockback(0.3f);
            }
        }
    }
}
