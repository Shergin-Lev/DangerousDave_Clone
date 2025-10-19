using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private WeaponData currentWeapon;
    [SerializeField] private Transform firePoint; // Точка, откуда вылетает пуля

    private PlayerInputAction inputActions;
    private Rigidbody2D rb;
    private float nextFireTime;
    private bool isFacingRight = true;

    // Для отложенной отдачи
    private bool shouldApplyRecoil = false;
    private Vector2 pendingRecoilDirection;

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
        if (bulletScript != null && currentWeapon.bulletData != null)
        {
            bulletScript.Initialize(currentWeapon.bulletData, shootDirection, gameObject);
        }

        // Запланировать отдачу (применится в FixedUpdate)
        shouldApplyRecoil = true;
        pendingRecoilDirection = -shootDirection;

        // Трясём камеру
        CameraShake.Shake(currentWeapon.shakeDuration, currentWeapon.shakeMagnitude);

        // Спавним вспышку
        if (currentWeapon.muzzleFlashPrefab != null)
        {
            Instantiate(currentWeapon.muzzleFlashPrefab, firePoint.position, Quaternion.identity);
        }
    }

    private void FixedUpdate()
    {
        // Применяем отдачу в FixedUpdate() синхронизация с физикой
        if (shouldApplyRecoil)
        {
            ApplyRecoil();
            shouldApplyRecoil = false;
        }
    }

    private void ApplyRecoil()
    {
        if (rb != null && currentWeapon != null)
        {
            rb.AddForce(pendingRecoilDirection * currentWeapon.recoilForce, ForceMode2D.Impulse);
        }
    }

    // Метод для определения направления взгляда (вызовем из PlayerMovement)
    public void SetFacingDirection(bool facingRight)
    {
        isFacingRight = facingRight;
    }
}