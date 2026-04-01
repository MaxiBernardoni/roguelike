using UnityEngine;

/// <summary>
/// Disparo automático hacia el enemigo más cercano o hacia el cursor.
/// Las mejoras modifican cadencia, multidisparo, rebotes, explosiones y callbacks de muerte.
/// </summary>
public class WeaponController : MonoBehaviour
{
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float fireRate = 3f;
    [SerializeField] int baseDamage = 12;
    [SerializeField] float projectileSpeed = 18f;
    [SerializeField] float aimRange = 30f;
    [SerializeField] LayerMask enemyLayer;

    PlayerController player;

    float fireTimer;
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

    void Awake()
    {
        player = GetComponentInParent<PlayerController>();
        if (firePoint == null)
            firePoint = transform;
        if (projectilePrefab != null)
            CombatReferences.RegisterPlayerProjectile(projectilePrefab);
    }

    void Start()
    {
        enemyLayer = GameLayers.GetEnemyMask(enemyLayer);
    }

    void Update()
    {
        if (player == null || !player.gameObject.activeSelf)
            return;

        fireTimer += Time.deltaTime;
        float interval = 1f / fireRate;
        while (fireTimer >= interval)
        {
            fireTimer -= interval;
            Fire();
        }
    }

    void Fire()
    {
        Vector2 origin = firePoint.position;
        Vector2 dir = GetAimDirection(origin);
        int count = 1 + extraProjectiles;
        float spread = count > 1 ? 18f : 0f;

        for (int i = 0; i < count; i++)
        {
            float angle = spread * (i - (count - 1) * 0.5f);
            Vector2 d = Rotate(dir, angle);
            SpawnProjectileFrom(firePoint.position, d);
        }
    }

    Vector2 GetAimDirection(Vector2 origin)
    {
        Collider2D nearest = null;
        float best = aimRange * aimRange;
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, aimRange, enemyLayer);
        foreach (var h in hits)
        {
            float d = ((Vector2)h.transform.position - origin).sqrMagnitude;
            if (d < best)
            {
                best = d;
                nearest = h;
            }
        }

        if (nearest != null)
            return ((Vector2)nearest.transform.position - origin).normalized;

        Camera cam = Camera.main;
        if (cam == null)
            cam = FindFirstObjectByType<Camera>();
        Vector3 mp = cam != null
            ? cam.ScreenToWorldPoint(Input.mousePosition)
            : (Vector3)origin;
        mp.z = 0f;
        return ((Vector2)mp - origin).normalized;
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
        Vector2 d = GetAimDirection(deathPos);
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

    // --- Mejoras (llamadas desde Upgrade.ApplyEffect vía PlayerController) ---

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
