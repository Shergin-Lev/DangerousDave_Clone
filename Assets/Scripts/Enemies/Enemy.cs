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
        // ������������� �� ������� ������
        health.OnDeath.AddListener(OnDeath);
    }

    private void OnDisable()
    {
        // ������������ �� ������� ������
        health.OnDeath.RemoveListener(OnDeath);
    }

    private void OnDeath()
    {
        // ������� ������ ������
        if (deatchEffectPrefab != null)
        {
            Instantiate(deatchEffectPrefab, transform.position, Quaternion.identity);
            Debug.Log("DEATH PARTICLES!!!");
        }

        Destroy(gameObject);
    }

}
