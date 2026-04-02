using UnityEngine;

/// <summary>
/// Disparo manual (ratón izquierdo), cadencia, munición y recarga. Mejoras: multidisparo, rebotes, explosiones, etc.
/// </summary>
public class WeaponController : MonoBehaviour
{
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float fireRate = 4f;
    [SerializeField] int baseDamage = 12;
    [SerializeField] float projectileSpeed = 18f;

    [Header("Ammo")]
    [SerializeField] int maxAmmo = 8;
    [SerializeField] float reloadTime = 1.15f;

    PlayerController player;

    int currentAmmo;
    bool isReloading;
    float reloadEndTime;
    float nextFireTime;

    int extraProjectiles;
    int projectileBounces;
    int pierceExtraHits;
    bool explosive;
    float explosionRadius = 2f;
    int explosionDamage = 8;
    bool spawnOnKill;
    bool subscribedKillEvent;
    float lowHpDamageMultiplier = 1f;

    public int BaseDamage => baseDamage;
    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;
    public bool IsReloading => isReloading;
    public float ReloadTime => reloadTime;
    public float ReloadRemaining => isReloading ? Mathf.Max(0f, reloadEndTime - Time.time) : 0f;

    void Awake()
    {
        player = GetComponentInParent<PlayerController>();
        if (firePoint == null)
            firePoint = transform;
        if (projectilePrefab != null)
            CombatReferences.RegisterPlayerProjectile(projectilePrefab);
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (player == null || !player.gameObject.activeSelf)
            return;

        if (isReloading)
        {
            if (Time.time >= reloadEndTime)
            {
                isReloading = false;
                currentAmmo = maxAmmo;
            }

            return;
        }

        if (!Input.GetMouseButton(0))
            return;

        if (Time.time < nextFireTime)
            return;

        if (currentAmmo <= 0)
        {
            StartReload();
            return;
        }

        Fire();
        currentAmmo--;
        nextFireTime = Time.time + 1f / fireRate;

        if (currentAmmo <= 0)
            StartReload();
    }

    void StartReload()
    {
        if (isReloading)
            return;
        isReloading = true;
        reloadEndTime = Time.time + reloadTime;
    }

    void Fire()
    {
        Vector2 origin = firePoint.position;
        Vector2 dir = GetMouseAimDirection(origin);
        int count = 1 + extraProjectiles;
        float spread = count > 1 ? 18f : 0f;

        for (int i = 0; i < count; i++)
        {
            float angle = spread * (i - (count - 1) * 0.5f);
            Vector2 d = Rotate(dir, angle);
            SpawnProjectileFrom(firePoint.position, d);
        }
    }

    /// <summary>Dirección desde <paramref name="origin"/> hacia el cursor en mundo (sin auto-apuntado).</summary>
    static Vector2 GetMouseAimDirection(Vector2 origin)
    {
        Camera cam = Camera.main;
        if (cam == null)
            cam = FindFirstObjectByType<Camera>();
        Vector3 mp = cam != null
            ? cam.ScreenToWorldPoint(Input.mousePosition)
            : (Vector3)origin;
        mp.z = 0f;
        Vector2 d = (Vector2)mp - origin;
        if (d.sqrMagnitude < 0.0001f)
            return Vector2.right;
        return d.normalized;
    }

    void SpawnProjectileFrom(Vector2 origin, Vector2 dir)
    {
        if (projectilePrefab == null)
            return;

        int dmg = Mathf.RoundToInt(baseDamage * DamageMultiplier());
        var p = Instantiate(projectilePrefab, origin, Quaternion.identity);
        p.Init(
            dir,
            dmg,
            projectileSpeed,
            projectileBounces,
            pierceExtraHits,
            explosive,
            explosionRadius,
            explosionDamage,
            true);
    }

    void OnEnemyKilledForUpgrade(Vector2 deathPos)
    {
        if (!spawnOnKill)
            return;
        Vector2 d = GetMouseAimDirection(deathPos);
        SpawnProjectileFrom(deathPos, d);
    }

    float DamageMultiplier()
    {
        float m = 1f;
        if (player != null && player.IsLowHealth)
            m *= lowHpDamageMultiplier;
        return m;
    }

    static Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float c = Mathf.Cos(rad), s = Mathf.Sin(rad);
        return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
    }

    public void AddFireRateMultiplier(float mult)
    {
        fireRate *= mult;
    }

    public void AddExtraProjectile()
    {
        extraProjectiles++;
    }

    public void SetProjectileBounces(int count)
    {
        projectileBounces = Mathf.Max(projectileBounces, count);
    }

    public void AddPierceHits(int extra)
    {
        pierceExtraHits += extra;
    }

    public void EnableExplosive(float radius, int bonusDmg)
    {
        explosive = true;
        explosionRadius = radius;
        explosionDamage = bonusDmg;
    }

    public void EnableSpawnOnKill()
    {
        spawnOnKill = true;
        if (subscribedKillEvent)
            return;
        subscribedKillEvent = true;
        EnemyBase.OnEnemyKilled += OnEnemyKilledForUpgrade;
    }

    public bool HasSpawnOnKill => spawnOnKill;

    void OnDestroy()
    {
        if (subscribedKillEvent)
            EnemyBase.OnEnemyKilled -= OnEnemyKilledForUpgrade;
    }

    public void SetLowHpDamageMultiplier(float mult)
    {
        lowHpDamageMultiplier = mult;
    }

    public void AddBaseDamage(int amount)
    {
        baseDamage += amount;
    }

    public void FireBurstToward(Vector2 worldTarget)
    {
        Vector2 origin = firePoint.position;
        Vector2 dir = (worldTarget - origin).normalized;
        if (dir.sqrMagnitude < 0.01f)
            dir = Vector2.right;
        SpawnProjectileFrom(origin, dir);
    }
}
