using UnityEngine;

[CreateAssetMenu(fileName = "New Bullet", menuName = "Weapons/Bullet Data")]
public class BulletData : ScriptableObject
{
    [Header("Bullet Info")]
    public string bulletName;

    [Header("Damage")]
    public int damage = 10;
    public float impactForce = 3f; // Сила отталкивания при попадании

    [Header("Movement")]
    public float speed = 20f;
    public float lifetime = 5f; // Время жизни (секунды)
    public bool affectedByGravity = false;
    public float gravityScale = 0f;

    [Header("Penetration")]
    public bool canPenetrateEnemies = false;
    public int maxPenetrarions = 0; // Сколько врагов может пробить

    [Header("Visual")]
    public Sprite bulletSprite;
    public Vector2 spriteScale = Vector2.one;
    public Color trailColor = Color.yellow;
    public float trailWidth = 0.1f;
    public float trailTime = 0.2f;

    [Header("Hit Effects")]
    public GameObject hitEnemyEffectPrefab;
    public GameObject hitWallEffectPrefab;
    public AudioClip hitEnemySound;
    public AudioClip hitWallSound;
}
