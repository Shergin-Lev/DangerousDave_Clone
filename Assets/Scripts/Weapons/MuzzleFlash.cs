using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.1f;

    private void Start()
    {
        // ��������� ������� ��� ������������
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        // �������������� �����������
        Destroy(gameObject, lifetime);
    }
}
