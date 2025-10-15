using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage;
    private float speed;
    private Vector2 direction;

    public void Initialize(int bullerDamage, float bulletSpeed, Vector2 bulletDirection)
    {
        damage = bullerDamage;
        speed = bulletSpeed;
        direction = bulletDirection.normalized;
    }

    private void Update()
    {
        // ƒвигаем пулю
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ѕытаемс€ нанести урон
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        // ”ничтожаем пулю
        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        // ”ничтожаем пулю, если она вылетела за пределы экрана
        Destroy(gameObject);
    }
}
