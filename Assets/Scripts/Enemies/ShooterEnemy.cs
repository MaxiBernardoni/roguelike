using UnityEngine;

/// <summary>
/// Mantiene distancia respecto al jugador y dispara proyectiles a intervalos.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class ShooterEnemy : EnemyBase
{
    [SerializeField] float screenBoundsMargin = 0.45f;
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] float preferredDistance = 5f;
    [SerializeField] float moveSpeed = 2.5f;
    [SerializeField] float shootInterval = 1.6f;
    [SerializeField] int shotDamage = 8;
    [SerializeField] float shotSpeed = 12f;

    Transform player;
    Rigidbody2D rb;
    float shootTimer;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        CombatVisuals.ApplyShooter(GetComponent<SpriteRenderer>());
    }

    void Start()
    {
        if (PlayerController.Instance != null)
            player = PlayerController.Instance.transform;
        shootTimer = shootInterval * 0.5f;
        if (projectilePrefab == null && CombatReferences.PlayerProjectilePrefab != null)
            projectilePrefab = CombatReferences.PlayerProjectilePrefab;
    }

    void Update()
    {
        if (player == null && PlayerController.Instance != null)
            player = PlayerController.Instance.transform;

        if (player == null)
            return;

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            shootTimer = 0f;
            Fire();
        }
    }

    void FixedUpdate()
    {
        if (player == null && PlayerController.Instance != null)
            player = PlayerController.Instance.transform;

        if (player == null)
            return;

        Vector2 toPlayer = (Vector2)player.position - rb.position;
        float dist = toPlayer.magnitude;
        Vector2 dir = dist > 0.01f ? toPlayer.normalized : Vector2.right;

        if (dist < preferredDistance * 0.85f)
            rb.linearVelocity = -dir * moveSpeed;
        else if (dist > preferredDistance * 1.15f)
            rb.linearVelocity = dir * moveSpeed * 0.5f;
        else
            rb.linearVelocity = Vector2.zero;

        ScreenBounds.ClampRigidbody(rb, screenBoundsMargin);
    }

    void Fire()
    {
        if (projectilePrefab == null || player == null)
            return;

        Vector2 origin = rb.position;
        Vector2 aim = ((Vector2)player.position - origin).normalized;
        var proj = Instantiate(projectilePrefab, origin, Quaternion.identity);
        proj.Init(aim, shotDamage, shotSpeed, 0, 0, false, 0f, 0, false);
    }
}
