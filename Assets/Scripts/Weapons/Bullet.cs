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
        // ������� ����
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �������� ������� ����
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        // ���������� ����
        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        // ���������� ����, ���� ��� �������� �� ������� ������
        Destroy(gameObject);
    }
}
