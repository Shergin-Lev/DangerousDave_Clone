using UnityEngine;

[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour
{
    [Header("Death Effects")]
    [SerializeField] private GameObject deatchEffectPrefab;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
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

    private void OnDeath()
    {
        // Спавним эффект смерти
        if (deatchEffectPrefab != null)
        {
            Instantiate(deatchEffectPrefab, transform.position, Quaternion.identity);
            Debug.Log("DEATH PARTICLES!!!");
        }

        Destroy(gameObject);
    }

}
