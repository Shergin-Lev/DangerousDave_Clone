using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Bullet : MonoBehaviour
{
    private BulletData bulletData;
    private Vector2 direction;
    private float currentLifeTime;
    private int penetrationCount;
    private Rigidbody2D rb;
    private TrailRenderer trail;
    private SpriteRenderer spriteRenderer;

    public void Initialize(BulletData data, Vector2 bulletDirection)
    {
        bulletData = data;
        direction = bulletDirection.normalized;
        currentLifeTime = 0f;
        penetrationCount = 0;

        // Настраиваем визуал
        SetupVisual();

        // Настраиваем физику если нужна гравитация
        if (bulletData.affectedByGravity)
        {
            SetupPhysics();
        }
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
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = bulletData.gravityScale;
        rb.linearVelocity = direction * bulletData.speed;
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

        // Если не используем физик - двигаем вручную
        if (!bulletData.affectedByGravity)
        {
            MoveBullet();
        }
    }

    private void MoveBullet()
    {
        Vector2 currentPosition = transform.position;
        Vector2 movement = direction * bulletData.speed * Time.deltaTime;
        Vector2 newPosition = currentPosition + movement;

        // Проверяем попадания
        RaycastHit2D hit = Physics2D.Raycast(currentPosition, direction, movement.magnitude);

        if (hit.collider != null)
        {
            OnHit(hit);
            return;
        }

        transform.position = newPosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Для пуль с физикой используем Trigger
        if (bulletData.affectedByGravity)
        {
            RaycastHit2D hit = new RaycastHit2D();
            // Создаём "фейковый" hit для совместимости
            OnHit(hit, other);
        }
    }

    private void OnHit(RaycastHit2D hit, Collider2D collider = null)
    {
        Collider2D targetCollider = collider ?? hit.collider;
        Vector2 hitPoint = collider != null ? transform.position : (Vector2)hit.point;

        // Проверяем, попал ли в "живую" цель
        IDamageable damageable = targetCollider.GetComponent<IDamageable>();
        Health health = targetCollider.GetComponent<Health>();

        if (damageable != null)
        {
            // Попадение в живую цель
            damageable.TakeDamage(bulletData.damage);
            ApplyImpact(targetCollider);
            SpawnHitEffect(bulletData.hitEnemyEffectPrefab, hitPoint, hit.normal);

            // Проверяем пробитие
            if (bulletData.canPenetrateEnemies && penetrationCount < bulletData.maxPenetrarions)
            {
                penetrationCount++;
                return; // Не уничтожаем пулю
            }
        }
        else
        {
            // Попадение в стену
            SpawnHitEffect(bulletData.hitWallEffectPrefab, hitPoint, hit.normal);              
        }

        // Уничтожаем пулю
        DestroyBullet();
    }

    private void ApplyImpact(Collider2D target)
    {
        Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
        if (targetRb != null && bulletData.impactForce > 0)
        {
            // Отталкиваем цель в направлении полёта пули
            targetRb.AddForce(direction * bulletData.impactForce, ForceMode2D.Impulse);
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
