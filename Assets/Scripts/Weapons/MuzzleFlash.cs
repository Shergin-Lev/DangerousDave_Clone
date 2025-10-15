using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.1f;

    private void Start()
    {
        // Случайный поворот для разнообразия
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        // Автоматическое уничтожение
        Destroy(gameObject, lifetime);
    }
}
