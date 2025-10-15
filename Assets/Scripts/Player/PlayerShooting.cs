using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private WeaponData currentWeapon;
    [SerializeField] private Transform firePoint; // Точка, откуда вылетает пуля

    private PlayerInputAction inputActions;
    private float nextFireTime;
    private bool isFacingRight = true;

    private Rigidbody2D rb;

    private void Awake()
    {
        inputActions = new PlayerInputAction();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Shoot.performed += OnShoot;
    }

    private void OnDisable()
    {
        inputActions.Player.Shoot.performed -= OnShoot;
        inputActions.Disable();
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        // Проверяем, можем ли стрелять (fire rate)
        if (Time.time >= nextFireTime && currentWeapon != null)
        {
            Shoot();
            nextFireTime = Time.time + currentWeapon.fireRate;
        }
    }

    private void Shoot()
    {
        // Определяем направление выстрела
        Vector2 shootDirection = isFacingRight ? Vector2.right : Vector2.left;

        // Создаём пулю
        GameObject bullet = Instantiate(currentWeapon.bulletPrefab, firePoint.position, Quaternion.identity);

        // Инициализируем пулю
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(currentWeapon.damage, currentWeapon.bullerSpeed, shootDirection);
        }

        // Применяем отдачу
        ApplyRecoil(shootDirection);

        // Трясём камеру
        CameraShake.Shake(currentWeapon.shakeDuration, currentWeapon.shakeMagnitude);

        // TODO: Добавить отдачу и эффекты позже
        Debug.Log("Shot fired!");
    }

    private void ApplyRecoil(Vector2 shootDirection)
    {
        if (rb != null && currentWeapon != null)
        {
            // Отбрасываем игрока в противоположную сторону
            Vector2 recoilDirection = -shootDirection;
            rb.AddForce(recoilDirection * currentWeapon.recoilForce, ForceMode2D.Impulse);
        }
    }

    // Метод для определения направления взгляда (вызовем из PlayerMovement)
    public void SetFacingDirection(bool facingRight)
    {
        isFacingRight = facingRight;
    }
}
