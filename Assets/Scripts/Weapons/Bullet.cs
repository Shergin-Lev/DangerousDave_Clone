using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private BulletData bulletData;
    private Vector2 direction;
    private float currentLifeTime;
    private int penetrationCount;
    private Rigidbody2D rb;
    private TrailRenderer trail;
    private SpriteRenderer spriteRenderer;
    private GameObject owner; // Чья пуля

    public void Initialize(BulletData data, Vector2 bulletDirection, GameObject bulletOwner = null)
    {
        bulletData = data;
        direction = bulletDirection.normalized;
        currentLifeTime = 0f;
        penetrationCount = 0;
        owner = bulletOwner;

        // Настраиваем визуал и физику
        SetupVisual();
        SetupPhysics();
    }

    private void SetupVisual()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (bulletData.bulletSprite != null)
        {
            spriteRenderer.sprite = bulletData.bulletSprite;
        }

        transform.localScale = bulletData.spriteScale;

        // Поварачиваем спрайт в направлении полёта
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Настраиваем Trail Renderer
        trail = GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.startColor = bulletData.trailColor;
            trail.endColor = new Color(bulletData.trailColor.r, bulletData.trailColor.g, bulletData.trailColor.b, 0);
            trail.startWidth = bulletData.trailWidth;
            trail.time = bulletData.trailTime;
        }
    }

    private void SetupPhysics()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = bulletData.affectedByGravity ? bulletData.gravityScale : 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        rb.linearVelocity = direction * bulletData.speed;

        if (owner != null)
        {
            Collider2D ownerCollider = owner.GetComponent<Collider2D>();
            Collider2D bulletCollider = GetComponent<Collider2D>();
            if (ownerCollider != null && bulletCollider != null)
            {
                Physics2D.IgnoreCollision(bulletCollider, ownerCollider);
            }
        }
    }

    private void Update()
    {
        // Проверяем время жизни
        currentLifeTime += Time.deltaTime;
        if (currentLifeTime >= bulletData.lifetime)
        {
            DestroyBullet();
            return;
        }

        if (bulletData.rotateInFlight)
        {
            transform.Rotate(0, 0, bulletData.rotationSpeed * Time.deltaTime);
        }

        if (bulletData.affectedByGravity && rb.linearVelocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Для пуль с физикой используем Trigger
        if (owner != null && collision.gameObject == owner)
            return;

        OnHit(collision);
    }

    private void OnHit(Collision2D collision)
    {
        Collider2D targetCollider = collision.collider;
        Vector2 hitPoint = collision.contacts[0].point;
        Vector2 hitNormal = collision.contacts[0].normal;

        IDamageable damageable = targetCollider.GetComponent<IDamageable>();

        if (damageable != null)
        {
            // Попадаем в живую цель
            damageable.TakeDamage(bulletData.damage);
            ApplyImpact(targetCollider);
            SpawnHitEffect(bulletData.hitEnemyEffectPrefab, hitPoint, hitNormal);

            // Проверяем пробитие
            if (bulletData.canPenetrateEnemies && penetrationCount < bulletData.maxPenetrations)
            {
                penetrationCount++;

                // Сохраняем оригинальную скорость до игнорирования коллизии
                Vector2 originalVelocity = rb.linearVelocity;

                // Игнорируем коллизии с этой целью
                Collider2D bulletCollider = GetComponent<Collider2D>();
                Physics2D.IgnoreCollision(bulletCollider, targetCollider);

                // Восстанавливаем скорость после столкновения
                rb.linearVelocity = originalVelocity;

                return;
            }
        }
        else
        {
            SpawnHitEffect(bulletData.hitWallEffectPrefab, hitPoint, hitNormal);
        }

        DestroyBullet();
    }


    private void ApplyImpact(Collider2D target)
    {
        Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
        if (targetRb != null && bulletData.impactForce > 0)
        {
            Vector2 impactDirection = rb.linearVelocity.normalized;
            targetRb.AddForce(impactDirection * bulletData.impactForce, ForceMode2D.Impulse);
        }
    }

    private void SpawnHitEffect(GameObject effectPrefab, Vector2 position, Vector2 normal)
    {
        if (effectPrefab != null)
        {
            // Создаём эффектб повёрнутый к поверхности
            float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;
            Instantiate(effectPrefab, position, Quaternion.Euler(0, 0, angle));
        }
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        DestroyBullet();
    }
}
