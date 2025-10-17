using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Info")]
    public string weaponId; // Уникальный ID оружия (например "pistol", "shotgun")
    public Sprite weaponSprite;

    [Header("Shooting")]
    public BulletData bulletData;
    public GameObject bulletPrefab;
    public float fireRate = 0.5f; // Время между выстрелами

    [Header("Recoil")]
    public float recoilForce = 2f; // Отдача от выстрела

    [Header("Effects")]
    public GameObject muzzleFlashPrefab; // Вспышка выстрела
    public AudioClip shootSound;

    [Header("Screen Shake")]
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.1f; 

    public string GetLocalizedName()
    {
        return LocalizationManager.GetText($"weapon_{weaponId}_name");
    }

    public string GetLocalizationDescription()
    {
        return LocalizationManager.GetText($"weapon_{weaponId}_description");
    }
}
