using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private WeaponData currentWeapon;
    [SerializeField] private Transform firePoint; // �����, ������ �������� ����

    private PlayerInputAction inputActions;
    private Rigidbody2D rb;
    private float nextFireTime;
    private bool isFacingRight = true;

    // ��� ���������� ������
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
        // ���������, ����� �� �������� (fire rate)
        if (Time.time >= nextFireTime && currentWeapon != null)
        {
            Shoot();
            nextFireTime = Time.time + currentWeapon.fireRate;
        }
    }

    private void Shoot()
    {
        // ���������� ����������� ��������
        Vector2 shootDirection = isFacingRight ? Vector2.right : Vector2.left;

        // ������ ����
        GameObject bullet = Instantiate(currentWeapon.bulletPrefab, firePoint.position, Quaternion.identity);

        // �������������� ����
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null && currentWeapon.bulletData != null)
        {
            bulletScript.Initialize(currentWeapon.bulletData, shootDirection, gameObject);
        }

        // ������������� ������ (���������� � FixedUpdate)
        shouldApplyRecoil = true;
        pendingRecoilDirection = -shootDirection;

        // ����� ������
        CameraShake.Shake(currentWeapon.shakeDuration, currentWeapon.shakeMagnitude);

        // ������� �������
        if (currentWeapon.muzzleFlashPrefab != null)
        {
            Instantiate(currentWeapon.muzzleFlashPrefab, firePoint.position, Quaternion.identity);
        }
    }

    private void FixedUpdate()
    {
        // ��������� ������ � FixedUpdate() ������������� � �������
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

    // ����� ��� ����������� ����������� ������� (������� �� PlayerMovement)
    public void SetFacingDirection(bool facingRight)
    {
        isFacingRight = facingRight;
    }
}